using InfrastrfuctureManagmentCore.Domains.Charity.StockAdvanced;
using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.StockAdvanced;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Purchase;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Suppliers;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Warehouses;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentServices.Charity.Workflow;
using InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.AidRequests;
using InfrastructureManagmentWebFramework.Models.Purchase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Skote.Controllers
{
    public class StockNeedRequestsController : Controller
    {
        private readonly IStockNeedRequestRepository _repo;
        private readonly IItemRepository             _itemRepo;
        private readonly IWarehouseRepository        _warehouseRepo;
        private readonly ISupplierRepository         _supplierRepo;
        private readonly IWorkflowService            _workflow;
        private readonly AppDbContext                _db;

        public StockNeedRequestsController(
            IStockNeedRequestRepository repo,
            IItemRepository itemRepo,
            IWarehouseRepository warehouseRepo,
            ISupplierRepository supplierRepo,
            IWorkflowService workflow,
            AppDbContext db)
        {
            _repo         = repo;
            _itemRepo     = itemRepo;
            _warehouseRepo = warehouseRepo;
            _supplierRepo = supplierRepo;
            _workflow     = workflow;
            _db           = db;
        }

        // ══════════════ Index ══════════════
        public async Task<IActionResult> Index()
        {
            var list = await _repo.GetAllAsync();
            return View(list);
        }

        // ══════════════ Details ══════════════
        public async Task<IActionResult> Details(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return NotFound();

            // مسار الـ Workflow
            var wfSteps = await _workflow.GetStepsAsync("StockNeedRequest", id);

            // فاتورة الشراء المرتبطة
            InfrastrfuctureManagmentCore.Domains.Purchase.PurchaseInvoice? linkedInvoice = null;
            if (entity.LinkedPurchaseInvoiceId.HasValue)
                linkedInvoice = await _db.Set<InfrastrfuctureManagmentCore.Domains.Purchase.PurchaseInvoice>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == entity.LinkedPurchaseInvoiceId.Value);

            ViewBag.WorkflowSteps  = wfSteps;
            ViewBag.LinkedInvoice  = linkedInvoice;
            return View(entity);
        }

        // ══════════════ Create ══════════════
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Items = await GetItemsAsync();
            return View(new StockNeedRequest
            {
                RequestDate = DateTime.UtcNow.Date,
                Lines       = new List<StockNeedRequestLine> { new() }
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StockNeedRequest model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Items = await GetItemsAsync();
                return View(model);
            }

            model.Id            = Guid.NewGuid();
            model.RequestNumber = $"NR-{DateTime.UtcNow:yyyyMMddHHmmss}";
            model.Status        = "Draft";
            model.CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            foreach (var line in model.Lines.Where(l => l.ItemId != Guid.Empty))
            {
                line.Id = Guid.NewGuid();
                line.StockNeedRequestId = model.Id;
                line.ApprovedQuantity   = line.RequestedQuantity;
            }

            await _repo.AddAsync(model);

            // بدء مسار الموافقة
            await _workflow.InitiateAsync(
                entityType:        "StockNeedRequest",
                entityId:          model.Id,
                entityTitle:       $"طلب احتياج {model.RequestNumber}",
                submittedByUserId: model.CreatedByUserId ?? "system"
            );

            TempData["Success"] = "تم حفظ طلب الاحتياج وبدأ مسار الموافقة";
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        // ══════════════ Approve (من خارج الـ Workflow) ══════════════
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return NotFound();

            entity.Status         = "Approved";
            entity.ApprovedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            entity.ApprovedAtUtc  = DateTime.UtcNow;
            entity.UpdatedAtUtc   = DateTime.UtcNow;
            await _repo.UpdateAsync(entity);

            TempData["Success"] = "تم اعتماد طلب الاحتياج — يمكنك الآن إنشاء فاتورة شراء";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(Guid id, string reason)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return NotFound();

            entity.Status          = "Rejected";
            entity.RejectionReason = reason;
            entity.UpdatedAtUtc    = DateTime.UtcNow;
            await _repo.UpdateAsync(entity);

            TempData["Warning"] = "تم رفض طلب الاحتياج";
            return RedirectToAction(nameof(Details), new { id });
        }

        // ══════════════ إنشاء فاتورة شراء من طلب الاحتياج ══════════════
        [HttpGet]
        public async Task<IActionResult> CreatePurchaseInvoice(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return NotFound();

            if (entity.Status != "Approved")
            {
                TempData["Error"] = "يجب اعتماد طلب الاحتياج أولاً قبل إنشاء فاتورة الشراء";
                return RedirectToAction(nameof(Details), new { id });
            }

            // احتساب بيانات الأصناف
            var itemIds = entity.Lines.Select(l => l.ItemId).ToList();
            var items   = (await _itemRepo.GetActiveAsync())
                .Where(x => itemIds.Contains(x.Id))
                .ToDictionary(x => x.Id);

            // إنشاء الـ VM مع pre-fill من طلب الاحتياج
            var vm = new CreatePurchaseInvoiceVm
            {
                InvoiceNumber  = $"PI-{entity.RequestNumber}",
                InvoiceDateUtc = DateTime.UtcNow,
                Notes          = $"مُنشأة من طلب الاحتياج {entity.RequestNumber}",
                Lines = entity.Lines.Select(l => new PurchaseInvoiceLineVm
                {
                    ItemId   = (Guid)l.ItemId,
                    Quantity = l.ApprovedQuantity,
                    UnitCost = 0
                }).ToList()
            };

            ViewBag.NeedRequest = entity;
            ViewBag.NeedRequestId = id;
            await FillInvoiceLookupsAsync(vm);
            return View("CreatePurchaseInvoice", vm);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromAidRequestLine(Guid aidRequestId, Guid lineId)
        {
            var aidRequest = await _db.BeneficiaryAidRequests
                .AsNoTracking()
                .Include(x => x.Beneficiary)
                .FirstOrDefaultAsync(x => x.Id == aidRequestId);

            if (aidRequest == null)
                return NotFound();

            var line = await _db.BeneficiaryAidRequestLines
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == lineId && x.AidRequestId == aidRequestId);

            if (line == null)
                return NotFound();

            if (!string.Equals(line.FulfillmentMethod, BeneficiaryAidRequestFulfillmentMethodOption.PurchaseNeedRequest, StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "لا يمكن إنشاء طلب احتياج إلا للبنود التي طريقة تنفيذها: طلب احتياج للشراء.";
                return RedirectToAction("Details", "BeneficiaryAidRequests", new { id = aidRequestId });
            }

            var existing = await _db.Set<StockNeedRequest>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.BeneficiaryAidRequestLineId == lineId);

            if (existing != null)
            {
                TempData["Warning"] = $"تم إنشاء طلب احتياج لهذا البند من قبل: {existing.RequestNumber}";
                return RedirectToAction(nameof(Details), new { id = existing.Id });
            }

            var quantity = line.ApprovedQuantity.GetValueOrDefault() > 0m
                ? line.ApprovedQuantity.GetValueOrDefault()
                : line.RequestedQuantity.GetValueOrDefault(1m);

            if (quantity <= 0m)
                quantity = 1m;

            var itemName = !string.IsNullOrWhiteSpace(line.ItemNameSnapshot)
                ? line.ItemNameSnapshot!.Trim()
                : line.Description?.Trim();

            var estimatedTotal = line.EstimatedTotalValue ??
                (line.EstimatedUnitValue.HasValue ? quantity * line.EstimatedUnitValue.Value : null);

            var need = new StockNeedRequest
            {
                Id = Guid.NewGuid(),
                RequestNumber = $"NR-AID-{DateTime.UtcNow:yyyyMMddHHmmss}",
                RequestDate = DateTime.UtcNow.Date,
                RequestType = "Beneficiary",
                BeneficiaryId = aidRequest.BeneficiaryId,
                BeneficiaryAidRequestId = aidRequest.Id,
                BeneficiaryAidRequestLineId = line.Id,
                AidRequestLineDescriptionSnapshot = line.Description,
                EstimatedTotalValue = estimatedTotal,
                RequestedByName = User.Identity?.Name,
                Status = "Draft",
                Notes = $"طلب احتياج منشأ من بند طلب مساعدة للمستفيد: {aidRequest.Beneficiary?.FullName ?? aidRequest.BeneficiaryId.ToString()}",
                CreatedAtUtc = DateTime.UtcNow,
                CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CreatedByUserName = User.Identity?.Name,
                Lines = new List<StockNeedRequestLine>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        ItemId = line.ItemId,
                        ItemNameSnapshot = itemName,
                        RequestedQuantity = quantity,
                        ApprovedQuantity = quantity,
                        EstimatedUnitValue = line.EstimatedUnitValue,
                        EstimatedTotalValue = estimatedTotal,
                        Notes = $"AidRequestId={aidRequest.Id}; AidRequestLineId={line.Id}; {line.Notes}"?.Trim()
                    }
                }
            };

            _db.Set<StockNeedRequest>().Add(need);
            await _db.SaveChangesAsync();

            await _workflow.InitiateAsync(
                entityType: "StockNeedRequest",
                entityId: need.Id,
                entityTitle: $"طلب احتياج {need.RequestNumber}",
                submittedByUserId: need.CreatedByUserId ?? "system"
            );

            TempData["Success"] = "تم إنشاء طلب الاحتياج من بند طلب المساعدة بنجاح.";
            return RedirectToAction(nameof(Details), new { id = need.Id });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePurchaseInvoice(
            CreatePurchaseInvoiceVm vm, Guid needRequestId)
        {
            var entity = await _repo.GetByIdAsync(needRequestId);
            if (entity == null) return NotFound();

            ViewBag.NeedRequest   = entity;
            ViewBag.NeedRequestId = needRequestId;
            await FillInvoiceLookupsAsync(vm);

            if (!ModelState.IsValid)
                return View("CreatePurchaseInvoice", vm);

            // احسب الإجماليات
            var subTotal = vm.Lines.Sum(l => l.Quantity * l.UnitCost - l.DiscountAmount);
            var tax      = vm.Lines.Sum(l => l.TaxAmount);

            var invoice = new InfrastrfuctureManagmentCore.Domains.Purchase.PurchaseInvoice
            {
                Id                   = Guid.NewGuid(),
                InvoiceNumber        = vm.InvoiceNumber,
                InvoiceDateUtc       = vm.InvoiceDateUtc,
                SupplierId           = vm.SupplierId,
                SupplierName         = vm.SupplierName,
                SupplierInvoiceNumber = vm.SupplierInvoiceNumber,
                WarehouseId          = vm.WarehouseId,
                SubTotal             = subTotal,
                TaxAmount            = tax,
                DiscountAmount       = vm.Lines.Sum(l => l.DiscountAmount),
                NetAmount            = subTotal + tax,
                Notes = string.IsNullOrWhiteSpace(vm.Notes)
                    ? $"مُنشأة من طلب الاحتياج {entity.RequestNumber}. StockNeedRequestId:{needRequestId}"
                    : $"{vm.Notes.Trim()}\nStockNeedRequestId:{needRequestId}",
                StockNeedRequestId   = needRequestId,
                Status = InfrastrfuctureManagmentCore.Domains.Purchase.PurchaseInvoiceStatus.Draft,
                Lines = vm.Lines.Select(l => new InfrastrfuctureManagmentCore.Domains.Purchase.PurchaseInvoiceLine
                {
                    Id               = Guid.NewGuid(),
                    ItemId           = l.ItemId,
                    Quantity         = l.Quantity,
                    UnitCost         = l.UnitCost,
                    DiscountAmount   = l.DiscountAmount,
                    TaxAmount        = l.TaxAmount,
                    LineTotal        = l.Quantity * l.UnitCost
                }).ToList()
            };

            _db.Set<InfrastrfuctureManagmentCore.Domains.Purchase.PurchaseInvoice>().Add(invoice);

            // حدّث حالة طلب الاحتياج
            entity.Status                = "Ordered";
            entity.LinkedPurchaseInvoiceId = invoice.Id;
            entity.UpdatedAtUtc          = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            TempData["Success"] = $"تم إنشاء فاتورة الشراء {invoice.InvoiceNumber} — بعد الترحيل ستُضاف الكميات للمخزن";
            return RedirectToAction("Details", "PurchaseInvoices", new { id = invoice.Id });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStoreReceiptFromPurchase(Guid id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var entity = await _db.Set<StockNeedRequest>()
                .Include(x => x.Lines)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return NotFound();

            if (!entity.LinkedPurchaseInvoiceId.HasValue)
            {
                TempData["Warning"] = "لا توجد فاتورة شراء مرتبطة بطلب الاحتياج حتى الآن.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var existingReceipt = await _db.Set<CharityStoreReceipt>()
                .AsNoTracking()
                .Where(x => x.Notes != null
                    && x.Notes.Contains($"StockNeedRequestId:{entity.Id}")
                    && x.Notes.Contains($"PurchaseInvoiceId:{entity.LinkedPurchaseInvoiceId.Value}"))
                .OrderByDescending(x => x.CreatedAtUtc)
                .FirstOrDefaultAsync();

            if (existingReceipt != null)
            {
                TempData["Info"] = "تم إنشاء إذن إضافة مخزني لهذه الفاتورة من قبل.";
                return RedirectToAction("Details", "CharityStoreReceipts", new { id = existingReceipt.Id });
            }

            var invoice = await _db.Set<InfrastrfuctureManagmentCore.Domains.Purchase.PurchaseInvoice>()
                .Include(x => x.Lines)
                .FirstOrDefaultAsync(x => x.Id == entity.LinkedPurchaseInvoiceId.Value);

            if (invoice == null)
            {
                TempData["Warning"] = "تعذر العثور على فاتورة الشراء المرتبطة بطلب الاحتياج.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var invoiceLines = invoice.Lines
                .Where(x => x.ItemId != Guid.Empty && x.Quantity > 0m)
                .ToList();

            if (invoiceLines.Count == 0)
            {
                TempData["Warning"] = "لا توجد أصناف صالحة داخل فاتورة الشراء لإنشاء إذن الإضافة المخزني.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var receipt = new CharityStoreReceipt
            {
                Id = Guid.NewGuid(),
                ReceiptNumber = await GenerateAutoStoreReceiptNumberAsync(),
                WarehouseId = invoice.WarehouseId,
                ReceiptDate = DateTime.Today,
                SourceType = "PurchaseNeedRequest",
                SourceName = entity.RequestNumber,
                ApprovalStatus = "Pending",
                Notes = $"إذن إضافة مخزني من فاتورة شراء مرتبطة بطلب احتياج. StockNeedRequestId:{entity.Id} | PurchaseInvoiceId:{invoice.Id} | BeneficiaryAidRequestId:{entity.BeneficiaryAidRequestId} | BeneficiaryAidRequestLineId:{entity.BeneficiaryAidRequestLineId}",
                CreatedAtUtc = DateTime.UtcNow,
                CreatedByUserId = currentUserId,
                Lines = invoiceLines.Select(line => new CharityStoreReceiptLine
                {
                    Id = Guid.NewGuid(),
                    ItemId = line.ItemId,
                    Quantity = line.Quantity,
                    UnitCost = line.UnitCost,
                    Notes = $"توريد لصالح طلب الاحتياج {entity.RequestNumber}. StockNeedRequestId:{entity.Id}"
                }).ToList()
            };

            _db.Set<CharityStoreReceipt>().Add(receipt);

            entity.Status = "ReceiptPending";
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            TempData["Success"] = "تم إنشاء إذن الإضافة المخزني في حالة انتظار الاعتماد. بعد اعتماد الإذن سيظهر البند كصرف عيني متاح للمستفيد.";
            return RedirectToAction("Details", "CharityStoreReceipts", new { id = receipt.Id });
        }
        // ══════════════ Helpers ══════════════
        private async Task<string> GenerateAutoStoreReceiptNumberAsync()
        {
            string receiptNumber;
            do
            {
                receiptNumber = $"GRN-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}"[..31];
            }
            while (await _db.Set<CharityStoreReceipt>().AsNoTracking().AnyAsync(x => x.ReceiptNumber == receiptNumber));

            return receiptNumber;
        }

        private async Task<List<SelectListItem>> GetItemsAsync()
            => (await _itemRepo.GetActiveAsync())
                .Where(x => x.IsStockItem && !x.IsService)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text  = $"{x.ItemCode} - {x.ItemNameAr}"
                }).ToList();

        private async Task FillInvoiceLookupsAsync(CreatePurchaseInvoiceVm vm)
        {
            var warehouses = await _warehouseRepo.GetActiveAsync();
            var suppliers  = await _supplierRepo.GetAllAsync();
            var items      = (await _itemRepo.GetActiveAsync()).Where(x => x.IsStockItem && !x.IsService);

            vm.Warehouses = warehouses.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text  = $"{x.WarehouseCode} - {x.WarehouseNameAr}"
            }).ToList();

            ViewBag.Suppliers = suppliers.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text  = x.NameAr
            }).ToList();

            ViewBag.Items = items.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text  = $"{x.ItemCode} - {x.ItemNameAr}"
            }).ToList();
        }
    }
}
