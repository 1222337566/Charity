using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.POS
{
    
    public class PosSaleVm
    {
        public Guid? HoldId { get; set; }
        [Required]
        [Display(Name = "رقم الفاتورة")]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "تاريخ الفاتورة")]
        public DateTime InvoiceDateUtc { get; set; } = DateTime.UtcNow;
        public Guid? CustomerId { get; set; }
        [Required]
        [Display(Name = "العميل")]
        public string CustomerName { get; set; } = "عميل نقدي";

        [Required]
        [Display(Name = "المخزن")]
        public Guid WarehouseId { get; set; }

        [Required]
        [Display(Name = "طريقة الدفع")]
        public Guid PaymentMethodId { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal NetAmount { get; set; }

        public List<SelectListItem> Warehouses { get; set; } = new();
        public List<SelectListItem> PaymentMethods { get; set; } = new();
        public List<PosPaymentVm> Payments { get; set; } = new();
        public List<PosSaleLineVm> Lines { get; set; } = new();
    }
}
