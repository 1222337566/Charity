using System.Security.Claims;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using InfrastrfuctureManagmentCore.Domains.Charity.StockAdvanced;
using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using InfrastrfuctureManagmentCore.Domains.Warehouses;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Stores;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentServices.Charity.Beneficiaries;
using InfrastructureManagmentServices.Stock;
using InfrastructureManagmentWebFramework.Models.Charity.Stores.Issues;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Skote.Helpers;

[Authorize(Policy = CharityPolicies.StoresView)]
public class CharityStoreIssuesController : Controller
{
    private readonly ICharityStoreIssueRepository _issueRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IBeneficiaryRepository _beneficiaryRepository;
    private readonly ICharityProjectRepository _projectRepository;
    private readonly IStockService _stockService;
    private readonly IOperationalJournalHookService _journalHookService;
    private readonly AppDbContext _db;

    public CharityStoreIssuesController(
        ICharityStoreIssueRepository issueRepository,
        IWarehouseRepository warehouseRepository,
        IItemRepository itemRepository,
        IBeneficiaryRepository beneficiaryRepository,
        ICharityProjectRepository projectRepository,
        IStockService stockService,
        IOperationalJournalHookService journalHookService,
        AppDbContext db)
    {
        _issueRepository = issueRepository;
        _warehouseRepository = warehouseRepository;
        _itemRepository = itemRepository;
        _beneficiaryRepository = beneficiaryRepository;
        _projectRepository = projectRepository;
        _stockService = stockService;
        _journalHookService = journalHookService;
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _issueRepository.GetAllAsync();
        return View(items.Select(x => new StoreIssueListItemVm
        {
            Id = x.Id,
            IssueNumber = x.IssueNumber,
            IssueDate = x.IssueDate,
            WarehouseName = x.Warehouse?.WarehouseNameAr ?? string.Empty,
            IssueType = x.IssueType,
            BeneficiaryName = x.Beneficiary?.FullName,
            ProjectName = x.Project?.Name,
            IssuedToName = x.IssuedToName,
            LinesCount = x.Lines.Count,
            TotalAmount = x.Lines.Sum(l => l.Quantity * l.UnitCost),
            ApprovalStatus = x.ApprovalStatus,
            ApprovalStatusName = ToApprovalStatusName(x.ApprovalStatus),
            ApprovedByUserId = x.ApprovedByUserId,
            ApprovedAtUtc = x.ApprovedAtUtc,
            RejectedAtUtc = x.RejectedAtUtc,
            ApprovalNotes = x.ApprovalNotes
        }).ToList());
    }

