using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Warehouses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Skote.Controllers
{
    public class StockItemCardController : Controller
    {
        private readonly IStockTransactionRepository _stockTransactionRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IWarehouseRepository _warehouseRepository;

        public StockItemCardController(IStockTransactionRepository stockTransactionRepository, IItemRepository itemRepository, IWarehouseRepository warehouseRepository)
        { _stockTransactionRepository = stockTransactionRepository; _itemRepository = itemRepository; _warehouseRepository = warehouseRepository; }

        [HttpGet]
        public async Task<IActionResult> Index(Guid? itemId, Guid? warehouseId)
        {
            ViewBag.Items = (await _itemRepository.GetActiveAsync()).Where(x => x.IsStockItem && !x.IsService)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = $"{x.ItemCode} - {x.ItemNameAr}" }).ToList();
            ViewBag.Warehouses = (await _warehouseRepository.GetActiveAsync())
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = $"{x.WarehouseCode} - {x.WarehouseNameAr}" }).ToList();

            if (itemId == null || warehouseId == null)
                return View(new List<InfrastrfuctureManagmentCore.Domains.Warehouses.StockTransaction>());

            var rows = await _stockTransactionRepository.GetByItemAndWarehouseAsync(itemId.Value, warehouseId.Value);
            return View(rows.OrderBy(x => x.TransactionDateUtc).ToList());
        }
    }
}
