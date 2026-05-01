using InfrastrfuctureManagmentCore.Domains.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Customer
{
    public class CustomerReceipt
    {
        public Guid Id { get; set; }

        public string ReceiptNumber { get; set; } = string.Empty;
        public DateTime ReceiptDateUtc { get; set; } = DateTime.UtcNow;

        public Guid CustomerId { get; set; }
        public CustomerClient? Customer { get; set; }

        public Guid? PaymentMethodId { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public Guid? SalesInvoiceId { get; set; }
        public InfrastrfuctureManagmentCore.Domains.Sale.SalesInvoice? SalesInvoice { get; set; }

        public decimal Amount { get; set; }

        public string? Notes { get; set; }

        public CustomerReceiptStatus Status { get; set; } = CustomerReceiptStatus.Draft;

        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public string? UpdatedByUserId { get; set; }
        public string? UpdatedByUserName { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
