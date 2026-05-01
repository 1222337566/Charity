using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations
{
    public class CreateDonationVm
    {
        public Guid DonorId { get; set; }

        [Required]
        [Display(Name = "رقم التبرع")]
        public string DonationNumber { get; set; } = string.Empty;

        [Display(Name = "تاريخ التبرع")]
        [DataType(DataType.Date)]
        public DateTime DonationDate { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "نوع التبرع")]
        public string DonationType { get; set; } = "نقدي";

        [Required(ErrorMessage = "نطاق توجيه التبرع مطلوب")]
        [Display(Name = "نطاق توجيه التبرع")]
        public string TargetingScopeCode { get; set; } = DonationTargetingScopeOption.SpecificRequests;

        [Display(Name = "نوع المساعدة المستهدف")]
        public Guid? AidTypeId { get; set; }

        [Display(Name = "القيمة")]
        [Range(0, 999999999)]
        public decimal? Amount { get; set; }

        [Display(Name = "طريقة الدفع")]
        public Guid? PaymentMethodId { get; set; }

        [Display(Name = "الحساب المالي")]
        public Guid? FinancialAccountId { get; set; }

        [Display(Name = "تبرع مخصص")]
        public bool IsRestricted { get; set; }

        [Display(Name = "اسم الحملة")]
        public string? CampaignName { get; set; }

        [Display(Name = "الغرض / الباب العام")]
        public string? GeneralPurposeName { get; set; }

        [Display(Name = "رقم الإيصال")]
        public string? ReceiptNumber { get; set; }

        [Display(Name = "رقم المرجع")]
        public string? ReferenceNumber { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        [Display(Name = "الصنف العيني")]
        public Guid? InKindItemId { get; set; }

        [Display(Name = "الكمية")]
        [Range(0.01, 999999999, ErrorMessage = "الكمية يجب أن تكون أكبر من صفر")]
        public decimal? InKindQuantity { get; set; }

        [Display(Name = "وصف الصنف")]
        public string? InKindItemDescription { get; set; }

        [Display(Name = "المخزن")]
        public Guid? InKindWarehouseId { get; set; }

        [Display(Name = "القيمة التقديرية للوحدة")]
        [Range(0, 999999999)]
        public decimal? InKindEstimatedUnitValue { get; set; }

        [Display(Name = "إجمالي القيمة التقديرية")]
        [Range(0, 999999999)]
        public decimal? InKindEstimatedTotalValue { get; set; }

        public List<SelectListItem> DonationTypes { get; set; } = new();
        public List<SelectListItem> TargetingScopes { get; set; } = new();
        public List<SelectListItem> AidTypes { get; set; } = new();
        public List<SelectListItem> PaymentMethods { get; set; } = new();
        public List<SelectListItem> FinancialAccounts { get; set; } = new();
        public List<SelectListItem> Items { get; set; } = new();
        public List<SelectListItem> Warehouses { get; set; } = new();
    }
}
