using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Funders;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Purchase;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Suppliers;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentServices.Purchase;
using InfrastructureManagmentWebFramework.Models.Purchase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class PurchaseInvoicesController : Controller
{
    private readonly IPurchaseInvoiceRepository _purchaseInvoiceRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IPurchaseService _purchaseService;
    private readonly ISupplierRepository _supplierRepository;
    private readonly ICharityProjectRepository _charityProjectRepository;
    private readonly IGrantAgreementRepository _grantAgreementRepository;
    private readonly AppDbContext _db;

    public PurchaseInvoicesController(
        IPurchaseInvoiceRepository purchaseInvoiceRepository,
        IWarehouseRepository warehouseRepository,
        IItemRepository itemRepository,
        IPurchaseService purchaseService,
        ISupplierRepository supplierRepository,
        ICharityProjectRepository charityProjectRepository,
        IGrantAgreementRepository grantAgreementRepository,
        AppDbContext db)
    {
        _purchaseInvoiceRepository = purchaseInvoiceRepository;
        _warehouseRepository = warehouseRepository;
        _itemRepository = itemRepository;
        _purchaseService = purchaseService;
        _supplierRepository = supplierRepository;
        _charityProjectRepository = charityProjectRepository;
        _grantAgreementRepository = grantAgreementRepository;
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var invoices = await _purchaseInvoiceRepository.GetAllAsync();
        return View(invoices);
    }
    [HttpGet]
    public async Task<IActionResult> CreateForSupplier(Guid supplierId, CancellationToken ct)
    {
        var supplier = await _supplierRepository.GetByIdAsync(supplierId);
        if (supplier == null)
            return NotFound();

        var vm = new CreatePurchaseInvoiceVm
        {
            InvoiceDateUtc = DateTime.UtcNow,
            InvoiceNumber = await GenerateInvoiceNumberAsync(ct),
            SupplierId = supplier.Id,
            SupplierName = supplier.NameAr,
            ProcurementCategory = "OperationalSupplies",
            Lines = new List<PurchaseInvoiceLineVm>
        {
            new PurchaseInvoiceLineVm()
        }
        };

        await FillLookups(vm);
        return View("Create", vm);
    }
    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var vm = new CreatePurchaseInvoiceVm
        {
            InvoiceDateUtc = DateTime.UtcNow,
            ProcurementCategory = "OperationalSupplies",
            Lines = new List<PurchaseInvoiceLineVm>
            {
                new PurchaseInvoiceLineVm()
            }
        };

        await FillLookups(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePurchaseInvoiceVm vm)
    {
        await FillLookups(vm);

        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            await _purchaseService.CreateAsync(vm);
            TempData["Success"] = "تم حفظ فاتورة الشراء وترحيل المخزون بنجاح";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(vm);
        }
    }

    private async Task<string> GenerateInvoiceNumberAsync(CancellationToken ct)
    {
        var year = DateTime.Now.Year;
        var month = DateTime.Now.Month;
        var count = await _db.Set<InfrastrfuctureManagmentCore.Domains.Purchase.PurchaseInvoice>()
            .CountAsync(x => x.InvoiceDateUtc.Year == year && x.InvoiceDateUtc.Month == month, ct);
        return $"PUR-{year}-{month:00}-{(count + 1):000}";
    }

    private async Task FillLookups(CreatePurchaseInvoiceVm vm)
    {
        var warehouses = await _warehouseRepository.GetActiveAsync();
        var items = await _itemRepository.GetActiveAsync();
        var suppliers = await _supplierRepository.GetAllAsync();
        vm.Suppliers = suppliers
            .OrderBy(x => x.NameAr)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.NameAr}" + (string.IsNullOrEmpty(x.NameEn) ? "" : $" / {x.NameEn}")
            }).ToList();

        var projects = await _charityProjectRepository.SearchAsync(null, null, true);
        var grantAgreements = await _grantAgreementRepository.GetAllAsync();

        vm.Warehouses = warehouses.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.WarehouseCode} - {x.WarehouseNameAr}"
        }).ToList();

        vm.Projects = projects.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.Code} - {x.Name}"
        }).ToList();

        vm.GrantAgreements = grantAgreements.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.AgreementNumber} - {x.Title}"
        }).ToList();

        foreach (var line in vm.Lines)
        {
            line.Items = items
                .Where(x => !x.IsService && x.IsStockItem)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"{x.ItemCode} - {x.ItemNameAr}"
                }).ToList();
        }
    }
}