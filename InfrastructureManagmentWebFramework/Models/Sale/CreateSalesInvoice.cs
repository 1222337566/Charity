using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace InfrastructureManagmentWebFramework.Models.Sale
{
  
    public class CreateSalesInvoiceVm
    {
        [Required]
        [Display(Name = "رقم الفاتورة")]
        public string InvoiceNumber { get; set; } = string.Empty;
        public Guid? CustomerId { get; set; }
        [Required]
        [Display(Name = "تاريخ الفاتورة")]
        public DateTime InvoiceDateUtc { get; set; } = DateTime.UtcNow;

        [Required]
        [Display(Name = "اسم العميل")]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "المخزن")]
        public Guid WarehouseId { get; set; }

        [Display(Name = "تصنيف الإيراد")]
        public string? RevenueCategory { get; set; }

        [Display(Name = "المشروع المرتبط")]
        public Guid? ProjectId { get; set; }

        [Display(Name = "اتفاقية التمويل")]
        public Guid? GrantAgreementId { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }
        public List<SalesPaymentVm> Payments { get; set; } = new();
        public List<SelectListItem> Warehouses { get; set; } = new();
        public List<SelectListItem> Projects { get; set; } = new();
        public List<SelectListItem> GrantAgreements { get; set; } = new();
        public List<SalesInvoiceLineVm> Lines { get; set; } = new();
        public Guid? PrescriptionId { get; set; }
        public List<SelectListItem> Prescriptions { get; set; } = new();
    }
}
