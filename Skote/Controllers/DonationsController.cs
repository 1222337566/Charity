using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastrfuctureManagmentCore.Domains.Item;
using InfrastrfuctureManagmentCore.Domains.Payments;
using InfrastrfuctureManagmentCore.Domains.Warehouses;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Donors;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentServices.Stock;
using InfrastructureManagmentWebFramework.Models.Charity.DonorProfile;
using InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations;
using InfrastructureManagmentWebFramework.Models.Charity.Donors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class DonationsController : Controller
{
    private const string DonationReceiptSourceType = "DonationInKind";
    private readonly IDonationRepository _donationRepository;
    private readonly IDonorRepository _donorRepository;
    private readonly IOperationalJournalHookService _journalHookService;
    private readonly IStockService _stockService;
    private readonly AppDbContext _db;

    public DonationsController(
        IDonationRepository donationRepository,
        IDonorRepository donorRepository,
        IOperationalJournalHookService journalHookService,
        IStockService stockService,
        AppDbContext db)
    {
        _donationRepository = donationRepository;
        _donorRepository = donorRepository;
        _journalHookService = journalHookService;
        _stockService = stockService;
        _db = db;
    }

    public async Task<IActionResult> Index(Guid? donorId = null)
    {
        List<Donation> items;

        if (donorId.HasValue && donorId.Value != Guid.Empty)
        {
            if (!await PopulateDonorAsync(donorId.Value))
                return NotFound();

            items = await _donationRepository.GetByDonorIdAsync(donorId.Value);
        }
        else
        {
            ViewBag.PageSubtitle = "عرض عام لكل التبرعات المسجلة على مستوى الجمعية";
            items = await _db.CharityDonations
                .AsNoTracking()
                .Include(x => x.Donor)
                .Include(x => x.AidType)
                .Include(x => x.PaymentMethod)
                .Include(x => x.FinancialAccount)
                .OrderByDescending(x => x.DonationDate)
                .ThenByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }

        var model = items.Select(x =>
        {
            var scope = ResolveTargetingScopeCode(x);
            return new DonationListItemVm
            {
                Id = x.Id,
                DonationNumber = x.DonationNumber,
                DonationDate = x.DonationDate,
                DonationType = x.DonationType,
                Amount = x.Amount,
                PaymentMethod = x.PaymentMethod?.MethodNameAr,
                FinancialAccount = x.FinancialAccount?.AccountNameAr,
                ReceiptNumber = x.ReceiptNumber,
                IsRestricted = x.IsRestricted,
                CampaignName = x.CampaignName,
                TargetingScopeCode = scope,
                TargetingScopeName = InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations.DonationTargetingScopeOption.GetArabicName(scope),
                GeneralPurposeName = string.IsNullOrWhiteSpace(x.GeneralPurposeName) ? x.CampaignName : x.GeneralPurposeName,
                ReferenceNumber = x.ReferenceNumber,
                Notes = x.Notes,
                DonorName = x.Donor?.FullName
            };
        }).ToList();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid donorId)
    {
        if (!await PopulateDonorAsync(donorId))
            return NotFound();

        var vm = new CreateDonationVm
        {
            DonorId = donorId,
            DonationDate = DateTime.Today,
            DonationNumber = await GenerateDonationNumberAsync(),
            DonationType = DonationTypeOption.Values.First(),
            TargetingScopeCode = InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations.DonationTargetingScopeOption.SpecificRequests
        };

        await FillLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDonationVm vm, string? afterSave = null)
    {
        if (!await PopulateDonorAsync(vm.DonorId))
            return NotFound();

        await FillLookupsAsync(vm);
        NormalizeScopeFields(vm);
        ValidateScopeRules(vm);

        if (!ModelState.IsValid)
            return View(vm);

        if (await _donationRepository.DonationNumberExistsAsync(vm.DonationNumber.Trim()))
        {
            ModelState.AddModelError(nameof(vm.DonationNumber), "رقم التبرع موجود بالفعل");
            return View(vm);
        }

        var isInKind = IsInKindDonation(vm.DonationType);
        var estimatedTotal = ResolveInKindEstimatedTotal(vm);
        if (isInKind && estimatedTotal.HasValue)
            vm.Amount = estimatedTotal;

        var entity = new Donation
        {
            Id = Guid.NewGuid(),
            DonorId = vm.DonorId,
            DonationNumber = vm.DonationNumber.Trim(),
            DonationDate = vm.DonationDate,
            DonationType = vm.DonationType,
            TargetingScopeCode = vm.TargetingScopeCode,
            AidTypeId = vm.AidTypeId,
            Amount = vm.Amount,
            PaymentMethodId = isInKind ? null : vm.PaymentMethodId,
            FinancialAccountId = isInKind ? null : vm.FinancialAccountId,
            IsRestricted = vm.IsRestricted,
            CampaignName = vm.CampaignName?.Trim(),
            GeneralPurposeName = vm.GeneralPurposeName?.Trim(),
            ReceiptNumber = vm.ReceiptNumber?.Trim(),
            ReferenceNumber = vm.ReferenceNumber?.Trim(),
            Notes = vm.Notes?.Trim(),
            CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            CreatedAtUtc = DateTime.UtcNow
        };

        if (!isInKind)
        {
            await _donationRepository.AddAsync(entity);

            var postingResult = await _journalHookService.TryCreateDonationEntryAsync(entity.Id);
            TempData[postingResult.IsSuccess ? "Success" : "Warning"] = postingResult.IsSuccess
                ? $"تم تسجيل التبرع بنجاح. {postingResult.Message}"
                : postingResult.Message;

            return RedirectToAction(nameof(Index), new { donorId = vm.DonorId });
        }

        await using var trx = await _db.Database.BeginTransactionAsync();
        try
        {
            _db.Set<Donation>().Add(entity);
            await _db.SaveChangesAsync();

            var inKindItem = new DonationInKindItem
            {
                Id = Guid.NewGuid(),
                DonationId = entity.Id,
                ItemId = vm.InKindItemId!.Value,
                Quantity = vm.InKindQuantity!.Value,
                EstimatedUnitValue = vm.InKindEstimatedUnitValue,
                EstimatedTotalValue = estimatedTotal,
                WarehouseId = vm.InKindWarehouseId,
                Notes = vm.InKindItemDescription?.Trim(),
                CreatedAtUtc = DateTime.UtcNow
            };

            _db.Set<DonationInKindItem>().Add(inKindItem);

            var receipt = new CharityStoreReceipt
            {
                Id = Guid.NewGuid(),
                ReceiptNumber = await GenerateDonationReceiptNumberAsync(),
                WarehouseId = vm.InKindWarehouseId!.Value,
                ReceiptDate = entity.DonationDate.Date,
                SourceType = DonationReceiptSourceType,
                SourceName = entity.DonationNumber,
                Notes = $"تبرع عيني وارد من التبرع رقم {entity.DonationNumber} | DonationInKindItemId:{inKindItem.Id}",
                CreatedAtUtc = DateTime.UtcNow,
                CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Lines =
                {
                    new CharityStoreReceiptLine
                    {
                        Id = Guid.NewGuid(),
                        ItemId = inKindItem.ItemId,
                        Quantity = inKindItem.Quantity,
                        UnitCost = inKindItem.EstimatedUnitValue ?? 0m,
                        Notes = inKindItem.Notes
                    }
                }
            };

            _db.Set<CharityStoreReceipt>().Add(receipt);
            await _db.SaveChangesAsync();

            await _stockService.AddPurchaseAsync(new StockOperationRequest
            {
                ItemId = inKindItem.ItemId,
                WarehouseId = vm.InKindWarehouseId.Value,
                Quantity = inKindItem.Quantity,
                UnitCost = inKindItem.EstimatedUnitValue ?? 0m,
                TransactionDateUtc = entity.DonationDate.Date,
                ReferenceType = nameof(CharityStoreReceipt),
                ReferenceNumber = receipt.ReceiptNumber,
                Notes = receipt.Notes
            });

            await trx.CommitAsync();

            var postingResult = await _journalHookService.TryCreateStoreReceiptEntryAsync(receipt.Id);
            TempData[postingResult.IsSuccess ? "Success" : "Warning"] = postingResult.IsSuccess
                ? $"تم تسجيل التبرع العيني وإضافة الصنف إلى المخزن عبر إذن إضافة رقم {receipt.ReceiptNumber}. {postingResult.Message}"
                : $"تم تسجيل التبرع العيني وإضافة الصنف إلى المخزن عبر إذن إضافة رقم {receipt.ReceiptNumber}. {postingResult.Message}";

            return RedirectToAction("Index", "DonationInKindItems", new { donationId = entity.Id });
        }
        catch (Exception ex)
        {
            await trx.RollbackAsync();
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(vm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _donationRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        if (!await PopulateDonorAsync(entity.DonorId))
            return NotFound();

        var scope = ResolveTargetingScopeCode(entity);
        var vm = new EditDonationVm
        {
            Id = entity.Id,
            DonorId = entity.DonorId,
            DonationNumber = entity.DonationNumber,
            DonationDate = entity.DonationDate,
            DonationType = entity.DonationType,
            TargetingScopeCode = scope,
            AidTypeId = entity.AidTypeId,
            Amount = entity.Amount,
            PaymentMethodId = entity.PaymentMethodId,
            FinancialAccountId = entity.FinancialAccountId,
            IsRestricted = entity.IsRestricted,
            CampaignName = entity.CampaignName,
            GeneralPurposeName = string.IsNullOrWhiteSpace(entity.GeneralPurposeName) ? entity.CampaignName : entity.GeneralPurposeName,
            ReceiptNumber = entity.ReceiptNumber,
            ReferenceNumber = entity.ReferenceNumber,
            Notes = entity.Notes
        };

        await FillLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditDonationVm vm)
    {
        var entity = await _donationRepository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        if (!await PopulateDonorAsync(entity.DonorId))
            return NotFound();

        await FillLookupsAsync(vm);
        NormalizeScopeFields(vm);
        ValidateScopeRules(vm);

        if (!ModelState.IsValid)
            return View(vm);

        if (await _donationRepository.DonationNumberExistsAsync(vm.DonationNumber.Trim(), vm.Id))
        {
            ModelState.AddModelError(nameof(vm.DonationNumber), "رقم التبرع موجود بالفعل");
            return View(vm);
        }

        var isInKind = IsInKindDonation(vm.DonationType);
        entity.DonationNumber = vm.DonationNumber.Trim();
        entity.DonationDate = vm.DonationDate;
        entity.DonationType = vm.DonationType;
        entity.TargetingScopeCode = vm.TargetingScopeCode;
        entity.AidTypeId = vm.AidTypeId;
        entity.Amount = vm.Amount;
        entity.PaymentMethodId = isInKind ? null : vm.PaymentMethodId;
        entity.FinancialAccountId = isInKind ? null : vm.FinancialAccountId;
        entity.IsRestricted = vm.IsRestricted;
        entity.CampaignName = vm.CampaignName?.Trim();
        entity.GeneralPurposeName = vm.GeneralPurposeName?.Trim();
        entity.ReceiptNumber = vm.ReceiptNumber?.Trim();
        entity.ReferenceNumber = vm.ReferenceNumber?.Trim();
        entity.Notes = vm.Notes?.Trim();
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _donationRepository.UpdateAsync(entity);

        TempData["Success"] = "تم تعديل التبرع بنجاح";
        return RedirectToAction(nameof(Index), new { donorId = entity.DonorId });
    }
    [HttpGet]
    public async Task<IActionResult> PrintReceipt(Guid id)
    {
        var donation = await _db.Set<Donation>()
            .AsNoTracking()
            .Include(x => x.Donor)
            .Include(x => x.AidType)
            .Include(x => x.PaymentMethod)
            .Include(x => x.FinancialAccount)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (donation == null)
            return NotFound();

        if (!await PopulateDonorAsync(donation.DonorId))
            return NotFound();
        var inKindItems = await _db.Set<DonationInKindItem>()
                .AsNoTracking()
                .Include(x => x.Item)
                .Include(x => x.Warehouse)
                .Where(x => x.DonationId == id)
                .OrderBy(x => x.CreatedAtUtc)
                .ToListAsync();

        var model = new DonationReceiptPrintVm
        {
            DonationId = donation.Id,
            DonationNumber = donation.DonationNumber,
            ReceiptNumber = donation.ReceiptNumber,
            DonationDate = donation.DonationDate,
            DonationType = donation.DonationType,
            AidTypeName = donation.AidType?.NameAr,
            Amount = donation.Amount,
            PaymentMethodName = donation.PaymentMethod?.MethodNameAr,
            FinancialAccountName = donation.FinancialAccount?.AccountNameAr,
            IsRestricted = donation.IsRestricted,
            CampaignName = donation.CampaignName,
            ReferenceNumber = donation.ReferenceNumber,
            Notes = donation.Notes,
            DonorCode = donation.Donor?.Code,
            DonorType = donation.Donor?.DonorType,
            DonorName = donation.Donor?.FullName ?? "-",
            ContactPerson = donation.Donor?.ContactPerson,
            DonorPhoneNumber = donation.Donor?.PhoneNumber,
            DonorAddress = donation.Donor?.AddressLine,
            InKindItems = inKindItems.Select(x => new DonationReceiptInKindItemVm
            {
                ItemName = x.Item?.ItemNameAr ?? "-",
                WarehouseName = x.Warehouse?.WarehouseNameAr,
                Quantity = x.Quantity,
                EstimatedUnitValue = x.EstimatedUnitValue,
                EstimatedTotalValue = x.EstimatedTotalValue,
                ExpiryDate = x.ExpiryDate,
                BatchNo = x.BatchNo,
                Notes = x.Notes
            }).ToList()
        };
        return View(model);
    }
    [HttpGet]
    public async Task<IActionResult> Tracking(Guid id)
    {
        var donation = await _db.Set<Donation>()
            .AsNoTracking()
            .Include(x => x.Donor)
            .Include(x => x.AidType)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (donation == null)
            return NotFound();

        if (!await PopulateDonorAsync(donation.DonorId))
            return NotFound();

        if (!IsInKindDonation(donation.DonationType))
        {
            TempData["Warning"] = "التتبع المستندي متاح للتبرعات العينية فقط.";
            return RedirectToAction(nameof(Index), new { donorId = donation.DonorId });
        }

        var inKindItems = await _db.Set<DonationInKindItem>()
            .AsNoTracking()
            .Include(x => x.Item)
            .Include(x => x.Warehouse)
            .Where(x => x.DonationId == id)
            .OrderBy(x => x.CreatedAtUtc)
            .ToListAsync();

        var allocations = await _db.Set<DonationAllocation>()
            .AsNoTracking()
            .Where(x => x.DonationId == id && x.DonationInKindItemId.HasValue)
            .ToListAsync();

        var receipts = await _db.Set<CharityStoreReceipt>()
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .Include(x => x.Lines)
            .Where(x => x.SourceType == DonationReceiptSourceType && x.SourceName == donation.DonationNumber)
            .OrderBy(x => x.ReceiptDate)
            .ToListAsync();

        var issues = await FindDonationStoreIssuesAsync(allocations);

        var receiptDocuments = receipts.Select(x => new DonationInKindTrackingDocumentVm
        {
            Id = x.Id,
            Number = x.ReceiptNumber,
            DocumentDate = x.ReceiptDate,
            WarehouseName = x.Warehouse?.WarehouseNameAr,
            TotalQuantity = x.Lines.Sum(l => l.Quantity),
            Notes = x.Notes
        }).ToList();

        var issueDocuments = issues.Select(x => new DonationInKindTrackingDocumentVm
        {
            Id = x.Id,
            Number = x.IssueNumber,
            DocumentDate = x.IssueDate,
            WarehouseName = x.Warehouse?.WarehouseNameAr,
            BeneficiaryName = x.Beneficiary?.FullName,
            TotalQuantity = x.Lines.Sum(l => l.Quantity),
            Notes = x.Notes
        }).ToList();

        var itemRows = new List<DonationInKindTrackingItemVm>();
        foreach (var item in inKindItems)
        {
            var itemMarker = $"DonationInKindItemId:{item.Id}";
            var itemReceipts = receipts
                .Where(x => !string.IsNullOrWhiteSpace(x.Notes) && x.Notes!.Contains(itemMarker, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (itemReceipts.Count == 0)
            {
                itemReceipts = receipts
                    .Where(x => x.WarehouseId == item.WarehouseId && x.Lines.Any(l => l.ItemId == item.ItemId))
                    .ToList();
            }

            var itemAllocations = allocations
                .Where(x => x.DonationInKindItemId == item.Id)
                .ToList();

            var itemIssues = issues
                .Where(issue => itemAllocations.Any(a => !string.IsNullOrWhiteSpace(issue.Notes)
                    && issue.Notes!.Contains($"DonationAllocationId:{a.Id}", StringComparison.OrdinalIgnoreCase)))
                .ToList();

            var allocatedQty = itemAllocations.Sum(x => x.AllocatedQuantity ?? 0m);
            var receivedQty = itemReceipts.Sum(x => x.Lines.Sum(l => l.Quantity));
            var issuedQty = itemIssues.Sum(x => x.Lines.Sum(l => l.Quantity));
            var status = ResolveInKindTrackingStatus(item.Quantity, itemReceipts.Count, receivedQty, itemIssues.Count, issuedQty);

            itemRows.Add(new DonationInKindTrackingItemVm
            {
                DonationInKindItemId = item.Id,
                ItemName = item.Item?.ItemNameAr ?? "-",
                WarehouseName = item.Warehouse?.WarehouseNameAr,
                BatchNo = item.BatchNo,
                Quantity = item.Quantity,
                AllocatedQuantity = allocatedQty,
                RemainingAllocationQuantity = Math.Max(0m, item.Quantity - allocatedQty),
                StoreReceiptCount = itemReceipts.Count,
                StoreIssueCount = itemIssues.Count,
                ReceivedToStoreQuantity = receivedQty,
                IssuedQuantity = issuedQty,
                RemainingToIssueQuantity = Math.Max(0m, receivedQty - issuedQty),
                TrackingStatusCode = status.Code,
                TrackingStatusName = status.Name
            });
        }

        var totalQty = itemRows.Sum(x => x.Quantity);
        var receivedTotal = itemRows.Sum(x => x.ReceivedToStoreQuantity);
        var issuedTotal = itemRows.Sum(x => x.IssuedQuantity);
        var overallStatus = ResolveInKindTrackingStatus(totalQty, receiptDocuments.Count, receivedTotal, issueDocuments.Count, issuedTotal);

        var model = new DonationInKindTrackingVm
        {
            DonationId = donation.Id,
            DonationNumber = donation.DonationNumber,
            DonationDate = donation.DonationDate,
            DonationType = donation.DonationType,
            DonorName = donation.Donor?.FullName,
            AidTypeName = donation.AidType?.NameAr,
            DonationAmount = donation.Amount ?? 0m,
            ItemsCount = itemRows.Count,
            TotalQuantity = totalQty,
            StoreReceiptCount = receiptDocuments.Count,
            StoreIssueCount = issueDocuments.Count,
            ReceivedToStoreQuantity = receivedTotal,
            IssuedQuantity = issuedTotal,
            RemainingToIssueQuantity = Math.Max(0m, receivedTotal - issuedTotal),
            TrackingStatusCode = overallStatus.Code,
            TrackingStatusName = overallStatus.Name,
            Items = itemRows,
            StoreReceipts = receiptDocuments,
            StoreIssues = issueDocuments
        };

        return View(model);
    }
    [HttpGet]
    public async Task<IActionResult> PrintMovement(Guid id)
    {
        var donation = await _db.Set<Donation>()
            .AsNoTracking()
            .Include(x => x.Donor)
            .Include(x => x.AidType)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (donation == null)
            return NotFound();

        var inKindItems = await _db.Set<DonationInKindItem>()
            .AsNoTracking()
            .Include(x => x.Item)
            .Include(x => x.Warehouse)
            .Where(x => x.DonationId == id)
            .OrderBy(x => x.CreatedAtUtc)
            .ToListAsync();

        var allocations = await _db.Set<DonationAllocation>()
            .AsNoTracking()
            .Where(x => x.DonationId == id && x.DonationInKindItemId.HasValue)
            .ToListAsync();

        var receipts = await _db.Set<CharityStoreReceipt>()
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .Include(x => x.Lines)
            .Where(x => x.SourceType == DonationReceiptSourceType && x.SourceName == donation.DonationNumber)
            .OrderBy(x => x.ReceiptDate)
            .ToListAsync();

        var issues = await FindDonationStoreIssuesAsync(allocations);

        var model = new DonationInKindMovementPrintVm
        {
            Id = donation.Id,
            DonationNumber = donation.DonationNumber,
            ReceiptNumber = donation.ReceiptNumber,
            DonationDate = donation.DonationDate,
            DonationType = donation.DonationType,
            AidTypeName = donation.AidType?.NameAr,
            DonorName = donation.Donor?.FullName,
            DonorCode = donation.Donor?.Code,
            DonorPhone = donation.Donor?.PhoneNumber,
            Notes = donation.Notes,
            Items = inKindItems.Select(x => new DonationInKindMovementItemPrintVm
            {
                DonationInKindItemId = x.Id,
                ItemId = x.ItemId,
                ItemCode = x.Item?.ItemCode ?? string.Empty,
                ItemName = x.Item?.ItemNameAr ?? "-",
                WarehouseName = x.Warehouse?.WarehouseNameAr,
                Quantity = x.Quantity,
                ReceivedQuantity = receipts
                    .Where(r => !string.IsNullOrWhiteSpace(r.Notes) && r.Notes!.Contains($"DonationInKindItemId:{x.Id}", StringComparison.OrdinalIgnoreCase))
                    .Sum(r => r.Lines.Sum(l => l.Quantity)),
                EstimatedUnitValue = x.EstimatedUnitValue,
                EstimatedTotalValue = x.EstimatedTotalValue,
                ExpiryDate = x.ExpiryDate,
                BatchNo = x.BatchNo,
                Notes = x.Notes
            }).ToList(),
            StoreReceipts = receipts.Select(x => new DonationInKindMovementStoreReceiptPrintVm
            {
                Id = x.Id,
                ReceiptNumber = x.ReceiptNumber,
                ReceiptDate = x.ReceiptDate,
                WarehouseName = x.Warehouse?.WarehouseNameAr,
                SourceType = x.SourceType,
                SourceName = x.SourceName,
                LinesCount = x.Lines.Count,
                TotalAmount = x.Lines.Sum(l => l.Quantity * l.UnitCost)
            }).ToList(),
            StoreIssues = issues.Select(x => new DonationInKindMovementStoreIssuePrintVm
            {
                Id = x.Id,
                IssueNumber = x.IssueNumber,
                IssueDate = x.IssueDate,
                WarehouseName = x.Warehouse?.WarehouseNameAr,
                IssueType = x.IssueType,
                BeneficiaryName = x.Beneficiary?.FullName,
                ProjectName = x.Project?.Name,
                IssuedToName = x.IssuedToName,
                LinesCount = x.Lines.Count,
                TotalAmount = x.Lines.Sum(l => l.Quantity * l.UnitCost)
            }).ToList()
        };

        return View(model);
    }

    private async Task<List<CharityStoreIssue>> FindDonationStoreIssuesAsync(List<DonationAllocation> allocations)
    {
        if (allocations.Count == 0)
            return new List<CharityStoreIssue>();

        var markers = allocations
            .Select(x => $"DonationAllocationId:{x.Id}")
            .ToList();

        var candidates = await _db.Set<CharityStoreIssue>()
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .Include(x => x.Beneficiary)
            .Include(x => x.Project)
            .Include(x => x.Lines)
            .Where(x => x.Notes != null && x.Notes.Contains("DonationAllocationId:"))
            .OrderBy(x => x.IssueDate)
            .ToListAsync();

        return candidates
            .Where(x => markers.Any(m => x.Notes!.Contains(m, StringComparison.OrdinalIgnoreCase)))
            .ToList();
    }

    private static (string Code, string Name) ResolveInKindTrackingStatus(decimal totalQty, int receiptCount, decimal receivedQty, int issueCount, decimal issuedQty)
    {
        if (receiptCount <= 0 || receivedQty <= 0)
            return ("NotReceivedToStore", "لم يدخل المخزن");

        if (issueCount <= 0 || issuedQty <= 0)
            return ("ReceivedToStore", "دخل المخزن");

        if (issuedQty < receivedQty)
            return ("PartiallyIssued", "صُرف جزئيًا");

        return ("FullyIssued", "صُرف بالكامل");
    }

    private async Task<bool> PopulateDonorAsync(Guid donorId)
    {
        var donor = await _donorRepository.GetByIdAsync(donorId);
        if (donor == null)
            return false;

        ViewBag.DonorHeader = new DonorHeaderVm
        {
            Id = donor.Id,
            Code = donor.Code,
            DonorType = donor.DonorType,
            FullName = donor.FullName,
            PhoneNumber = donor.PhoneNumber,
            ContactPerson = donor.ContactPerson,
            IsActive = donor.IsActive
        };

        return true;
    }

    private async Task FillLookupsAsync(CreateDonationVm vm)
    {
        var aidTypes = await _db.Set<AidTypeLookup>()
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.NameAr)
            .ToListAsync();

        var paymentMethods = await _db.Set<PaymentMethod>()
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.MethodNameAr)
            .ToListAsync();

        var accounts = await _db.Set<FinancialAccount>()
            .AsNoTracking()
            .Where(x => x.IsActive && x.IsPosting)
            .OrderBy(x => x.AccountCode)
            .ThenBy(x => x.AccountNameAr)
            .ToListAsync();

        var items = await _db.Set<Item>()
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.ItemNameAr)
            .ToListAsync();

        var warehouses = await _db.Set<Warehouse>()
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.WarehouseNameAr)
            .ToListAsync();

        vm.DonationTypes = DonationTypeOption.Values.Select(x => new SelectListItem { Value = x, Text = x }).ToList();
        vm.TargetingScopes = InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations.DonationTargetingScopeOption.Values
            .Select(x => new SelectListItem
            {
                Value = x,
                Text = InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations.DonationTargetingScopeOption.GetArabicName(x),
                Selected = string.Equals(vm.TargetingScopeCode, x, StringComparison.OrdinalIgnoreCase)
            }).ToList();
        vm.AidTypes = aidTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.NameAr }).ToList();
        vm.PaymentMethods = paymentMethods.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.MethodNameAr }).ToList();
        vm.FinancialAccounts = accounts.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = $"{x.AccountCode} - {x.AccountNameAr}" }).ToList();
        vm.Items = items.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = $"{x.ItemCode} - {x.ItemNameAr}" }).ToList();
        vm.Warehouses = warehouses.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.WarehouseNameAr }).ToList();
    }

    private void NormalizeScopeFields(CreateDonationVm vm)
    {
        if (IsInKindDonation(vm.DonationType))
        {
            vm.TargetingScopeCode = InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations.DonationTargetingScopeOption.GeneralFund;
            vm.IsRestricted = false;
            vm.PaymentMethodId = null;
            vm.FinancialAccountId = null;
            vm.GeneralPurposeName = null;
            vm.CampaignName = null;
            vm.Amount = ResolveInKindEstimatedTotal(vm);
            return;
        }

        vm.TargetingScopeCode = string.IsNullOrWhiteSpace(vm.TargetingScopeCode)
            ? InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations.DonationTargetingScopeOption.SpecificRequests
            : vm.TargetingScopeCode.Trim();

        switch (vm.TargetingScopeCode)
        {
            case InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations.DonationTargetingScopeOption.GeneralFund:
                vm.IsRestricted = false;
                vm.GeneralPurposeName = null;
                vm.CampaignName = null;
                vm.AidTypeId = null;
                break;

            case InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations.DonationTargetingScopeOption.GeneralPurpose:
                vm.IsRestricted = false;
                vm.GeneralPurposeName = string.IsNullOrWhiteSpace(vm.GeneralPurposeName)
                    ? vm.CampaignName?.Trim()
                    : vm.GeneralPurposeName.Trim();
                vm.CampaignName = vm.GeneralPurposeName;
                break;

            default:
                vm.TargetingScopeCode = InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations.DonationTargetingScopeOption.SpecificRequests;
                vm.IsRestricted = true;
                vm.CampaignName = null;
                vm.GeneralPurposeName = null;
                break;
        }
    }

    private void ValidateScopeRules(CreateDonationVm vm)
    {
        if (IsInKindDonation(vm.DonationType))
        {
            if (vm is EditDonationVm)
                return;

            if (!vm.InKindItemId.HasValue || vm.InKindItemId == Guid.Empty)
                ModelState.AddModelError(nameof(vm.InKindItemId), "الصنف العيني مطلوب عند تسجيل تبرع عيني.");

            if (!vm.InKindQuantity.HasValue || vm.InKindQuantity <= 0)
                ModelState.AddModelError(nameof(vm.InKindQuantity), "كمية الصنف العيني مطلوبة ويجب أن تكون أكبر من صفر.");

            if (string.IsNullOrWhiteSpace(vm.InKindItemDescription))
                ModelState.AddModelError(nameof(vm.InKindItemDescription), "وصف الصنف العيني مطلوب.");

            if (!vm.InKindWarehouseId.HasValue || vm.InKindWarehouseId == Guid.Empty)
                ModelState.AddModelError(nameof(vm.InKindWarehouseId), "المخزن مطلوب لإضافة التبرع العيني إلى المخزون.");

            return;
        }

        switch (vm.TargetingScopeCode)
        {
            case InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations.DonationTargetingScopeOption.SpecificRequests:
                if (!vm.AidTypeId.HasValue || vm.AidTypeId == Guid.Empty)
                    ModelState.AddModelError(nameof(vm.AidTypeId), "نوع المساعدة المستهدف مطلوب للتبرع المخصص لطلبات معينة.");
                break;

            case InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations.DonationTargetingScopeOption.GeneralPurpose:
                if (string.IsNullOrWhiteSpace(vm.GeneralPurposeName))
                    ModelState.AddModelError(nameof(vm.GeneralPurposeName), "اسم الغرض / الباب العام مطلوب لهذا النوع من التبرعات.");
                break;
        }
    }

    private static string ResolveTargetingScopeCode(Donation donation)
    {
        if (IsInKindDonation(donation.DonationType))
            return InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations.DonationTargetingScopeOption.GeneralFund;

        if (!string.IsNullOrWhiteSpace(donation.TargetingScopeCode))
            return donation.TargetingScopeCode;

        // Temporary fallback for legacy rows only.
        if (donation.IsRestricted)
            return InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations.DonationTargetingScopeOption.SpecificRequests;

        return string.IsNullOrWhiteSpace(donation.CampaignName)
            ? InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations.DonationTargetingScopeOption.GeneralFund
            : InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations.DonationTargetingScopeOption.GeneralPurpose;
    }

    private static bool IsInKindDonation(string? donationType)
    {
        return string.Equals(donationType?.Trim(), "عيني", StringComparison.OrdinalIgnoreCase);
    }

    private static decimal? ResolveInKindEstimatedTotal(CreateDonationVm vm)
    {
        if (vm.InKindEstimatedTotalValue.HasValue && vm.InKindEstimatedTotalValue.Value > 0)
            return vm.InKindEstimatedTotalValue.Value;

        if (vm.InKindQuantity.HasValue && vm.InKindEstimatedUnitValue.HasValue)
            return vm.InKindQuantity.Value * vm.InKindEstimatedUnitValue.Value;

        return vm.Amount;
    }

    private async Task<string> GenerateDonationNumberAsync()
    {
        var latestNumber = await _db.Set<Donation>()
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => x.DonationNumber)
            .FirstOrDefaultAsync();

        var next = 1;
        if (!string.IsNullOrWhiteSpace(latestNumber))
        {
            var digits = new string(latestNumber.Where(char.IsDigit).ToArray());
            if (int.TryParse(digits, out var parsed))
                next = parsed + 1;
        }

        return $"DONA-{next:00000}";
    }

    private async Task<string> GenerateDonationReceiptNumberAsync()
    {
        var latest = await _db.Set<CharityStoreReceipt>()
            .AsNoTracking()
            .Where(x => x.SourceType == DonationReceiptSourceType)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => x.ReceiptNumber)
            .FirstOrDefaultAsync();

        var next = 1;
        if (!string.IsNullOrWhiteSpace(latest))
        {
            var parts = latest.Split('-');
            if (parts.Length > 0 && int.TryParse(parts[^1], out var parsed))
                next = parsed + 1;
        }

        return $"DON-RCV-{next:0000}";
    }
}
