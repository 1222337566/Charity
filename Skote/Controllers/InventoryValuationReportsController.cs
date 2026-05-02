using System;
using System.Linq;
using System.Threading.Tasks;
using InfrastrfuctureManagmentCore.Domains.Warehouses;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.InventoryValuation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    [Authorize]
    public class InventoryValuationReportsController : Controller
    {
        private readonly AppDbContext _db;

        public InventoryValuationReportsController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(Guid? warehouseId = null, DateTime? asOfDateUtc = null)
        {
            var asOf = asOfDateUtc ?? DateTime.UtcNow;

            var warehouses = await _db.Warehouses
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.WarehouseNameAr)
                .ToListAsync();

            var balancesQuery = _db.ItemWarehouseBalances
                .AsNoTracking()
                .Include(x => x.Item)
                .Include(x => x.Warehouse)
                .Where(x => x.QuantityOnHand != 0 || x.AvailableQuantity != 0);

            if (warehouseId.HasValue && warehouseId.Value != Guid.Empty)
                balancesQuery = balancesQuery.Where(x => x.WarehouseId == warehouseId.Value);

            var balances = await balancesQuery
                .OrderBy(x => x.Warehouse!.WarehouseNameAr)
                .ThenBy(x => x.Item!.ItemCode)
                .ToListAsync();

            var rows = new System.Collections.Generic.List<InventoryValuationRowVm>();

            foreach (var balance in balances)
            {
                var averageCost = await CalculateMovingAverageUnitCostAsync(
                    balance.ItemId,
                    balance.WarehouseId,
                    asOf);

                var lastPurchaseCost = await _db.StockTransactions
                    .AsNoTracking()
                    .Where(x => x.ItemId == balance.ItemId
                        && x.WarehouseId == balance.WarehouseId
                        && x.TransactionType == StockTransactionType.Purchase
                        && x.UnitCost > 0
                        && x.TransactionDateUtc <= asOf)
                    .OrderByDescending(x => x.TransactionDateUtc)
                    .ThenByDescending(x => x.CreatedAtUtc)
                    .Select(x => x.UnitCost)
                    .FirstOrDefaultAsync();

                var lastTransactionDate = await _db.StockTransactions
                    .AsNoTracking()
                    .Where(x => x.ItemId == balance.ItemId
                        && x.WarehouseId == balance.WarehouseId
                        && x.TransactionDateUtc <= asOf)
                    .OrderByDescending(x => x.TransactionDateUtc)
                    .ThenByDescending(x => x.CreatedAtUtc)
                    .Select(x => (DateTime?)x.TransactionDateUtc)
                    .FirstOrDefaultAsync();

                rows.Add(new InventoryValuationRowVm
                {
                    ItemId = balance.ItemId,
                    ItemCode = balance.Item?.ItemCode ?? string.Empty,
                    ItemNameAr = balance.Item?.ItemNameAr ?? string.Empty,
                    WarehouseId = balance.WarehouseId,
                    WarehouseNameAr = balance.Warehouse?.WarehouseNameAr ?? string.Empty,
                    QuantityOnHand = balance.QuantityOnHand,
                    AvailableQuantity = balance.AvailableQuantity,
                    AverageUnitCost = averageCost,
                    InventoryValue = Math.Round(balance.QuantityOnHand * averageCost, 2),
                    LastPurchaseUnitCost = lastPurchaseCost,
                    LastTransactionDateUtc = lastTransactionDate
                });
            }

            var vm = new InventoryValuationPageVm
            {
                WarehouseId = warehouseId,
                AsOfDateUtc = asOf,
                Rows = rows,
                TotalQuantity = rows.Sum(x => x.QuantityOnHand),
                TotalValue = rows.Sum(x => x.InventoryValue),
                Warehouses = warehouses.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.WarehouseNameAr,
                    Selected = warehouseId.HasValue && warehouseId.Value == x.Id
                }).ToList()
            };

            return View(vm);
        }

        private static bool IsIncreaseType(StockTransactionType type)
        {
            return type == StockTransactionType.OpeningBalance
                || type == StockTransactionType.Purchase
                || type == StockTransactionType.SaleReturn
                || type == StockTransactionType.TransferIn
                || type == StockTransactionType.AdjustmentIncrease;
        }

        private static bool IsDecreaseType(StockTransactionType type)
        {
            return type == StockTransactionType.PurchaseReturn
                || type == StockTransactionType.Sale
                || type == StockTransactionType.TransferOut
                || type == StockTransactionType.AdjustmentDecrease;
        }

        private async Task<decimal> CalculateMovingAverageUnitCostAsync(Guid itemId, Guid warehouseId, DateTime asOfDateUtc)
        {
            var transactions = await _db.StockTransactions
                .AsNoTracking()
                .Where(x => x.ItemId == itemId
                    && x.WarehouseId == warehouseId
                    && x.TransactionDateUtc <= asOfDateUtc)
                .OrderBy(x => x.TransactionDateUtc)
                .ThenBy(x => x.CreatedAtUtc)
                .ToListAsync();

            decimal qty = 0;
            decimal value = 0;
            decimal lastKnownCost = 0;

            foreach (var trx in transactions)
            {
                if (IsIncreaseType(trx.TransactionType))
                {
                    var unitCost = trx.UnitCost > 0 ? trx.UnitCost : lastKnownCost;
                    qty += trx.Quantity;
                    value += trx.Quantity * unitCost;

                    if (unitCost > 0)
                        lastKnownCost = unitCost;
                }
                else if (IsDecreaseType(trx.TransactionType))
                {
                    var avgCost = qty > 0 ? value / qty : lastKnownCost;
                    var unitCost = trx.UnitCost > 0 ? trx.UnitCost : avgCost;

                    qty -= trx.Quantity;
                    value -= trx.Quantity * unitCost;

                    if (unitCost > 0)
                        lastKnownCost = unitCost;

                    if (qty <= 0)
                    {
                        qty = 0;
                        value = 0;
                    }
                }
            }

            if (qty > 0 && value > 0)
                return Math.Round(value / qty, 4);

            return Math.Round(lastKnownCost, 4);
        }
    }
}
