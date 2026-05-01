using System;
using System.Collections.Generic;

namespace InfrastructureManagmentWebFramework.Models.SupplierDuesPayments
{
    public class SupplierDuesPaymentsPageVm
    {
        public Guid? SupplierId { get; set; }

        public List<SupplierDuesPaymentsRowVm> Rows { get; set; } = new();

        public SupplierDuesPaymentsRowVm? SelectedSupplier { get; set; }

        public List<SupplierInvoiceDueRowVm> Invoices { get; set; } = new();

        public List<SupplierPaymentRowVm> Payments { get; set; } = new();

        public decimal TotalDue { get; set; }
    }

    public class SupplierDuesPaymentsRowVm
    {
        public Guid SupplierId { get; set; }

        public string SupplierNumber { get; set; } = string.Empty;

        public string SupplierName { get; set; } = string.Empty;

        public string? MobileNo { get; set; }

        public decimal Invoiced { get; set; }

        public decimal Paid { get; set; }

        public decimal Due => Invoiced - Paid;
    }

    public class SupplierInvoiceDueRowVm
    {
        public Guid Id { get; set; }

        public string InvoiceNumber { get; set; } = string.Empty;

        public DateTime InvoiceDateUtc { get; set; }

        public decimal NetAmount { get; set; }

        public decimal Paid { get; set; }

        public decimal Due => NetAmount - Paid;
    }

    public class SupplierPaymentRowVm
    {
        public Guid Id { get; set; }

        public string PaymentNumber { get; set; } = string.Empty;

        public DateTime PaymentDateUtc { get; set; }

        public decimal Amount { get; set; }

        public string? PaymentMethodName { get; set; }

        public string? InvoiceNumber { get; set; }

        public string StatusText { get; set; } = string.Empty;

        public string? Notes { get; set; }
    }
}
