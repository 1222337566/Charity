
using System;
using System.Linq;
using System.Threading.Tasks;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.InventoryAccountingReconciliation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

// عدّل الـ using التالية حسب أسماء الـ Domains الفعلية في مشروعك
using InfrastrfuctureManagmentCore.Domains.Accounting;
//using InfrastrfuctureManagmentCore.Domains.Stock;
using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastrfuctureManagmentCore.Domains.Warehouses;

namespace Skote.Controllers
{
    [Authorize]
    public class InventoryAccountingReconciliationReportsController : Controller
    {
        private readonly AppDbContext _db;

        public InventoryAccountingReconciliationReportsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? asOfDate, Guid? inventoryAccountId)
        {
            var vm = new InventoryAccountingReconciliationPageVm
            {
                AsOfDate = asOfDate ?? DateTime.Today,
                InventoryAccountId = inventoryAccountId
            };

            await LoadInventoryAccountsAsync(vm);

            vm.Rows = await BuildInventoryRowsAsync(vm.AsOfDate);
            vm.StockValueFromInventory = vm.Rows.Sum(x => x.StockValue);

            if (vm.InventoryAccountId.HasValue)
            {
                var account = await _db.Set<FinancialAccount>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == vm.InventoryAccountId.Value);

                if (account != null)
                {
                    vm.InventoryAccountCode = account.AccountCode;
                    vm.InventoryAccountName = account.AccountNameAr;
                }

                vm.InventoryAccountBalance = await CalculateInventoryAccountBalanceAsync(
                    vm.InventoryAccountId.Value,
                    vm.AsOfDate);
            }

            return View(vm);
        }

        private async Task LoadInventoryAccountsAsync(InventoryAccountingReconciliationPageVm vm)
        {
            var accounts = await _db.Set<FinancialAccount>()
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    x.Category == AccountCategory.Asset &&
                    (
                        x.AccountNameAr.Contains("مخزون") ||
                        x.AccountNameAr.Contains("المخزون") ||
                        x.AccountCode.Contains("13")
                    ))
                .OrderBy(x => x.AccountCode)
                .ToListAsync();

            vm.InventoryAccounts = accounts
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"{x.AccountCode} - {x.AccountNameAr}",
                    Selected = vm.InventoryAccountId == x.Id
                })
                .ToList();
        }

        private async Task<System.Collections.Generic.List<InventoryAccountingReconciliationRowVm>> BuildInventoryRowsAsync(DateTime asOfDate)
        {
            var asOfEnd = asOfDate.Date.AddDays(1).AddTicks(-1);

            var txs = await _db.Set<StockTransaction>()
                .AsNoTracking()
                .Where(x => x.TransactionDateUtc <= asOfEnd)
                .ToListAsync();

            var groups = txs
                .GroupBy(x => new { x.ItemId, x.WarehouseId })
                .Select(g =>
                {
                    var inbound = g.Where(IsInboundTransaction).ToList();
                    var outbound = g.Where(IsOutboundTransaction).ToList();

                    var inQty = inbound.Sum(x => Math.Abs(x.Quantity));
                    var outQty = outbound.Sum(x => Math.Abs(x.Quantity));
                    var qty = inQty - outQty;

                    var inCost = inbound.Sum(x => Math.Abs(x.Quantity) * x.UnitCost);
                    var avgCost = inQty > 0 ? inCost / inQty : 0m;

                    return new InventoryAccountingReconciliationRowVm
                    {
                        ItemId = g.Key.ItemId,
                        WarehouseId = g.Key.WarehouseId,
                        QuantityOnHand = qty,
                        AverageCost = avgCost,
                        StockValue = qty * avgCost,
                        LastTransactionDateUtc = g.Max(x => x.TransactionDateUtc)
                    };
                })
                .Where(x => Math.Abs(x.QuantityOnHand) > 0.0001m)
                .OrderBy(x => x.ItemName)
                .ThenBy(x => x.WarehouseName)
                .ToList();

            return groups;
        }

        private async Task<decimal> CalculateInventoryAccountBalanceAsync(Guid accountId, DateTime asOfDate)
        {
            var asOfEnd = asOfDate.Date.AddDays(1).AddTicks(-1);

            var lines = await _db.Set<JournalEntryLine>()
                .AsNoTracking()
                .Include(x => x.JournalEntry)
                .Where(x =>
                    x.FinancialAccountId == accountId &&
                    x.JournalEntry.CreatedAtUtc <= asOfEnd &&
                    x.JournalEntry.Status != JournalEntryStatus.Reversed)
                .ToListAsync();

            return lines.Sum(x => x.DebitAmount - x.CreditAmount);
        }

        private static bool IsInboundTransaction(StockTransaction x)
        {
            var t = (x.TransactionType.ToString() ?? string.Empty).ToLowerInvariant();

            return t.Contains("purchase") ||
                   t.Contains("receipt") ||
                   t.Contains("in") ||
                   t.Contains("opening") ||
                   t.Contains("adjustmentin");
        }

        private static bool IsOutboundTransaction(StockTransaction x)
        {
            var t = (x.TransactionType.ToString() ?? string.Empty).ToLowerInvariant();

            return t.Contains("sale") ||
                   t.Contains("issue") ||
                   t.Contains("out") ||
                   t.Contains("adjustmentout");
        }
    }
}
