using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.FunderProfile.GrantAgreements
{
    public class CreateGrantAgreementVm
    {
        public Guid FunderId { get; set; }

        [Display(Name = "رقم الاتفاقية")]
        [Required]
        public string AgreementNumber { get; set; } = string.Empty;

        [Display(Name = "عنوان الاتفاقية")]
        [Required]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "الوصف")]
        public string? Description { get; set; }

        [Display(Name = "تاريخ الاتفاقية")]
        [DataType(DataType.Date)]
        public DateTime AgreementDate { get; set; } = DateTime.Today;

        [Display(Name = "تاريخ البداية")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "تاريخ النهاية")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Display(Name = "إجمالي التمويل")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "العملة")]
        public string Currency { get; set; } = "EGP";

        [Display(Name = "شروط الدفع")]
        public string? PaymentTerms { get; set; }

        [Display(Name = "متطلبات التقارير")]
        public string? ReportingRequirements { get; set; }

        [Display(Name = "الحالة")]
        public string Status { get; set; } = "Draft";

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> Statuses { get; set; } = new();
    }
}
