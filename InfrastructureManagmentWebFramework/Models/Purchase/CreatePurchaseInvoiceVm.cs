using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Purchase
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.ComponentModel.DataAnnotations;

    public class CreatePurchaseInvoiceVm
    {
        [Required]
        [Display(Name = "رقم الفاتورة")]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "تاريخ الفاتورة")]
        public DateTime InvoiceDateUtc { get; set; } = DateTime.UtcNow;
        public Guid? SupplierId { get; set; }
        [Required]
        [Display(Name = "اسم المورد")]
        public string SupplierName { get; set; } = string.Empty;

        [Display(Name = "رقم فاتورة المورد")]
        public string? SupplierInvoiceNumber { get; set; }

        [Required]
        [Display(Name = "المخزن")]
        public Guid WarehouseId { get; set; }

        [Display(Name = "تصنيف العملية")]
        public string? ProcurementCategory { get; set; }

        [Display(Name = "المشروع المرتبط")]
        public Guid? ProjectId { get; set; }

        [Display(Name = "اتفاقية التمويل")]
        public Guid? GrantAgreementId { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> Warehouses  { get; set; } = new();
        public List<SelectListItem> Suppliers   { get; set; } = new();
        public List<SelectListItem> Projects { get; set; } = new();
        public List<SelectListItem> GrantAgreements { get; set; } = new();

        public List<PurchaseInvoiceLineVm> Lines { get; set; } = new();
    }
}
