using InfrastrfuctureManagmentCore.Domains.Charity.StockAdvanced;
using InfrastrfuctureManagmentCore.Domains.Warehouses;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.StockAdvanced;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastructureManagmentServices.Stock;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Skote.Controllers
{
    public class StockReturnVouchersController : Controller
    {
        private readonly IStockReturnVoucherRepository _repo;
        private readonly IStockService _stockService;
        private readonly IItemRepository _itemRepo;
        private readonly IWarehouseRepository _warehouseRepo;

        public StockReturnVouchersController(IStockReturnVoucherRepository repo, IStockService stockService, IItemRepository itemRepo, IWarehouseRepository warehouseRepo)
        { _repo = repo; _stockService = stockService; _itemRepo = itemRepo; _warehouseRepo = warehouseRepo; }

        public async Task<IActionResult> Index() => View(await _repo.GetAllAsync());

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await FillLookups();
            return View(new StockReturnVoucher { VoucherDate = DateTime.UtcNow.Date, Lines = new List<StockReturnVoucherLine> { new() } });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StockReturnVoucher model)
        {
            await FillLookups();
            if (!ModelState.IsValid) return View(model);
            model.Id = Guid.NewGuid();
            model.VoucherNumber = $"RT-{DateTime.UtcNow:yyyyMMddHHmmss}";
            foreach (var line in model.Lines)
            {
                line.Id = Guid.NewGuid();
                line.StockReturnVoucherId = model.Id;
                await _stockService.AddSaleReturnAsync(new StockOperationRequest
                {
                    ItemId = line.ItemId,
                    WarehouseId = model.WarehouseId,
                    Quantity = line.Quantity,
                    UnitCost = line.UnitCost,
                    TransactionDateUtc = model.VoucherDate,
                    ReferenceType = "StockReturnVoucher",
                    ReferenceNumber = model.VoucherNumber,
                    Notes = model.Notes
                });
            }
            await _repo.AddAsync(model);
            TempData["Success"] = "تم حفظ المرتجع المخزني";
            return RedirectToAction(nameof(Index));
        }

        private async Task FillLookups()
        {
            ViewBag.Items = (await _itemRepo.GetActiveAsync()).Where(x => x.IsStockItem && !x.IsService)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = $"{x.ItemCode} - {x.ItemNameAr}" }).ToList();
            ViewBag.Warehouses = (await _warehouseRepo.GetActiveAsync())
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = $"{x.WarehouseCode} - {x.WarehouseNameAr}" }).ToList();
        }
    }
}
