using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using InfrastrfuctureManagmentCore.Domains.Item;
using InfrastrfuctureManagmentCore.Domains.Warehouses;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Donors;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Stores;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentServices.Stock;
using InfrastructureManagmentWebFramework.Models.Charity.DonorProfile;
using InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class DonationInKindItemsController : Controller
{
    private const string DonationReceiptSourceType = "DonationInKind";
    private readonly IDonationInKindItemRepository _repository;
    private readonly IDonationRepository _donationRepository;
    private readonly IDonorRepository _donorRepository;
    private readonly ICharityStoreReceiptRepository _storeReceiptRepository;
    private readonly IStockService _stockService;
    private readonly IOperationalJournalHookService _journalHookService;
    private readonly AppDbContext _db;

    public DonationInKindItemsController(
        IDonationInKindItemRepository repository,
        IDonationRepository donationRepository,
        IDonorRepository donorRepository,
        ICharityStoreReceiptRepository storeReceiptRepository,
        IStockService stockService,
        IOperationalJournalHookService journalHookService,
        AppDbContext db)
    {
        _repository = repository;
        _donationRepository = donationRepository;
        _donorRepository = donorRepository;
        _storeReceiptRepository = storeReceiptRepository;
        _stockService = stockService;
        _journalHookService = journalHookService;
        _db = db;
    }

    public async Task<IActionResult> Index(Guid donationId)
    {
        var donation = await _donationRepository.GetByIdAsync(donationId);
        if (donation == null)
            return NotFound();

        if (!await PopulateDonorAsync(donation.DonorId))
            return NotFound();

        ViewBag.Donation = donation;

        var items = await _repository.GetByDonationIdAsync(donationId);
        var receiptDocs = await _db.Set<CharityStoreReceipt>()
            .AsNoTracking()
            .Include(x => x.Lines)
            .Where(x => x.SourceType == DonationReceiptSourceType && x.SourceName == donation.DonationNumber)
            .ToListAsync();

        var allocations = await _db.Set<DonationAllocation>()
            .AsNoTracking()
            .Where(x => x.DonationId == donationId && x.DonationInKindItemId.HasValue)
            .ToListAsync();

        var allocationMarkerMap = allocations.ToDictionary(
            x => $"DonationAllocationId:{x.Id}",
            x => x,
            StringComparer.OrdinalIgnoreCase);

        var issueCandidates = allocationMarkerMap.Count == 0
            ? new List<CharityStoreIssue>()
            : await _db.Set<CharityStoreIssue>()
                .AsNoTracking()
                .Include(x => x.Lines)
                .Where(x => x.Notes != null && x.Notes.Contains("DonationAllocationId:"))
                .ToListAsync();

        var issueDocs = new List<CharityStoreIssue>();
        var issueIds = new HashSet<Guid>();
        var issueAllocationMap = new Dictionary<Guid, List<DonationAllocation>>();
        foreach (var issue in issueCandidates)
        {
            var matches = allocationMarkerMap
                .Where(x => !string.IsNullOrWhiteSpace(issue.Notes) && issue.Notes!.Contains(x.Key, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Value)
                .DistinctBy(x => x.Id)
                .ToList();

            if (matches.Count == 0)
                continue;

            if (issueIds.Add(issue.Id))
                issueDocs.Add(issue);

            issueAllocationMap[issue.Id] = matches;
        }

        var approvedReceiptDocs = receiptDocs
            .Where(x => string.Equals(x.ApprovalStatus ?? "Approved", "Approved", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var approvedIssueDocs = issueDocs
            .Where(x => string.Equals(x.ApprovalStatus ?? "Approved", "Approved", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var model = new List<DonationInKindItemListItemVm>();
        foreach (var item in items)
        {
            var allocated = await _repository.GetAllocatedQuantityAsync(item.Id);
            var itemReceipts = ResolveReceiptsForItem(item, approvedReceiptDocs);
            var itemIssues = approvedIssueDocs
                .Where(issue => issueAllocationMap.TryGetValue(issue.Id, out var matchedAllocations)
                    && matchedAllocations.Any(a => a.DonationInKindItemId == item.Id))
                .ToList();

            var receivedQty = itemReceipts.Sum(x => x.Lines.Sum(l => l.Quantity));
            var issuedQty = itemIssues.Sum(x => x.Lines.Sum(l => l.Quantity));
            var (statusCode, statusName) = ResolveInKindTrackingStatus(item.Quantity, itemReceipts.Count, receivedQty, itemIssues.Count, issuedQty);

            model.Add(new DonationInKindItemListItemVm
            {
                Id = item.Id,
                ItemName = item.Item?.ItemNameAr ?? "-",
                Quantity = item.Quantity,
                EstimatedUnitValue = item.EstimatedUnitValue,
                EstimatedTotalValue = item.EstimatedTotalValue,
                ExpiryDate = item.ExpiryDate,
                BatchNo = item.BatchNo,
                WarehouseName = item.Warehouse?.WarehouseNameAr,
                Notes = item.Notes,
                AllocatedQuantity = allocated,
                RemainingQuantity = item.Quantity - allocated,
                StoreReceiptCount = itemReceipts.Count,
                StoreIssueCount = itemIssues.Count,
                ReceivedToStoreQuantity = receivedQty,
                IssuedQuantity = issuedQty,
                RemainingToIssueQuantity = Math.Max(0m, receivedQty - issuedQty),
                TrackingStatusCode = statusCode,
                TrackingStatusName = statusName
            });
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid donationId)
    {
        var donation = await _donationRepository.GetByIdAsync(donationId);
        if (donation == null)
            return NotFound();

        if (!await PopulateDonorAsync(donation.DonorId))
            return NotFound();

        ViewBag.Donation = donation;

        var vm = new CreateDonationInKindItemVm
        {
            DonationId = donationId
        };

        await FillLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDonationInKindItemVm vm)
    {
        var donation = await _donationRepository.GetByIdAsync(vm.DonationId);
        if (donation == null)
            return NotFound();

        if (!await PopulateDonorAsync(donation.DonorId))
            return NotFound();

        ViewBag.Donation = donation;
        await FillLookupsAsync(vm);

        if (vm.WarehouseId == null || vm.WarehouseId == Guid.Empty)
            ModelState.AddModelError(nameof(vm.WarehouseId), "المخزن مطلوب لإضافة التبرع العيني إلى المخزون.");

        if (!ModelState.IsValid)
            return View(vm);

        await using var trx = await _db.Database.BeginTransactionAsync();
        try
        {
            var entity = new DonationInKindItem
            {
                Id = Guid.NewGuid(),
                DonationId = vm.DonationId,
                ItemId = vm.ItemId!.Value,
                Quantity = vm.Quantity,
                EstimatedUnitValue = vm.EstimatedUnitValue,
                EstimatedTotalValue = vm.EstimatedTotalValue ?? (vm.EstimatedUnitValue.HasValue ? vm.Quantity * vm.EstimatedUnitValue.Value : null),
                ExpiryDate = vm.ExpiryDate,
                BatchNo = vm.BatchNo?.Trim(),
                WarehouseId = vm.WarehouseId,
                Notes = vm.Notes?.Trim(),
                CreatedAtUtc = DateTime.UtcNow
            };

            await _repository.AddAsync(entity);

            var receipt = new CharityStoreReceipt
            {
                Id = Guid.NewGuid(),
                ReceiptNumber = await GenerateDonationReceiptNumberAsync(),
                WarehouseId = vm.WarehouseId!.Value,
                ReceiptDate = donation.DonationDate.Date,
                SourceType = DonationReceiptSourceType,
                SourceName = donation.DonationNumber,
                Notes = $"تبرع عيني وارد من التبرع رقم {donation.DonationNumber} | DonationInKindItemId:{entity.Id}",
                CreatedAtUtc = DateTime.UtcNow,
                CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Lines =
                {
                    new CharityStoreReceiptLine
                    {
                        Id = Guid.NewGuid(),
                        ItemId = entity.ItemId,
                        Quantity = entity.Quantity,
                        UnitCost = entity.EstimatedUnitValue ?? 0m,
                        ExpiryDate = entity.ExpiryDate,
                        BatchNo = entity.BatchNo,
                        Notes = entity.Notes
                    }
                }
            };

            await _storeReceiptRepository.AddAsync(receipt);

            await _stockService.AddPurchaseAsync(new StockOperationRequest
            {
                ItemId = entity.ItemId,
                WarehouseId = vm.WarehouseId.Value,
                Quantity = entity.Quantity,
                UnitCost = entity.EstimatedUnitValue ?? 0m,
                TransactionDateUtc = donation.DonationDate.Date,
                ReferenceType = nameof(CharityStoreReceipt),
                ReferenceNumber = receipt.ReceiptNumber,
                Notes = receipt.Notes
            });

            await trx.CommitAsync();

            var postingResult = await _journalHookService.TryCreateStoreReceiptEntryAsync(receipt.Id);
            TempData[postingResult.IsSuccess ? "Success" : "Warning"] = postingResult.IsSuccess
                ? $"تم إضافة الصنف العيني إلى المخزن بنجاح عبر إذن إضافة رقم {receipt.ReceiptNumber}. {postingResult.Message}"
                : $"تم إضافة الصنف العيني إلى المخزن بنجاح عبر إذن إضافة رقم {receipt.ReceiptNumber}. {postingResult.Message}";

            return RedirectToAction(nameof(Index), new { donationId = vm.DonationId });
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
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null || entity.Donation == null)
            return NotFound();

        if (!await PopulateDonorAsync(entity.Donation.DonorId))
            return NotFound();

        ViewBag.Donation = entity.Donation;

        var vm = new EditDonationInKindItemVm
        {
            Id = entity.Id,
            DonationId = entity.DonationId,
            ItemId = entity.ItemId,
            Quantity = entity.Quantity,
            EstimatedUnitValue = entity.EstimatedUnitValue,
            EstimatedTotalValue = entity.EstimatedTotalValue,
            ExpiryDate = entity.ExpiryDate,
            BatchNo = entity.BatchNo,
            WarehouseId = entity.WarehouseId,
            Notes = entity.Notes
        };

        await FillLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditDonationInKindItemVm vm)
    {
        var entity = await _repository.GetByIdAsync(vm.Id);
        if (entity == null || entity.Donation == null)
            return NotFound();

        if (!await PopulateDonorAsync(entity.Donation.DonorId))
            return NotFound();

        ViewBag.Donation = entity.Donation;
        await FillLookupsAsync(vm);

        if (!ModelState.IsValid)
            return View(vm);

        var allocated = await _repository.GetAllocatedQuantityAsync(vm.Id);
        if (vm.Quantity < allocated)
        {
            ModelState.AddModelError(nameof(vm.Quantity), $"الكمية لا يمكن أن تكون أقل من الكمية المخصصة بالفعل ({allocated:N2}).");
            return View(vm);
        }

        var receiptExists = await DonationReceiptExistsAsync(vm.Id);
        var materialChanged = entity.ItemId != vm.ItemId!.Value
                              || entity.Quantity != vm.Quantity
                              || entity.WarehouseId != vm.WarehouseId
                              || entity.EstimatedUnitValue != vm.EstimatedUnitValue;

        if (receiptExists && materialChanged)
        {
            ModelState.AddModelError(string.Empty, "تمت إضافة هذا التبرع العيني إلى المخزن بالفعل، لذلك لا يمكن تعديل الصنف أو الكمية أو المخزن أو القيمة الوحدية مباشرة. استخدم إذن مرتجع/تسوية مخزنية أولًا إذا أردت تغيير الأثر المخزني.");
            return View(vm);
        }

        entity.ItemId = vm.ItemId!.Value;
        entity.Quantity = vm.Quantity;
        entity.EstimatedUnitValue = vm.EstimatedUnitValue;
        entity.EstimatedTotalValue = vm.EstimatedTotalValue ?? (vm.EstimatedUnitValue.HasValue ? vm.Quantity * vm.EstimatedUnitValue.Value : null);
        entity.ExpiryDate = vm.ExpiryDate;
        entity.BatchNo = vm.BatchNo?.Trim();
        entity.WarehouseId = vm.WarehouseId;
        entity.Notes = vm.Notes?.Trim();
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(entity);
        TempData["Success"] = "تم تعديل الصنف العيني بنجاح";
        return RedirectToAction(nameof(Index), new { donationId = entity.DonationId });
    }

    private static List<CharityStoreReceipt> ResolveReceiptsForItem(DonationInKindItem item, List<CharityStoreReceipt> receiptDocs)
    {
        var marker = $"DonationInKindItemId:{item.Id}";
        var exact = receiptDocs
            .Where(x => !string.IsNullOrWhiteSpace(x.Notes) && x.Notes!.Contains(marker, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (exact.Count > 0)
            return exact;

        return receiptDocs
            .Where(x => x.WarehouseId == item.WarehouseId
                && x.Lines.Any(l => l.ItemId == item.ItemId
                    && (string.IsNullOrWhiteSpace(item.BatchNo) || l.BatchNo == item.BatchNo)))
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

    private async Task FillLookupsAsync(CreateDonationInKindItemVm vm)
    {
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

        vm.Items = items.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = $"{x.ItemCode} - {x.ItemNameAr}" }).ToList();
        vm.Warehouses = warehouses.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.WarehouseNameAr }).ToList();
    }

    private async Task<bool> DonationReceiptExistsAsync(Guid donationInKindItemId)
    {
        var marker = $"DonationInKindItemId:{donationInKindItemId}";
        return await _db.Set<CharityStoreReceipt>()
            .AsNoTracking()
            .AnyAsync(x => x.SourceType == DonationReceiptSourceType && x.Notes != null && x.Notes.Contains(marker));
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
