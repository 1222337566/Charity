using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations
{
    public class CreateDonationInKindItemVm
    {
        public Guid DonationId { get; set; }

        [Required]
        [Display(Name = "الصنف")]
        public Guid? ItemId { get; set; }

        [Required]
        [Display(Name = "الكمية")]
        [Range(0.01, 999999999)]
        public decimal Quantity { get; set; }

        [Display(Name = "القيمة التقديرية للوحدة")]
        [Range(0, 999999999)]
        public decimal? EstimatedUnitValue { get; set; }

        [Display(Name = "إجمالي القيمة التقديرية")]
        [Range(0, 999999999)]
        public decimal? EstimatedTotalValue { get; set; }

        [Display(Name = "تاريخ الصلاحية")]
        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "رقم التشغيلة")]
        public string? BatchNo { get; set; }

        [Display(Name = "المخزن")]
        public Guid? WarehouseId { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> Items { get; set; } = new();
        public List<SelectListItem> Warehouses { get; set; } = new();
    }
}
