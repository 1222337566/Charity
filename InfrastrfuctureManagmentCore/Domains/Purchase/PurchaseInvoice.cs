using InfrastrfuctureManagmentCore.Domains.Suppliers;
using InfrastrfuctureManagmentCore.Domains.Warehouses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Purchase
{
    public class PurchaseInvoice
    {
        public Guid Id { get; set; }

        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDateUtc { get; set; } = DateTime.UtcNow;

        public Guid? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        public string SupplierName { get; set; } = string.Empty;
        public string? SupplierInvoiceNumber { get; set; }

        public Guid WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal NetAmount { get; set; }

        public PurchaseInvoiceStatus Status { get; set; } = PurchaseInvoiceStatus.Draft;

        public string? Notes { get; set; }

        /// <summary>طلب الاحتياج المنشأ منه هذه الفاتورة</summary>
        public Guid? StockNeedRequestId { get; set; }

        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public string? UpdatedByUserId { get; set; }
        public string? UpdatedByUserName { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<PurchaseInvoiceLine> Lines { get; set; } = new List<PurchaseInvoiceLine>();
    }
}
