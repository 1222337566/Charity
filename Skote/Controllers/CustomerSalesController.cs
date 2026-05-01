using InfrastrfuctureManagmentCore.Domains.Item;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Customer;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Payments;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Prescriptions;
using InfrastructureManagmentServices.Sales;
using InfrastructureManagmentWebFramework.Models.Optics;
using InfrastructureManagmentWebFramework.Models.Sale;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
/// مؤجل مؤقتًا - خاص بمسار النظارات
/// غير مستخدم من الواجهة الحال
public class CustomerSalesController : Controller
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IPrescriptionRepository _prescriptionRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly ISalesService _salesService;

    public CustomerSalesController(
        ICustomerRepository customerRepository,
        IPrescriptionRepository prescriptionRepository,
        IItemRepository itemRepository,
        IWarehouseRepository warehouseRepository,
        IPaymentMethodRepository paymentMethodRepository,
        ISalesService salesService)
    {
        _customerRepository = customerRepository;
        _prescriptionRepository = prescriptionRepository;
        _itemRepository = itemRepository;
        _warehouseRepository = warehouseRepository;
        _paymentMethodRepository = paymentMethodRepository;
        _salesService = salesService;
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid customerId)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null)
            return NotFound();

        var vm = new CustomerOpticalSaleVm
        {
            CustomerId = customer.Id,
            CustomerNumber = customer.CustomerNumber,
            CustomerName = customer.NameAr,
            InvoiceNumber = "SAL-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
            InvoiceDateUtc = DateTime.UtcNow,
            Payments = new List<SalesPaymentVm>
            {
                new SalesPaymentVm()
            }
        };

        await FillLookups(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CustomerOpticalSaleVm vm)
    {
        var customer = await _customerRepository.GetByIdAsync(vm.CustomerId);
        if (customer == null)
            return NotFound();

        vm.CustomerNumber = customer.CustomerNumber;
        vm.CustomerName = customer.NameAr;

        await FillLookups(vm);

        if (!ModelState.IsValid)
            return View(vm);

        var lines = new List<SalesInvoiceLineVm>();

        await AddLineIfSelected(lines, vm.FrameItemId, vm.FrameQty, vm.FrameUnitPrice);
        await AddLineIfSelected(lines, vm.LensItemId, vm.LensQty, vm.LensUnitPrice);
        await AddLineIfSelected(lines, vm.ContactLensItemId, vm.ContactLensQty, vm.ContactLensUnitPrice);
        await AddLineIfSelected(lines, vm.AccessoryItemId, vm.AccessoryQty, vm.AccessoryUnitPrice);

        if (!lines.Any())
        {
            ModelState.AddModelError(string.Empty, "اختر صنفًا واحدًا على الأقل");
            return View(vm);
        }

        var salesVm = new CreateSalesInvoiceVm
        {
            InvoiceNumber = vm.InvoiceNumber.Trim(),
            InvoiceDateUtc = vm.InvoiceDateUtc,
            CustomerId = vm.CustomerId,
            CustomerName = vm.CustomerName,
            PrescriptionId = vm.PrescriptionId,
            WarehouseId = vm.WarehouseId,
            Notes = vm.Notes,
            Lines = lines,
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
            TempData["Success"] = "تم حفظ Customer Sale بنجاح";
            return RedirectToAction("PrintReceipt", "SalesInvoices", new { id = invoiceId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(vm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetPrescription(Guid id)
    {
        var prescription = await _prescriptionRepository.GetByIdAsync(id);
        if (prescription == null)
            return Json(new { ok = false });

        return Json(new
        {
            ok = true,
            data = new
            {
                date = prescription.PrescriptionDateUtc.ToString("yyyy-MM-dd"),
                doctor = prescription.DoctorName,
                rightSph = prescription.RightSph,
                rightCyl = prescription.RightCyl,
                rightAxis = prescription.RightAxis,
                leftSph = prescription.LeftSph,
                leftCyl = prescription.LeftCyl,
                leftAxis = prescription.LeftAxis,
                addValue = prescription.AddValue,
                ipd = prescription.IPD,
                sHeight = prescription.SHeight,
                remarks = prescription.Remarks
            }
        });
    }

    private async Task FillLookups(CustomerOpticalSaleVm vm)
    {
        var warehouses = await _warehouseRepository.GetActiveAsync();
        var prescriptions = await _prescriptionRepository.GetActiveByCustomerIdAsync(vm.CustomerId);
        var frames = await _itemRepository.GetByOpticalTypeAsync(OpticalItemType.Frame);
        var lenses = await _itemRepository.GetByOpticalTypeAsync(OpticalItemType.Lens);
        var contactLenses = await _itemRepository.GetByOpticalTypeAsync(OpticalItemType.ContactLens);
        var accessories = await _itemRepository.GetByOpticalTypeAsync(OpticalItemType.Accessory);
        var paymentMethods = await _paymentMethodRepository.GetActiveAsync();

        vm.Warehouses = warehouses.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.WarehouseCode} - {x.WarehouseNameAr}"
        }).ToList();

        vm.Prescriptions = prescriptions.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.PrescriptionDateUtc:yyyy-MM-dd} - Dr: {(string.IsNullOrWhiteSpace(x.DoctorName) ? "-" : x.DoctorName)}"
        }).ToList();

        vm.Frames = frames.Select(x => new OpticalItemOptionVm
        {
            Id = x.Id,
            DisplayName = $"{x.ItemCode} - {x.ItemNameAr} {(string.IsNullOrWhiteSpace(x.BrandName) ? "" : "- " + x.BrandName)}",
            SalePrice = x.SalePrice
        }).ToList();

        vm.Lenses = lenses.Select(x => new OpticalItemOptionVm
        {
            Id = x.Id,
            DisplayName = $"{x.ItemCode} - {x.ItemNameAr}",
            SalePrice = x.SalePrice
        }).ToList();

        vm.ContactLenses = contactLenses.Select(x => new OpticalItemOptionVm
        {
            Id = x.Id,
            DisplayName = $"{x.ItemCode} - {x.ItemNameAr}",
            SalePrice = x.SalePrice
        }).ToList();

        vm.Accessories = accessories.Select(x => new OpticalItemOptionVm
        {
            Id = x.Id,
            DisplayName = $"{x.ItemCode} - {x.ItemNameAr}",
            SalePrice = x.SalePrice
        }).ToList();

        if (vm.Payments == null || !vm.Payments.Any())
        {
            vm.Payments = new List<SalesPaymentVm> { new SalesPaymentVm() };
        }

        foreach (var payment in vm.Payments)
        {
            payment.PaymentMethods = paymentMethods.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.MethodCode} - {x.MethodNameAr}"
            }).ToList();
        }
    }

    private async Task AddLineIfSelected(List<SalesInvoiceLineVm> lines, Guid? itemId, decimal qty, decimal price)
    {
        if (!itemId.HasValue || itemId.Value == Guid.Empty || qty <= 0)
            return;

        var item = await _itemRepository.GetByIdAsync(itemId.Value);
        if (item == null || !item.IsActive)
            throw new InvalidOperationException("أحد الأصناف غير موجود أو غير نشط");

        lines.Add(new SalesInvoiceLineVm
        {
            ItemId = item.Id,
            Quantity = qty,
            UnitPrice = price > 0 ? price : item.SalePrice,
            DiscountAmount = 0,
            TaxAmount = 0
        });
    }
}