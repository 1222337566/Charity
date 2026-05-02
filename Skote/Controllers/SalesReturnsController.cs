using InfrastrfuctureManagmentCore.Persistence.Repositories.Sale;
using InfrastrfuctureManagmentCore.Persistence.Repositories.salesreturn;
using InfrastructureManagmentServices.Salesreturn;
using InfrastructureManagmentWebFramework.Models.Salesreturn;
using Microsoft.AspNetCore.Mvc;

public class SalesReturnsController : Controller
{
    private readonly ISalesInvoiceRepository _salesInvoiceRepository;
    private readonly ISalesReturnRepository _salesReturnRepository;
    private readonly ISalesReturnService _salesReturnService;

    public SalesReturnsController(
        ISalesInvoiceRepository salesInvoiceRepository,
        ISalesReturnRepository salesReturnRepository,
        ISalesReturnService salesReturnService)
    {
        _salesInvoiceRepository = salesInvoiceRepository;
        _salesReturnRepository = salesReturnRepository;
        _salesReturnService = salesReturnService;
    }

    public async Task<IActionResult> Index()
    {
        var returns = await _salesReturnRepository.GetAllAsync();
        return View(returns);
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid salesInvoiceId)
    {
        var invoice = await _salesInvoiceRepository.GetByIdAsync(salesInvoiceId);
        if (invoice == null)
            return NotFound();

        var vm = new CreateSalesReturnVm
        {
            ReturnNumber = "SR-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
            ReturnDateUtc = DateTime.UtcNow,
            OriginalSalesInvoiceId = invoice.Id,
            OriginalInvoiceNumber = invoice.InvoiceNumber,
            CustomerName = invoice.CustomerName,
            WarehouseId = invoice.WarehouseId,
            WarehouseName = invoice.Warehouse?.WarehouseNameAr ?? "-",
            Lines = new List<SalesReturnLineVm>()
        };

        foreach (var line in invoice.Lines)
        {
            vm.Lines.Add(new SalesReturnLineVm
            {
                OriginalSalesInvoiceLineId = line.Id,
                ItemId = line.ItemId,
                ItemCode = line.Item?.ItemCode ?? "",
                ItemNameAr = line.Item?.ItemNameAr ?? "",
                SoldQuantity = line.Quantity,
                AlreadyReturnedQuantity = await _salesReturnRepository.GetReturnedQtyForOriginalLineAsync(line.Id),
                RemainingQuantity = line.Quantity - await _salesReturnRepository.GetReturnedQtyForOriginalLineAsync(line.Id),
                UnitPrice = line.UnitPrice,
                DiscountAmount = line.DiscountAmount,
                TaxAmount = line.TaxAmount,
                ReturnQuantity = 0
            });
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSalesReturnVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            var returnId = await _salesReturnService.CreateAsync(vm);
            TempData["Success"] = "تم حفظ مرتجع المبيعات بنجاح";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(vm);
        }
    }
}