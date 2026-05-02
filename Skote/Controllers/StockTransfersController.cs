using InfrastrfuctureManagmentCore.Domains.Warehouses;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastructureManagmentServices.Stock;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Skote.Controllers
{
    public class StockTransfersController : Controller
    {
        private readonly IStockService _stockService;
        private readonly IItemRepository _itemRepo;
        private readonly IWarehouseRepository _warehouseRepo;
        public StockTransfersController(IStockService stockService, IItemRepository itemRepo, IWarehouseRepository warehouseRepo)
        { _stockService = stockService; _itemRepo = itemRepo; _warehouseRepo = warehouseRepo; }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await FillLookups();
            return View(new StockTransferRequest { TransactionDateUtc = DateTime.UtcNow });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StockTransferRequest vm)
        {
            await FillLookups();
            if (!ModelState.IsValid) return View(vm);
            await _stockService.TransferAsync(vm);
            TempData["Success"] = "تم تنفيذ التحويل المخزني";
            return RedirectToAction("Index", "StockBalances");
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
