using InfrastrfuctureManagmentCore.Domains.Customer;
using InfrastrfuctureManagmentCore.Domains.Prescriptions;
using InfrastrfuctureManagmentCore.Domains.Warehouses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Sale
{
    public class SalesInvoice
    {
        public Guid Id { get; set; }

        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDateUtc { get; set; } = DateTime.UtcNow;
        public Guid? PrescriptionId { get; set; }
        public Prescription? Prescription { get; set; }
        public Guid? CustomerId { get; set; }
        public CustomerClient? Customer { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public Guid WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal NetAmount { get; set; }

        public SalesInvoiceStatus Status { get; set; } = SalesInvoiceStatus.Draft;

        public string? Notes { get; set; }

        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public string? UpdatedByUserId { get; set; }
        public string? UpdatedByUserName { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<SalesInvoiceLine> Lines { get; set; } = new List<SalesInvoiceLine>();
        public ICollection<SalesInvoicePayment> Payments { get; set; } = new List<SalesInvoicePayment>();
    }
}
