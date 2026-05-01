using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.CustomerDuesPayments
{
    public class CustomerDuesPaymentsPageVm
    {
        public Guid? CustomerId { get; set; }

        public List<CustomerDuesPaymentsRowVm> Rows { get; set; } = new();

        public CustomerDuesPaymentsRowVm? SelectedCustomer { get; set; }

        public List<CustomerInvoiceDueRowVm> Invoices { get; set; } = new();

        public List<CustomerReceiptRowVm> Receipts { get; set; } = new();

        public decimal TotalDue { get; set; }
    }

    public class CustomerDuesPaymentsRowVm
    {
        public Guid CustomerId { get; set; }

        public string CustomerNumber { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public string? MobileNo { get; set; }

        public decimal Invoiced { get; set; }

        public decimal Received { get; set; }

        public decimal Due => Invoiced - Received;
    }

    public class CustomerInvoiceDueRowVm
    {
        public Guid Id { get; set; }

        public string InvoiceNumber { get; set; } = string.Empty;

        public DateTime InvoiceDateUtc { get; set; }

        public decimal NetAmount { get; set; }

        public decimal Received { get; set; }

        public decimal Due => NetAmount - Received;
    }

    public class CustomerReceiptRowVm
    {
        public Guid Id { get; set; }

        public string ReceiptNumber { get; set; } = string.Empty;

        public DateTime ReceiptDateUtc { get; set; }

        public decimal Amount { get; set; }

        public string? PaymentMethodName { get; set; }

        public string? InvoiceNumber { get; set; }

        public string StatusText { get; set; } = string.Empty;

        public string? Notes { get; set; }
    }

    public class CustomerReceiveVm
    {
        [Required]
        public Guid CustomerId { get; set; }

        public string CustomerNumber { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public decimal CurrentDue { get; set; }

        [Display(Name = "الفاتورة المرتبطة")]
        public Guid? SalesInvoiceId { get; set; }

        [Required]
        [Display(Name = "رقم سند القبض")]
        public string ReceiptNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "تاريخ السند")]
        public DateTime ReceiptDateUtc { get; set; } = DateTime.UtcNow;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "المبلغ يجب أن يكون أكبر من صفر")]
        [Display(Name = "المبلغ")]
        public decimal Amount { get; set; }

        public decimal SuggestedAmount { get; set; }

        [Display(Name = "طريقة الدفع")]
        public Guid? PaymentMethodId { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> PaymentMethods { get; set; } = new();

        public List<SelectListItem> Invoices { get; set; } = new();
    }
}
