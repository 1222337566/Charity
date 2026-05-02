using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.ComponentModel.DataAnnotations;

    public class CreateCustomerReceiptVm
    {
        [Required]
        [Display(Name = "رقم السند")]
        public string ReceiptNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "تاريخ السند")]
        public DateTime ReceiptDateUtc { get; set; } = DateTime.UtcNow;

        public Guid CustomerId { get; set; }

        [Display(Name = "رقم العميل")]
        public string CustomerNumber { get; set; } = string.Empty;

        [Display(Name = "اسم العميل")]
        public string CustomerName { get; set; } = string.Empty;

        [Display(Name = "الرصيد الحالي")]
        public decimal CurrentBalance { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        [Display(Name = "المبلغ")]
        public decimal Amount { get; set; }

        [Display(Name = "طريقة الدفع")]
        public Guid? PaymentMethodId { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        [Display(Name = "الفاتورة المرتبطة")]
        public Guid? SalesInvoiceId { get; set; }

        public List<SelectListItem> PaymentMethods { get; set; } = new();
    }
}
