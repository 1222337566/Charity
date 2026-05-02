using InfrastrfuctureManagmentCore.Domains.Warehouses;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Warehouses;
using InfrastructureManagmentServices.Stock;
using InfrastructureManagmentWebFramework.Models.Stock;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class StockBalancesController : Controller
{
    private readonly IItemWarehouseBalanceRepository _itemWarehouseBalanceRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IStockService _stockService;

    public StockBalancesController(
        IItemWarehouseBalanceRepository itemWarehouseBalanceRepository,
        IItemRepository itemRepository,
        IWarehouseRepository warehouseRepository,
        IStockService stockService)
    {
        _itemWarehouseBalanceRepository = itemWarehouseBalanceRepository;
        _itemRepository = itemRepository;
        _warehouseRepository = warehouseRepository;
        _stockService = stockService;
    }

    public async Task<IActionResult> Index()
    {
        var balances = await _itemWarehouseBalanceRepository.GetAllAsync();

        var model = balances.Select(x => new StockBalanceListItemVm
        {
            ItemId = x.ItemId,
            ItemCode = x.Item?.ItemCode ?? "",
            ItemNameAr = x.Item?.ItemNameAr ?? "",
            WarehouseId = x.WarehouseId,
            WarehouseNameAr = x.Warehouse?.WarehouseNameAr ?? "",
            QuantityOnHand = x.QuantityOnHand,
            ReservedQuantity = x.ReservedQuantity,
            AvailableQuantity = x.AvailableQuantity,
            LastUpdatedUtc = x.LastUpdatedUtc
        }).ToList();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> OpeningBalance()
    {
        var vm = new CreateOpeningBalanceVm
        {
            TransactionDateUtc = DateTime.UtcNow
        };

        await FillLookups(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OpeningBalance(CreateOpeningBalanceVm vm)
    {
        await FillLookups(vm);

        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            var request = new StockOperationRequest
            {
                ItemId = vm.ItemId,
                WarehouseId = vm.WarehouseId,
                Quantity = vm.Quantity,
                UnitCost = vm.UnitCost,
                TransactionDateUtc = vm.TransactionDateUtc ?? DateTime.UtcNow,
                ReferenceType = "OpeningBalance",
                ReferenceNumber = vm.ReferenceNumber,
                Notes = vm.Notes
            };

            await _stockService.AddOpeningBalanceAsync(request);

            TempData["Success"] = "تم إضافة الرصيد الافتتاحي بنجاح";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(vm);
        }
    }

    private async Task FillLookups(CreateOpeningBalanceVm vm)
    {
        var items = await _itemRepository.GetActiveAsync();
        var warehouses = await _warehouseRepository.GetActiveAsync();

        vm.Items = items
            .Where(x => !x.IsService && x.IsStockItem)
            .OrderBy(x => x.ItemCode)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.ItemCode} - {x.ItemNameAr}"
            })
            .ToList();

        vm.Warehouses = warehouses
            .OrderBy(x => x.WarehouseCode)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.WarehouseCode} - {x.WarehouseNameAr}"
            })
            .ToList();
    }
}