    [Authorize(Policy = CharityPolicies.StoresManage)]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new CreateStoreIssueVm { IssueDate = DateTime.Today };
        await FillLookupsAsync(vm);
        return View(vm);
    }

    [Authorize(Policy = CharityPolicies.StoresManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateStoreIssueVm vm)
    {
        await FillLookupsAsync(vm);

        if (!ModelState.IsValid)
            return View(vm);

        if (vm.IssueType == "Beneficiary" && !vm.BeneficiaryId.HasValue)
            ModelState.AddModelError(nameof(vm.BeneficiaryId), "اختر المستفيد عند الصرف لمستفيد");

        if (vm.IssueType == "Project" && !vm.ProjectId.HasValue)
            ModelState.AddModelError(nameof(vm.ProjectId), "اختر المشروع عند الصرف لمشروع");

        if (!ModelState.IsValid)
            return View(vm);

        if (await _issueRepository.NumberExistsAsync(vm.IssueNumber.Trim()))
        {
            ModelState.AddModelError(nameof(vm.IssueNumber), "رقم الإذن موجود بالفعل");
            return View(vm);
        }

        var entity = new CharityStoreIssue
        {
            Id = Guid.NewGuid(),
            IssueNumber = vm.IssueNumber.Trim(),
            WarehouseId = vm.WarehouseId,
            IssueDate = vm.IssueDate,
            IssueType = vm.IssueType,
            BeneficiaryId = vm.BeneficiaryId,
            ProjectId = vm.ProjectId,
            IssuedToName = vm.IssuedToName?.Trim(),
            Notes = vm.Notes?.Trim(),
            ApprovalStatus = "Pending",
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            Lines =
            {
                new CharityStoreIssueLine
                {
                    Id = Guid.NewGuid(),
                    ItemId = vm.ItemId,
                    Quantity = vm.Quantity,
                    UnitCost = vm.UnitCost,
                    Notes = vm.Notes?.Trim()
                }
            }
        };

        await _issueRepository.AddAsync(entity);
        TempData["Success"] = "تم حفظ إذن الصرف المخزني في حالة انتظار الاعتماد.";
        return RedirectToAction(nameof(Details), new { id = entity.Id });
    }

    [Authorize(Policy = CharityPolicies.StoresManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateFromAidDisbursement(Guid disbursementId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var disbursement = await _db.Set<BeneficiaryAidDisbursement>()
            .Include(x => x.Beneficiary)
            .Include(x => x.AidRequest)
            .Include(x => x.FundingLines)
                .ThenInclude(x => x.DonationAllocation)
                    .ThenInclude(x => x.DonationInKindItem)
                        .ThenInclude(x => x.Item)
            .Include(x => x.FundingLines)
                .ThenInclude(x => x.DonationAllocation)
                    .ThenInclude(x => x.Donation)
            .FirstOrDefaultAsync(x => x.Id == disbursementId);

        if (disbursement == null)
            return NotFound();

        if (disbursement.StoreIssueId.HasValue)
        {
            TempData["Warning"] = "يوجد إذن صرف مخزني مرتبط بهذا السجل بالفعل.";
            return RedirectToAction(nameof(Details), new { id = disbursement.StoreIssueId.Value });
        }

        if (!string.Equals(disbursement.ApprovalStatus, "Approved", StringComparison.OrdinalIgnoreCase))
        {
            TempData["Warning"] = "لا يمكن إنشاء إذن صرف مخزني إلا من سجل صرف معتمد ومتاح.";
            return RedirectToAction("Index", "BeneficiaryAidDisbursements", new { beneficiaryId = disbursement.BeneficiaryId });
        }

        if (string.Equals(disbursement.ExecutionStatus, "FullyDisbursed", StringComparison.OrdinalIgnoreCase)
            || string.Equals(disbursement.ExecutionStatus, "Cancelled", StringComparison.OrdinalIgnoreCase))
        {
            TempData["Warning"] = "لا يمكن إنشاء إذن صرف مخزني لسجل تم تنفيذه أو إلغاؤه.";
            return RedirectToAction("Index", "BeneficiaryAidDisbursements", new { beneficiaryId = disbursement.BeneficiaryId });
        }
        if (string.Equals(disbursement.SourceType, "PurchaseNeedRequest", StringComparison.OrdinalIgnoreCase))
            return await CreateIssueFromPurchaseNeedDisbursementAsync(disbursement, currentUserId);

        var fundingLine = disbursement.FundingLines
            .Where(x => x.DonationAllocation?.DonationInKindItem != null)
            .OrderBy(x => x.CreatedAtUtc)
            .FirstOrDefault();

        var allocation = fundingLine?.DonationAllocation;
        var inKindItem = allocation?.DonationInKindItem;
        if (allocation == null || inKindItem == null)
        {
            TempData["Warning"] = "هذا السجل لا يحتوي على تخصيص عيني يمكن إنشاء إذن صرف مخزني منه.";
            return RedirectToAction("Index", "BeneficiaryAidDisbursements", new { beneficiaryId = disbursement.BeneficiaryId });
        }

        if (!inKindItem.WarehouseId.HasValue || inKindItem.WarehouseId.Value == Guid.Empty)
        {
            TempData["Warning"] = "لا يمكن إنشاء إذن الصرف لأن الصنف العيني غير مرتبط بمخزن.";
            return RedirectToAction("Index", "BeneficiaryAidDisbursements", new { beneficiaryId = disbursement.BeneficiaryId });
        }

        var quantity = allocation.AllocatedQuantity ?? 0m;
        if (quantity <= 0m)
        {
            TempData["Warning"] = "لا يمكن إنشاء إذن صرف بدون كمية مخصصة.";
            return RedirectToAction("Index", "BeneficiaryAidDisbursements", new { beneficiaryId = disbursement.BeneficiaryId });
        }

        var unitCost = inKindItem.EstimatedUnitValue ?? 0m;
        var issue = new CharityStoreIssue
        {
            Id = Guid.NewGuid(),
            IssueNumber = await GenerateAutoStoreIssueNumberAsync(),
            WarehouseId = inKindItem.WarehouseId.Value,
            IssueDate = DateTime.Today,
            IssueType = "Beneficiary",
            BeneficiaryId = disbursement.BeneficiaryId,
            ProjectId = disbursement.ProjectId,
            IssuedToName = disbursement.Beneficiary?.FullName,
            ApprovalStatus = "Pending",
            Notes = $"إذن صرف مخزني بناءً على سجل صرف عيني متاح. DisbursementId:{disbursement.Id} | DonationAllocationId:{allocation.Id} | DonationId:{allocation.DonationId}",
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByUserId = currentUserId,
            Lines =
            {
                new CharityStoreIssueLine
                {
                    Id = Guid.NewGuid(),
                    ItemId = inKindItem.ItemId,
                    Quantity = quantity,
                    UnitCost = unitCost,
                    Notes = $"صرف عيني للمستفيد بناءً على تخصيص التبرع رقم {allocation.Donation?.DonationNumber ?? allocation.DonationId.ToString()}."
                }
            }
        };

        await _db.Set<CharityStoreIssue>().AddAsync(issue);
        disbursement.StoreIssueId = issue.Id;
        disbursement.ExecutionStatus = string.IsNullOrWhiteSpace(disbursement.ExecutionStatus) ? "Available" : disbursement.ExecutionStatus;
        disbursement.Notes = string.IsNullOrWhiteSpace(disbursement.Notes)
            ? $"تم إنشاء إذن صرف مخزني رقم {issue.IssueNumber} من سجل الصرف."
            : $"{disbursement.Notes}\nتم إنشاء إذن صرف مخزني رقم {issue.IssueNumber} من سجل الصرف.";

        await _db.SaveChangesAsync();
        TempData["Success"] = "تم إنشاء إذن صرف مخزني في حالة انتظار الاعتماد، ويمكن طباعته من صفحة التفاصيل.";
        return RedirectToAction(nameof(Details), new { id = issue.Id });
    }


    public async Task<IActionResult> Details(Guid id)
    {
        var entity = await _issueRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var vm = new StoreIssueDetailsVm
        {
            Id = entity.Id,
            IssueNumber = entity.IssueNumber,
            IssueDate = entity.IssueDate,
            WarehouseName = entity.Warehouse?.WarehouseNameAr ?? string.Empty,
            IssueType = entity.IssueType,
            BeneficiaryName = entity.Beneficiary?.FullName,
            ProjectName = entity.Project?.Name,
            IssuedToName = entity.IssuedToName,
            Notes = entity.Notes,
            ApprovalStatus = entity.ApprovalStatus,
            ApprovalStatusName = ToApprovalStatusName(entity.ApprovalStatus),
            ApprovedByUserId = entity.ApprovedByUserId,
            ApprovedAtUtc = entity.ApprovedAtUtc,
            RejectedByUserId = entity.RejectedByUserId,
            RejectedAtUtc = entity.RejectedAtUtc,
            ApprovalNotes = entity.ApprovalNotes,
            CanApprove = string.Equals(entity.ApprovalStatus, "Pending", StringComparison.OrdinalIgnoreCase),
            CanReject = string.Equals(entity.ApprovalStatus, "Pending", StringComparison.OrdinalIgnoreCase),
            Lines = entity.Lines.Select(x => new StoreIssueLineVm
            {
                ItemCode = x.Item?.ItemCode ?? string.Empty,
                ItemName = x.Item?.ItemNameAr ?? string.Empty,
                Quantity = x.Quantity,
                UnitCost = x.UnitCost,
                Notes = x.Notes
            }).ToList()
        };

        return View(vm);
    }

    [Authorize(Policy = CharityPolicies.StoresApprove)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(Guid id, string? approvalNotes)
    {
        var entity = await _db.Set<CharityStoreIssue>()
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
            return NotFound();

        if (!string.Equals(entity.ApprovalStatus, "Pending", StringComparison.OrdinalIgnoreCase))
        {
            TempData["Warning"] = "لا يمكن اعتماد هذا الإذن لأن حالته الحالية ليست انتظار الاعتماد.";
            return RedirectToAction(nameof(Details), new { id });
        }

        await using var trx = await _db.Database.BeginTransactionAsync();
        try
        {
            foreach (var line in entity.Lines)
            {
                await _stockService.AddSaleAsync(new StockOperationRequest
                {
                    ItemId = line.ItemId,
                    WarehouseId = entity.WarehouseId,
                    Quantity = line.Quantity,
                    UnitCost = line.UnitCost,
                    TransactionDateUtc = entity.IssueDate,
                    ReferenceType = nameof(CharityStoreIssue),
                    ReferenceNumber = entity.IssueNumber,
                    Notes = line.Notes ?? entity.Notes
                });
            }

            entity.ApprovalStatus = "Approved";
            entity.ApprovedAtUtc = DateTime.UtcNow;
            entity.ApprovedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            entity.RejectedAtUtc = null;
            entity.RejectedByUserId = null;
            entity.ApprovalNotes = string.IsNullOrWhiteSpace(approvalNotes) ? null : approvalNotes.Trim();

            await AddLinkedBeneficiaryDisbursementIfNeededAsync(entity, User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _db.SaveChangesAsync();

            await trx.CommitAsync();
        }
        catch (Exception ex)
        {
            await trx.RollbackAsync();
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Details), new { id });
        }

        var postingResult = await _journalHookService.TryCreateStoreIssueEntryAsync(entity.Id);
        TempData[postingResult.IsSuccess ? "Success" : "Warning"] = postingResult.IsSuccess
            ? $"تم اعتماد إذن الصرف بنجاح. {postingResult.Message}"
            : $"تم اعتماد إذن الصرف، لكن هناك ملاحظة محاسبية: {postingResult.Message}";

        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Policy = CharityPolicies.StoresApprove)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(Guid id, string? approvalNotes)
    {
        var entity = await _db.Set<CharityStoreIssue>().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return NotFound();

        if (!string.Equals(entity.ApprovalStatus, "Pending", StringComparison.OrdinalIgnoreCase))
        {
            TempData["Warning"] = "لا يمكن رفض هذا الإذن لأن حالته الحالية ليست انتظار الاعتماد.";
            return RedirectToAction(nameof(Details), new { id });
        }

        entity.ApprovalStatus = "Rejected";
        entity.RejectedAtUtc = DateTime.UtcNow;
        entity.RejectedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        entity.ApprovalNotes = string.IsNullOrWhiteSpace(approvalNotes) ? "تم رفض الإذن." : approvalNotes.Trim();
        await _db.SaveChangesAsync();

        TempData["Success"] = "تم رفض إذن الصرف المخزني.";
        return RedirectToAction(nameof(Details), new { id });
    }


    private async Task<IActionResult> CreateIssueFromPurchaseNeedDisbursementAsync(BeneficiaryAidDisbursement disbursement, string? currentUserId)
    {
        var needRequestId = disbursement.SourceId;
        if (!needRequestId.HasValue && TryExtractGuidMarker(disbursement.Notes, "StockNeedRequestId:", out var parsedNeedRequestId))
            needRequestId = parsedNeedRequestId;

        if (!needRequestId.HasValue || needRequestId.Value == Guid.Empty)
        {
            TempData["Warning"] = "سجل الصرف غير مرتبط بطلب احتياج شراء.";
            return RedirectToAction("Index", "BeneficiaryAidDisbursements", new { beneficiaryId = disbursement.BeneficiaryId });
        }

        var needRequest = await _db.Set<StockNeedRequest>()
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == needRequestId.Value);

        if (needRequest == null)
        {
            TempData["Warning"] = "تعذر العثور على طلب الاحتياج المرتبط بسجل الصرف.";
            return RedirectToAction("Index", "BeneficiaryAidDisbursements", new { beneficiaryId = disbursement.BeneficiaryId });
        }

        var receipt = await _db.Set<CharityStoreReceipt>()
            .Include(x => x.Lines)
            .Where(x => x.ApprovalStatus == "Approved"
                && x.SourceType == "PurchaseNeedRequest"
                && x.Notes != null
                && x.Notes.Contains($"StockNeedRequestId:{needRequest.Id}"))
            .OrderByDescending(x => x.ApprovedAtUtc ?? x.CreatedAtUtc)
            .FirstOrDefaultAsync();

        if (receipt == null)
        {
            TempData["Warning"] = "لا يوجد إذن إضافة مخزني معتمد مرتبط بطلب الاحتياج. اعتمد إذن الإضافة أولاً.";
            return RedirectToAction("Index", "BeneficiaryAidDisbursements", new { beneficiaryId = disbursement.BeneficiaryId });
        }

        var receiptLines = receipt.Lines
            .Where(x => x.ItemId != Guid.Empty && x.Quantity > 0m)
            .ToList();

        if (receiptLines.Count == 0)
        {
            TempData["Warning"] = "إذن الإضافة المعتمد لا يحتوي على أصناف صالحة للصرف.";
            return RedirectToAction("Index", "BeneficiaryAidDisbursements", new { beneficiaryId = disbursement.BeneficiaryId });
        }

        var issue = new CharityStoreIssue
        {
            Id = Guid.NewGuid(),
            IssueNumber = await GenerateAutoStoreIssueNumberAsync(),
            WarehouseId = receipt.WarehouseId,
            IssueDate = DateTime.Today,
            IssueType = "Beneficiary",
            BeneficiaryId = disbursement.BeneficiaryId,
            ProjectId = disbursement.ProjectId ?? disbursement.AidRequest?.ProjectId ?? needRequest.ProjectId,
            IssuedToName = disbursement.Beneficiary?.FullName,
            ApprovalStatus = "Pending",
            Notes = $"إذن صرف مخزني بناءً على طلب احتياج شراء. DisbursementId:{disbursement.Id} | StockNeedRequestId:{needRequest.Id} | StoreReceiptId:{receipt.Id} | BeneficiaryAidRequestId:{needRequest.BeneficiaryAidRequestId} | BeneficiaryAidRequestLineId:{needRequest.BeneficiaryAidRequestLineId}",
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByUserId = currentUserId,
            Lines = receiptLines.Select(line => new CharityStoreIssueLine
            {
                Id = Guid.NewGuid(),
                ItemId = line.ItemId,
                Quantity = line.Quantity,
                UnitCost = line.UnitCost,
                Notes = $"صرف للمستفيد من طلب الاحتياج {needRequest.RequestNumber}. StoreReceiptId:{receipt.Id} | StockNeedRequestId:{needRequest.Id}"
            }).ToList()
        };

        await _db.Set<CharityStoreIssue>().AddAsync(issue);

        disbursement.StoreIssueId = issue.Id;
        disbursement.ExecutionStatus = AidDisbursementExecutionStatusCodes.Available;
        disbursement.Notes = AppendNote(disbursement.Notes, $"تم إنشاء إذن صرف مخزني رقم {issue.IssueNumber} من طلب الاحتياج {needRequest.RequestNumber} وحالته انتظار الاعتماد.");

        await _db.SaveChangesAsync();

        TempData["Success"] = "تم إنشاء إذن الصرف المخزني من طلب الاحتياج. يمكنك مراجعته وطباعته ثم اعتماده.";
        return RedirectToAction(nameof(Details), new { id = issue.Id });
    }

    private async Task<string> GenerateAutoStoreIssueNumberAsync()
    {
        string issueNumber;
        do
        {
            issueNumber = $"SI-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}"[..31];
        }
        while (await _db.Set<CharityStoreIssue>().AsNoTracking().AnyAsync(x => x.IssueNumber == issueNumber));

        return issueNumber;
    }

    private async Task AddLinkedBeneficiaryDisbursementIfNeededAsync(CharityStoreIssue issue, string? currentUserId)
    {
        var firstLine = issue.Lines.FirstOrDefault();
        var quantity = firstLine?.Quantity ?? 0m;
        var unitCost = firstLine?.UnitCost ?? 0m;
        var executedAmount = unitCost > 0m && quantity > 0m ? quantity * unitCost : 0m;
        var now = DateTime.UtcNow;
        var existingDisbursements = await _db.Set<BeneficiaryAidDisbursement>()
    .Where(x => x.StoreIssueId == issue.Id)
    .ToListAsync();

        if (existingDisbursements.Count > 0)
        {
            var totalAmount = issue.Lines.Sum(x => x.Quantity * x.UnitCost);

            foreach (var disbursement1 in existingDisbursements)
            {
                disbursement1.ApprovalStatus = AidDisbursementApprovalStatusCodes.Approved;
                disbursement1.ApprovedAtUtc ??= DateTime.UtcNow;
                disbursement1.ApprovedByUserId ??= currentUserId;
                disbursement1.ExecutionStatus = AidDisbursementExecutionStatusCodes.FullyDisbursed;
                disbursement1.ExecutedAmount = totalAmount > 0m ? totalAmount : disbursement1.Amount;
                disbursement1.ExecutedAtUtc = DateTime.UtcNow;
                disbursement1.ExecutedByUserId = currentUserId;
                disbursement1.Notes = AppendNote(disbursement1.Notes, $"تم اعتماد إذن الصرف المخزني رقم {issue.IssueNumber} وتنفيذ الصرف العيني.");
            }

            return;
        }

        if (!TryExtractGuidMarker(issue.Notes, "DonationAllocationId:", out var allocationId))
            return;

        var allocation = await _db.Set<DonationAllocation>()
            .AsNoTracking()
            .Include(x => x.Donation)
            .Include(x => x.AidRequest)
            .Include(x => x.DonationInKindItem)
                .ThenInclude(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == allocationId);

        if (allocation == null || !allocation.BeneficiaryId.HasValue || !allocation.AidRequestId.HasValue)
            return;

        var aidRequest = allocation.AidRequest ?? await _db.Set<BeneficiaryAidRequest>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == allocation.AidRequestId.Value);

        if (aidRequest == null)
            return;

        if (quantity <= 0m)
            quantity = allocation.AllocatedQuantity ?? 0m;

        if (unitCost <= 0m)
            unitCost = allocation.DonationInKindItem?.EstimatedUnitValue ?? 0m;

        executedAmount = unitCost > 0m && quantity > 0m ? quantity * unitCost : 0m;

        var disbursement = new BeneficiaryAidDisbursement
        {
            Id = Guid.NewGuid(),
            BeneficiaryId = allocation.BeneficiaryId.Value,
            AidRequestId = allocation.AidRequestId.Value,
            AidTypeId = aidRequest.AidTypeId,
            DisbursementDate = issue.IssueDate,
            Amount = executedAmount > 0m ? executedAmount : null,
            StoreIssueId = issue.Id,
            DonationId = allocation.DonationId,
            ProjectId = aidRequest.ProjectId,
            ApprovalStatus = "Approved",
            ApprovedAtUtc = now,
            ApprovedByUserId = currentUserId,
            ExecutionStatus = "FullyDisbursed",
            ExecutedAmount = executedAmount > 0m ? executedAmount : null,
            ExecutedAtUtc = now,
            ExecutedByUserId = currentUserId,
            Notes = $"صرف عيني من المخزن بإذن رقم {issue.IssueNumber}{(allocation.DonationInKindItem?.Item != null ? $" للصنف {allocation.DonationInKindItem.Item.ItemNameAr}" : string.Empty)}{(quantity > 0m ? $" بكمية {quantity:N2}" : string.Empty)}.",
            CreatedByUserId = currentUserId ?? issue.CreatedByUserId,
            CreatedAtUtc = now
        };

        await _db.Set<BeneficiaryAidDisbursement>().AddAsync(disbursement);
    }


    private static bool TryExtractGuidMarker(string? text, string marker, out Guid value)
    {
        value = Guid.Empty;
        if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(marker))
            return false;

        var start = text.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (start < 0)
            return false;

        start += marker.Length;
        var remainder = text[start..].Trim();
        var token = remainder.Split(new[] { ' ', '|', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        return Guid.TryParse(token, out value);
    }

    private async Task FillLookupsAsync(CreateStoreIssueVm vm)
    {
        var warehouses = await _warehouseRepository.GetActiveAsync();
        var items = await _itemRepository.GetActiveAsync();
        var beneficiaries = await _beneficiaryRepository.SearchAsync(null, null, true);
        var projects = await _projectRepository.SearchAsync(null, null, true);

        vm.Warehouses = warehouses
            .OrderBy(x => x.WarehouseCode)
            .Select(x => new SelectListItem(x.WarehouseNameAr, x.Id.ToString()))
            .ToList();

        vm.Items = items
            .Where(x => x.IsStockItem && !x.IsService && x.IsActive)
            .OrderBy(x => x.ItemCode)
            .Select(x => new SelectListItem($"{x.ItemCode} - {x.ItemNameAr}", x.Id.ToString()))
            .ToList();

        vm.Beneficiaries = beneficiaries
            .OrderBy(x => x.Code)
            .Select(x => new SelectListItem($"{x.Code} - {x.FullName}", x.Id.ToString()))
            .ToList();

        vm.Projects = projects
            .OrderBy(x => x.Code)
            .Select(x => new SelectListItem($"{x.Code} - {x.Name}", x.Id.ToString()))
            .ToList();
    }
    private static string AppendNote(string? oldNotes, string newNote)
    {
        if (string.IsNullOrWhiteSpace(oldNotes))
            return newNote;

        if (oldNotes.Contains(newNote, StringComparison.OrdinalIgnoreCase))
            return oldNotes;

        return oldNotes.TrimEnd() + Environment.NewLine + newNote;
    }
    private static string ToApprovalStatusName(string? status) => status switch
    {
        "Approved" => "معتمد",
        "Rejected" => "مرفوض",
        "Pending" => "بانتظار الاعتماد",
        _ => "غير محدد"
    };
}
