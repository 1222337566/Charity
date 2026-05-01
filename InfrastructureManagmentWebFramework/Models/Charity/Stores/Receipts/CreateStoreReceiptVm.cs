using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.Stores.Receipts
{
    public class CreateStoreReceiptVm
    {
        [Required]
        [Display(Name = "رقم الإذن")]
        public string ReceiptNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "المخزن")]
        public Guid WarehouseId { get; set; }

        [Required]
        [Display(Name = "تاريخ الإضافة")]
        [DataType(DataType.Date)]
        public DateTime ReceiptDate { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "مصدر الإضافة")]
        public string SourceType { get; set; } = "Purchase";

        [Display(Name = "اسم المصدر")]
        public string? SourceName { get; set; }

        [Required]
        [Display(Name = "الصنف")]
        public Guid ItemId { get; set; }

        [Range(0.01, 999999999)]
        [Display(Name = "الكمية")]
        public decimal Quantity { get; set; }

        [Range(0, 999999999)]
        [Display(Name = "التكلفة للوحدة")]
        public decimal UnitCost { get; set; }

        [Display(Name = "تاريخ الصلاحية")]
        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "رقم التشغيلة")]
        public string? BatchNo { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> Warehouses { get; set; } = new();
        public List<SelectListItem> Items { get; set; } = new();

        public List<SelectListItem> SourceTypes { get; } = new()
        {
            new("شراء", "Purchase"),
            new("تبرع عيني", "Donation"),
            new("رصيد افتتاحي", "OpeningBalance"),
            new("تسوية زيادة", "AdjustmentIncrease"),
            new("مرتجع", "Return")
        };
    }
}
