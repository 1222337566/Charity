
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfrastructureManagmentWebFramework.Models.InventoryAccountingReconciliation
{
    public class InventoryAccountingReconciliationRowVm
    {
        public Guid? ItemId { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }

        public Guid? WarehouseId { get; set; }
        public string? WarehouseName { get; set; }

        public decimal QuantityOnHand { get; set; }
        public decimal AverageCost { get; set; }
        public decimal StockValue { get; set; }

        public DateTime? LastTransactionDateUtc { get; set; }
    }

    public class InventoryAccountingReconciliationPageVm
    {
        public DateTime AsOfDate { get; set; } = DateTime.Today;

        public Guid? InventoryAccountId { get; set; }

        public string? InventoryAccountCode { get; set; }
        public string? InventoryAccountName { get; set; }

        public decimal StockValueFromInventory { get; set; }
        public decimal InventoryAccountBalance { get; set; }
        public decimal Difference => StockValueFromInventory - InventoryAccountBalance;

        public bool IsBalanced => Math.Abs(Difference) < 0.01m;

        public List<SelectListItem> InventoryAccounts { get; set; } = new();

        public List<InventoryAccountingReconciliationRowVm> Rows { get; set; } = new();
    }
}

