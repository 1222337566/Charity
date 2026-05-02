using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfrastrfuctureManagmentCore.Domains.Item;

namespace InfrastrfuctureManagmentCore.Domains.Warehouses
{
    public class StockTransaction
    {
        public Guid Id { get; set; }

        public Guid ItemId { get; set; }
        public InfrastrfuctureManagmentCore.Domains.Item.Item Item { get; set; }

        public Guid WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        public StockTransactionType TransactionType { get; set; }

        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }

        public DateTime TransactionDateUtc { get; set; } = DateTime.UtcNow;

        public string? ReferenceType { get; set; }   // PurchaseInvoice / SalesInvoice / Transfer / Adjustment
        public string? ReferenceNumber { get; set; } // رقم المستند
        public Guid? ReferenceId { get; set; }

        public string? Notes { get; set; }

        public Guid? RelatedWarehouseId { get; set; } // مفيد في التحويل بين مخزنين
        public Warehouse? RelatedWarehouse { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
