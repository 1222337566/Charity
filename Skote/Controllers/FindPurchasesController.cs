using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Purchase;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Suppliers;
using InfrastructureManagmentWebFramework.Models.Optics.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
/// مؤجل مؤقتًا - خاص بمسار النظارات
/// غير مستخدم من الواجهة الحال
public class FindPurchasesController : Controller
{
    private readonly IPurchaseInvoiceRepository _purchaseInvoiceRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IWarehouseRepository _warehouseRepository;

    public FindPurchasesController(
        IPurchaseInvoiceRepository purchaseInvoiceRepository,
        ISupplierRepository supplierRepository,
        IWarehouseRepository warehouseRepository)
    {
        _purchaseInvoiceRepository = purchaseInvoiceRepository;
        _supplierRepository = supplierRepository;
        _warehouseRepository = warehouseRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? q,
        DateTime? dateFrom,
        DateTime? dateTo,
        Guid? supplierId,
        Guid? warehouseId)
    {
        var vm = new FindPurchasePageVm
        {
            Filter = new FindPurchaseFilterVm
            {
                Q = q,
                DateFrom = dateFrom,
                DateTo = dateTo,
                SupplierId = supplierId,
                WarehouseId = warehouseId
            }
        };

        await FillLookups(vm.Filter);

        var invoices = await _purchaseInvoiceRepository.SearchAsync(
            dateFrom, dateTo, supplierId, warehouseId, q);

        vm.Rows = invoices.Select(x => new FindPurchaseRowVm
        {
            PurchaseInvoiceId = x.Id,
            InvoiceNumber = x.InvoiceNumber,
            InvoiceDateUtc = x.InvoiceDateUtc,
            SupplierName = x.Supplier?.NameAr ?? x.SupplierName,
            WarehouseName = x.Warehouse?.WarehouseNameAr ?? "-",
            NetAmount = x.NetAmount,
            SupplierInvoiceNumber = x.SupplierInvoiceNumber,
            Notes = x.Notes
        }).ToList();

        vm.Count = vm.Rows.Count;
        vm.TotalNetAmount = vm.Rows.Sum(x => x.NetAmount);

        return View(vm);
    }

    private async Task FillLookups(FindPurchaseFilterVm vm)
    {
        var suppliers = await _supplierRepository.GetActiveAsync();
        var warehouses = await _warehouseRepository.GetActiveAsync();

        vm.Suppliers = suppliers.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.SupplierNumber} - {x.NameAr}"
        }).ToList();

        vm.Warehouses = warehouses.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.WarehouseCode} - {x.WarehouseNameAr}"
        }).ToList();
    }
}