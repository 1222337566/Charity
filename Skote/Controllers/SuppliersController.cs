using InfrastrfuctureManagmentCore.Domains.Suppliers;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Purchase;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Suppliers;
using InfrastructureManagmentWebFramework.Models.Optics;
using Microsoft.AspNetCore.Mvc;

public class SuppliersController : Controller
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IPurchaseInvoiceRepository _purchaseInvoiceRepository;

    public SuppliersController(
        ISupplierRepository supplierRepository,
        IPurchaseInvoiceRepository purchaseInvoiceRepository)
    {
        _supplierRepository = supplierRepository;
        _purchaseInvoiceRepository = purchaseInvoiceRepository;
    }

    public async Task<IActionResult> Index()
    {
        var suppliers = await _supplierRepository.GetAllAsync();

        var model = suppliers.Select(x => new SupplierListItemVm
        {
            Id = x.Id,
            SupplierNumber = x.SupplierNumber,
            NameAr = x.NameAr,
            ContactPerson = x.ContactPerson,
            Tel = x.Tel,
            MobileNo = x.MobileNo,
            IsActive = x.IsActive
        }).ToList();

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var vm = new CreateSupplierVm
        {
            SupplierNumber = GenerateSupplierNumber()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSupplierVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        if (await _supplierRepository.NumberExistsAsync(vm.SupplierNumber.Trim()))
        {
            ModelState.AddModelError(nameof(vm.SupplierNumber), "رقم المورد موجود بالفعل");
            return View(vm);
        }

        var entity = new Supplier
        {
            Id = Guid.NewGuid(),
            SupplierNumber = vm.SupplierNumber.Trim(),
            NameAr = vm.NameAr.Trim(),
            NameEn = vm.NameEn?.Trim(),
            ContactPerson = vm.ContactPerson?.Trim(),
            Tel = vm.Tel?.Trim(),
            MobileNo = vm.MobileNo?.Trim(),
            Address = vm.Address?.Trim(),
            TaxNumber = vm.TaxNumber?.Trim(),
            Remarks = vm.Remarks?.Trim(),
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _supplierRepository.AddAsync(entity);

        TempData["Success"] = "تم إضافة المورد بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _supplierRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var vm = new EditSupplierVm
        {
            Id = entity.Id,
            SupplierNumber = entity.SupplierNumber,
            NameAr = entity.NameAr,
            NameEn = entity.NameEn,
            ContactPerson = entity.ContactPerson,
            Tel = entity.Tel,
            MobileNo = entity.MobileNo,
            Address = entity.Address,
            TaxNumber = entity.TaxNumber,
            Remarks = entity.Remarks,
            IsActive = entity.IsActive
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditSupplierVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var entity = await _supplierRepository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        if (await _supplierRepository.NumberExistsAsync(vm.SupplierNumber.Trim(), vm.Id))
        {
            ModelState.AddModelError(nameof(vm.SupplierNumber), "رقم المورد موجود بالفعل");
            return View(vm);
        }

        entity.SupplierNumber = vm.SupplierNumber.Trim();
        entity.NameAr = vm.NameAr.Trim();
        entity.NameEn = vm.NameEn?.Trim();
        entity.ContactPerson = vm.ContactPerson?.Trim();
        entity.Tel = vm.Tel?.Trim();
        entity.MobileNo = vm.MobileNo?.Trim();
        entity.Address = vm.Address?.Trim();
        entity.TaxNumber = vm.TaxNumber?.Trim();
        entity.Remarks = vm.Remarks?.Trim();
        entity.IsActive = vm.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _supplierRepository.UpdateAsync(entity);

        TempData["Success"] = "تم تعديل المورد بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var entity = await _supplierRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var purchases = await _purchaseInvoiceRepository.GetBySupplierIdAsync(entity.Id);

        var vm = new SupplierDetailsVm
        {
            Id = entity.Id,
            SupplierNumber = entity.SupplierNumber,
            NameAr = entity.NameAr,
            ContactPerson = entity.ContactPerson,
            Tel = entity.Tel,
            MobileNo = entity.MobileNo,
            Address = entity.Address,
            TaxNumber = entity.TaxNumber,
            Remarks = entity.Remarks,
            IsActive = entity.IsActive,
            PurchasesCount = purchases.Count,
            Purchases = purchases.Select(x => new SupplierPurchaseItemVm
            {
                PurchaseInvoiceId = x.Id,
                InvoiceNumber = x.InvoiceNumber,
                InvoiceDateUtc = x.InvoiceDateUtc,
                WarehouseName = x.Warehouse?.WarehouseNameAr ?? "-",
                NetAmount = x.NetAmount,
                Notes = x.Notes
            }).ToList()
        };

        return View(vm);
    }

    private static string GenerateSupplierNumber()
    {
        return "SUP-" + DateTime.Now.ToString("yyMMddHHmmss");
    }
}