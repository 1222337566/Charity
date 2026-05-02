using InfrastrfuctureManagmentCore.Domains.Sale;
using InfrastrfuctureManagmentCore.Domains.Warehouses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.SalesReturn
{
    public class SalesReturnInvoice
    {
        public Guid Id { get; set; }

        public string ReturnNumber { get; set; } = string.Empty;
        public DateTime ReturnDateUtc { get; set; } = DateTime.UtcNow;

        public Guid OriginalSalesInvoiceId { get; set; }
        public SalesInvoice? OriginalSalesInvoice { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public Guid WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal NetAmount { get; set; }

        public SalesReturnStatus Status { get; set; } = SalesReturnStatus.Draft;

        public string? Notes { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<SalesReturnLine> Lines { get; set; } = new List<SalesReturnLine>();
    }
}
