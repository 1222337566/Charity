using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Stock
{
    public class StockBalanceListItemVm
    {
        public Guid ItemId { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string ItemNameAr { get; set; } = string.Empty;

        public Guid WarehouseId { get; set; }
        public string WarehouseNameAr { get; set; } = string.Empty;

        public decimal QuantityOnHand { get; set; }
        public decimal ReservedQuantity { get; set; }
        public decimal AvailableQuantity { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
    }
}
