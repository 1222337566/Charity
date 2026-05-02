using System.Security.Claims;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.StockAdvanced;
using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using InfrastrfuctureManagmentCore.Domains.Warehouses;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Stores;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentServices.Charity.Beneficiaries;
using InfrastructureManagmentServices.Stock;
using InfrastructureManagmentWebFramework.Models.Charity.Stores.Receipts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Skote.Helpers;

[Authorize(Policy = CharityPolicies.StoresView)]
public class CharityStoreReceiptsController : Controller
{
    private readonly ICharityStoreReceiptRepository _receiptRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IStockService _stockService;
    private readonly IOperationalJournalHookService _journalHookService;
    private readonly AppDbContext _db;

    public CharityStoreReceiptsController(
        ICharityStoreReceiptRepository receiptRepository,
        IWarehouseRepository warehouseRepository,
        IItemRepository itemRepository,
        IStockService stockService,
        IOperationalJournalHookService journalHookService,
        AppDbContext db)
    {
        _receiptRepository = receiptRepository;
        _warehouseRepository = warehouseRepository;
        _itemRepository = itemRepository;
        _stockService = stockService;
        _journalHookService = journalHookService;
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _receiptRepository.GetAllAsync();
        return View(items.Select(x => new StoreReceiptListItemVm
        {
            Id = x.Id,
            ReceiptNumber = x.ReceiptNumber,
            ReceiptDate = x.ReceiptDate,
            WarehouseName = x.Warehouse?.WarehouseNameAr ?? string.Empty,
            SourceType = x.SourceType,
            SourceName = x.SourceName,
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
        var vm = new CreateStoreReceiptVm { ReceiptDate = DateTime.Today };
        await FillLookupsAsync(vm);
        return View(vm);
    }

    [Authorize(Policy = CharityPolicies.StoresManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateStoreReceiptVm vm)
    {
        await FillLookupsAsync(vm);

        if (!ModelState.IsValid)
            return View(vm);

        if (await _receiptRepository.NumberExistsAsync(vm.ReceiptNumber.Trim()))
        {
            ModelState.AddModelError(nameof(vm.ReceiptNumber), "رقم الإذن موجود بالفعل");
            return View(vm);
        }

        var entity = new CharityStoreReceipt
        {
            Id = Guid.NewGuid(),
            ReceiptNumber = vm.ReceiptNumber.Trim(),
            WarehouseId = vm.WarehouseId,
            ReceiptDate = vm.ReceiptDate,
            SourceType = vm.SourceType,
            SourceName = vm.SourceName?.Trim(),
            Notes = vm.Notes?.Trim(),
            ApprovalStatus = "Pending",
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            Lines =
            {
                new CharityStoreReceiptLine
                {
                    Id = Guid.NewGuid(),
                    ItemId = vm.ItemId,
                    Quantity = vm.Quantity,
                    UnitCost = vm.UnitCost,
                    ExpiryDate = vm.ExpiryDate,
                    BatchNo = vm.BatchNo?.Trim(),
                    Notes = vm.Notes?.Trim()
                }
            }
        };

        await _receiptRepository.AddAsync(entity);
        TempData["Success"] = "تم حفظ إذن الإضافة المخزني في حالة انتظار الاعتماد.";
        return RedirectToAction(nameof(Details), new { id = entity.Id });
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var entity = await _receiptRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var vm = new StoreReceiptDetailsVm
        {
            Id = entity.Id,
            ReceiptNumber = entity.ReceiptNumber,
            ReceiptDate = entity.ReceiptDate,
            WarehouseName = entity.Warehouse?.WarehouseNameAr ?? string.Empty,
            SourceType = entity.SourceType,
            SourceName = entity.SourceName,
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
            Lines = entity.Lines.Select(x => new StoreReceiptLineVm
            {
                ItemCode = x.Item?.ItemCode ?? string.Empty,
                ItemName = x.Item?.ItemNameAr ?? string.Empty,
                Quantity = x.Quantity,
                UnitCost = x.UnitCost,
                ExpiryDate = x.ExpiryDate,
                BatchNo = x.BatchNo,
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
        var entity = await _db.Set<CharityStoreReceipt>()
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
                await _stockService.AddPurchaseAsync(new StockOperationRequest
                {
                    ItemId = line.ItemId,
                    WarehouseId = entity.WarehouseId,
                    Quantity = line.Quantity,
                    UnitCost = line.UnitCost,
                    TransactionDateUtc = entity.ReceiptDate,
                    ReferenceType = nameof(CharityStoreReceipt),
                    ReferenceNumber = entity.ReceiptNumber,
                    Notes = line.Notes ?? entity.Notes
                });
            }

            entity.ApprovalStatus = "Approved";
            entity.ApprovedAtUtc = DateTime.UtcNow;
            entity.ApprovedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            entity.RejectedAtUtc = null;
            entity.RejectedByUserId = null;
            entity.ApprovalNotes = string.IsNullOrWhiteSpace(approvalNotes) ? null : approvalNotes.Trim();
            await UpdateLinkedNeedRequestAfterReceiptApprovalAsync(entity, User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _db.SaveChangesAsync();

            await trx.CommitAsync();
        }
        catch (Exception ex)
        {
            await trx.RollbackAsync();
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Details), new { id });
        }

        var postingResult = await _journalHookService.TryCreateStoreReceiptEntryAsync(entity.Id);
        TempData[postingResult.IsSuccess ? "Success" : "Warning"] = postingResult.IsSuccess
            ? $"تم اعتماد إذن الإضافة بنجاح. {postingResult.Message}"
            : $"تم اعتماد إذن الإضافة، لكن هناك ملاحظة محاسبية: {postingResult.Message}";

        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Policy = CharityPolicies.StoresApprove)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(Guid id, string? approvalNotes)
    {
        var entity = await _db.Set<CharityStoreReceipt>().FirstOrDefaultAsync(x => x.Id == id);
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

        TempData["Success"] = "تم رفض إذن الإضافة المخزني.";
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task FillLookupsAsync(CreateStoreReceiptVm vm)
    {
        var warehouses = await _warehouseRepository.GetActiveAsync();
        var items = await _itemRepository.GetActiveAsync();

        vm.Warehouses = warehouses
            .OrderBy(x => x.WarehouseCode)
            .Select(x => new SelectListItem(x.WarehouseNameAr, x.Id.ToString()))
            .ToList();

        vm.Items = items
            .Where(x => x.IsStockItem && !x.IsService && x.IsActive)
            .OrderBy(x => x.ItemCode)
            .Select(x => new SelectListItem($"{x.ItemCode} - {x.ItemNameAr}", x.Id.ToString()))
            .ToList();
    }
    // ══════════════ Patch E: عند اعتماد إذن إضافة طلب الاحتياج، اجعل البند متاحًا للصرف العيني ══════════════
    private async Task UpdateLinkedNeedRequestAfterReceiptApprovalAsync(CharityStoreReceipt receipt, string? currentUserId)
    {
        if (!string.Equals(receipt.SourceType, "PurchaseNeedRequest", StringComparison.OrdinalIgnoreCase))
            return;

        if (!TryExtractGuidMarker(receipt.Notes, "StockNeedRequestId:", out var needRequestId))
            return;

        var needRequest = await _db.Set<StockNeedRequest>()
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == needRequestId);

        if (needRequest == null)
            return;

        foreach (var needLine in needRequest.Lines)
        {
            if (!needLine.ItemId.HasValue)
                continue;

            var receivedQty = receipt.Lines
                .Where(x => x.ItemId == needLine.ItemId.Value)
                .Sum(x => x.Quantity);

            if (receivedQty > 0m)
                needLine.FulfilledQuantity = Math.Max(needLine.FulfilledQuantity, receivedQty);
        }

        var allApprovedQty = needRequest.Lines.Sum(x => x.ApprovedQuantity > 0m ? x.ApprovedQuantity : x.RequestedQuantity);
        var fulfilledQty = needRequest.Lines.Sum(x => x.FulfilledQuantity);

        needRequest.Status = fulfilledQty > 0m && fulfilledQty >= allApprovedQty
            ? "Fulfilled"
            : "PartiallyFulfilled";
        needRequest.UpdatedAtUtc = DateTime.UtcNow;

        await CreateOrUpdateAvailableDisbursementFromNeedRequestAsync(needRequest, receipt, currentUserId);
    }
    private async Task CreateOrUpdateAvailableDisbursementFromNeedRequestAsync(StockNeedRequest needRequest, CharityStoreReceipt receipt, string? currentUserId)
    {
        if (!needRequest.BeneficiaryId.HasValue || !needRequest.BeneficiaryAidRequestId.HasValue)
            return;

        var aidRequest = await _db.BeneficiaryAidRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == needRequest.BeneficiaryAidRequestId.Value);

        if (aidRequest == null)
            return;

        var now = DateTime.UtcNow;
        var totalAmount = receipt.Lines.Sum(x => x.Quantity * x.UnitCost);

        var existing = await _db.Set<BeneficiaryAidDisbursement>()
            .FirstOrDefaultAsync(x => x.SourceType == "PurchaseNeedRequest" && x.SourceId == needRequest.Id);

        if (existing == null)
        {
            var disbursement = new BeneficiaryAidDisbursement
            {
                Id = Guid.NewGuid(),
                BeneficiaryId = needRequest.BeneficiaryId.Value,
                AidRequestId = needRequest.BeneficiaryAidRequestId.Value,
                AidRequestLineId = needRequest.BeneficiaryAidRequestLineId,
                AidTypeId = aidRequest.AidTypeId,
                DisbursementDate = DateTime.Today,
                Amount = totalAmount > 0m ? totalAmount : null,
                ProjectId = needRequest.ProjectId ?? aidRequest.ProjectId,
                ApprovalStatus = AidDisbursementApprovalStatusCodes.Approved,
                ApprovedAtUtc = now,
                ApprovedByUserId = currentUserId,
                ExecutionStatus = AidDisbursementExecutionStatusCodes.Available,
                ExecutedAmount = 0m,
                ExecutedAtUtc = null,
                ExecutedByUserId = null,
                StoreIssueId = null,
                SourceType = "PurchaseNeedRequest",
                SourceId = needRequest.Id,
                Notes = $"متاح للصرف العيني بعد اعتماد إذن الإضافة {receipt.ReceiptNumber}. StockNeedRequestId:{needRequest.Id} | StoreReceiptId:{receipt.Id} | BeneficiaryAidRequestId:{needRequest.BeneficiaryAidRequestId} | BeneficiaryAidRequestLineId:{needRequest.BeneficiaryAidRequestLineId}",
                CreatedByUserId = currentUserId,
                CreatedAtUtc = now
            };

            await _db.Set<BeneficiaryAidDisbursement>().AddAsync(disbursement);
            return;
        }

        existing.Amount = totalAmount > 0m ? totalAmount : existing.Amount;
        existing.ApprovalStatus = AidDisbursementApprovalStatusCodes.Approved;
        existing.ApprovedAtUtc ??= now;
        existing.ApprovedByUserId ??= currentUserId;
        existing.ExecutionStatus = AidDisbursementExecutionStatusCodes.Available;
        existing.ExecutedAmount ??= 0m;
        existing.Notes = AppendNote(existing.Notes, $"تم تحديث الإتاحة بعد اعتماد إذن الإضافة {receipt.ReceiptNumber}. StoreReceiptId:{receipt.Id}");
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
