using System;

namespace InfrastructureManagmentWebFramework.Models.InventoryValuation
{
    public class InventoryValuationRowVm
    {
        public Guid ItemId { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string ItemNameAr { get; set; } = string.Empty;

        public Guid WarehouseId { get; set; }
        public string WarehouseNameAr { get; set; } = string.Empty;

        public decimal QuantityOnHand { get; set; }
        public decimal AvailableQuantity { get; set; }
        public decimal AverageUnitCost { get; set; }
        public decimal InventoryValue { get; set; }

        public decimal LastPurchaseUnitCost { get; set; }
        public DateTime? LastTransactionDateUtc { get; set; }
    }
}
