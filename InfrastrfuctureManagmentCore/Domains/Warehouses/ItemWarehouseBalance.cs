using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Warehouses
{
    public class ItemWarehouseBalance
    {
        public Guid ItemId { get; set; }
        public InfrastrfuctureManagmentCore.Domains.Item.Item? Item { get; set; }

        public Guid WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        public decimal QuantityOnHand { get; set; } = 0;   // الرصيد الفعلي
        public decimal ReservedQuantity { get; set; } = 0; // محجوز لاحقًا
        public decimal AvailableQuantity { get; set; } = 0; // متاح = OnHand - Reserved

        public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
        public Guid Id { get; set; }
    }
}

