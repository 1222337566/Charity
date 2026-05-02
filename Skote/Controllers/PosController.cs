using InfrastrfuctureManagmentCore.Domains.Item;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Payments;
using InfrastrfuctureManagmentCore.Persistence.Repositories.PosHolds;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Sale;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Warehouses;
using InfrastructureManagmentServices.PosHolds;
using InfrastructureManagmentServices.Sales;
using InfrastructureManagmentWebFramework.Models.POS;
using InfrastructureManagmentWebFramework.Models.Sale;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class PosController : Controller
{
    private readonly IItemRepository _itemRepository;
    private readonly IPosHoldService _posHoldService;
    private readonly IPosHoldRepository _posHoldRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly ISalesService _salesService;
    private readonly ISalesInvoiceRepository _salesInvoiceRepository;
    private readonly IItemWarehouseBalanceRepository  _itemWarehouseBalanceRepository;

    public PosController(
        IItemRepository itemRepository,
        IWarehouseRepository warehouseRepository,
        IPaymentMethodRepository paymentMethodRepository,
        ISalesService salesService,
        IItemWarehouseBalanceRepository itemWarehouseBalanceRepository,
        IPosHoldRepository posHoldRepository,
        IPosHoldService posHoldService,
        ISalesInvoiceRepository salesInvoiceRepository)
    {
        _itemRepository = itemRepository;
        _warehouseRepository = warehouseRepository;
        _paymentMethodRepository = paymentMethodRepository;
        _salesService = salesService;
        _itemWarehouseBalanceRepository = itemWarehouseBalanceRepository;
        _posHoldRepository = posHoldRepository;
        _posHoldService = posHoldService;
        _salesInvoiceRepository = salesInvoiceRepository;
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveAjax(PosSaleVm vm)
    {
        await FillLookups(vm);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "بيانات غير صحيحة" : e.ErrorMessage)
                .ToList();

            return Json(new
            {
                ok = false,
                message = string.Join(" | ", errors)
            });
        }

        if (vm.Lines == null || !vm.Lines.Any(x => x.ItemId != Guid.Empty && x.Quantity > 0))
        {
            return Json(new
            {
                ok = false,
                message = "أضف صنفًا واحدًا على الأقل"
            });
        }

        var paymentMethod = await _paymentMethodRepository.GetByIdAsync(vm.PaymentMethodId);
        if (paymentMethod == null || !paymentMethod.IsActive)
        {
            return Json(new
            {
                ok = false,
                message = "طريقة الدفع غير موجودة أو غير نشطة"
            });
        }

        var salesVm = new CreateSalesInvoiceVm
        {
            CustomerId = vm.CustomerId,
            InvoiceNumber = vm.InvoiceNumber.Trim(),
            InvoiceDateUtc = vm.InvoiceDateUtc,
            CustomerName = vm.CustomerName.Trim(),
            WarehouseId = vm.WarehouseId,
            Notes = vm.Notes,
            Lines = vm.Lines
                .Where(x => x.ItemId != Guid.Empty && x.Quantity > 0)
                .Select(x => new SalesInvoiceLineVm
                {
                    ItemId = x.ItemId,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    DiscountAmount = x.DiscountAmount,
                    TaxAmount = x.TaxAmount
                })
                .ToList()
        };

        try
        {
            var invoiceId = await _salesService.CreateAsync(salesVm);

            return Json(new
            {
                ok = true,
                invoiceId = invoiceId,
                printUrl = Url.Action("PrintReceipt", "SalesInvoices", new { id = invoiceId }),
                nextInvoiceNumber = "POS-" + DateTime.Now.AddSeconds(1).ToString("yyyyMMddHHmmss"),
                message = "تم حفظ الفاتورة بنجاح"
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                ok = false,
                message = ex.Message
            });
        }
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var vm = new PosSaleVm
        {
            InvoiceDateUtc = DateTime.UtcNow,
            CustomerName = "عميل نقدي",
            InvoiceNumber = GenerateInvoiceNumber(),
            Lines = new List<PosSaleLineVm>(),
            Payments = new List<PosPaymentVm>
    {
        new PosPaymentVm()
    }
        };

        await FillLookups(vm);
        return View(vm);
    }
    [HttpGet]
    public async Task<IActionResult> HeldInvoices()
    {
        var holds = await _posHoldRepository.GetHeldAsync();
        return View(holds);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Resume(Guid id)
    {
        var vm = await _posHoldService.ResumeAsync(id);
        if (vm == null)
        {
            TempData["Error"] = "الفاتورة المعلقة غير موجودة أو غير متاحة";
            return RedirectToAction(nameof(HeldInvoices));
        }

        await FillLookups(vm);
        TempData["Success"] = "تم استرجاع الفاتورة المعلقة";
        return View("Index", vm);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Hold(PosSaleVm vm)
    {
        await FillLookups(vm);

        try
        {
            await _posHoldService.HoldAsync(vm);
            TempData["Success"] = "تم تعليق الفاتورة بنجاح";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("Index", vm);
        }
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(PosSaleVm vm)
    {
        await FillLookups(vm);

        if (!ModelState.IsValid)
            return View("Index", vm);

        if (vm.Lines == null || !vm.Lines.Any(x => x.ItemId != Guid.Empty && x.Quantity > 0))
        {
            ModelState.AddModelError(string.Empty, "أضف صنفًا واحدًا على الأقل");
            return View("Index", vm);
        }

        var paymentMethod = await _paymentMethodRepository.GetByIdAsync(vm.PaymentMethodId);
        if (paymentMethod == null || !paymentMethod.IsActive)
        {
            ModelState.AddModelError(nameof(vm.PaymentMethodId), "طريقة الدفع غير موجودة أو غير نشطة");
            return View("Index", vm);
        }

        var salesVm = new CreateSalesInvoiceVm
        {
            CustomerId = vm.CustomerId,
            InvoiceNumber = vm.InvoiceNumber.Trim(),
            InvoiceDateUtc = vm.InvoiceDateUtc,
            CustomerName = vm.CustomerName.Trim(),
            WarehouseId = vm.WarehouseId,
            Notes = vm.Notes,
            Lines = vm.Lines
         .Where(x => x.ItemId != Guid.Empty && x.Quantity > 0)
         .Select(x => new SalesInvoiceLineVm
         {
             ItemId = x.ItemId,
             Quantity = x.Quantity,
             UnitPrice = x.UnitPrice,
             DiscountAmount = x.DiscountAmount,
             TaxAmount = x.TaxAmount
         })
         .ToList(),
            Payments = vm.Payments
         .Where(x => x.PaymentMethodId != Guid.Empty && x.Amount > 0)
         .Select(x => new SalesPaymentVm
         {
             PaymentMethodId = x.PaymentMethodId,
             Amount = x.Amount
         })
         .ToList()
        };

        try
        {
            var invoiceId = await _salesService.CreateAsync(salesVm);
            TempData["Success"] = "تم حفظ فاتورة البيع بنجاح";
            return RedirectToAction("PrintReceipt", "SalesInvoices", new { id = invoiceId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("Index", vm);
        }
    }
    [HttpGet]
    public async Task<IActionResult> PrintReceipt(Guid id)
    {
        var invoice = await _salesInvoiceRepository.GetByIdAsync(id);
        if (invoice == null)
            return NotFound();

        return View(invoice);
    }

    [HttpGet]
    public async Task<IActionResult> FindItem(string q, Guid? warehouseId)
    {
        q = (q ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(q))
            return Json(new { ok = false, message = "قيمة البحث مطلوبة" });

        Item? item = await _itemRepository.GetByBarcodeAsync(q);

        if (item == null)
            item = await _itemRepository.GetByCodeAsync(q);

        if (item == null)
        {
            var all = await _itemRepository.GetActiveAsync();
            item = all.FirstOrDefault(x =>
                (!string.IsNullOrWhiteSpace(x.ItemNameAr) && x.ItemNameAr.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(x.ItemNameEn) && x.ItemNameEn.Contains(q, StringComparison.OrdinalIgnoreCase)));
        }

        if (item == null)
            return Json(new { ok = false, message = "الصنف غير موجود" });

        if (!item.IsActive)
            return Json(new { ok = false, message = "الصنف غير نشط" });

        if (item.IsService || !item.IsStockItem)
            return Json(new { ok = false, message = "هذا الصنف غير مناسب للبيع من شاشة POS الحالية" });

        decimal availableQty = 0;

        if (warehouseId.HasValue && warehouseId.Value != Guid.Empty)
        {
            var balance = await _itemWarehouseBalanceRepository.GetByItemAndWarehouseAsync(item.Id, warehouseId.Value);
            availableQty = balance?.AvailableQuantity ?? 0;
        }

        return Json(new
        {
            ok = true,
            item = new
            {
                id = item.Id,
                code = item.ItemCode,
                name = item.ItemNameAr,
                barcode = item.Barcode,
                salePrice = item.SalePrice,
                isTaxable = item.IsTaxable,
                taxRate = item.TaxRate,
                availableQty = availableQty
            }
        });
    }


    private async Task FillLookups(PosSaleVm vm)
    {
        var warehouses = await _warehouseRepository.GetActiveAsync();
        var methods = await _paymentMethodRepository.GetActiveAsync();

        vm.Warehouses = warehouses.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.WarehouseCode} - {x.WarehouseNameAr}"
        }).ToList();
        if (vm.Payments == null || !vm.Payments.Any())
        {
            vm.Payments = new List<PosPaymentVm> { new PosPaymentVm() };
        }

        foreach (var payment in vm.Payments)
        {
            payment.PaymentMethods = methods.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.MethodCode} - {x.MethodNameAr}"
            }).ToList();
        }
        vm.PaymentMethods = methods.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.MethodCode} - {x.MethodNameAr}"
        }).ToList();
    }

    private static string GenerateInvoiceNumber()
    {
        return "POS-" + DateTime.Now.ToString("yyyyMMddHHmmss");
    }
}