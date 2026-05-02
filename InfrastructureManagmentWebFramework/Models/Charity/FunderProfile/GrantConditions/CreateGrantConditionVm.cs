using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.FunderProfile.GrantConditions
{
    public class CreateGrantConditionVm
    {
        public Guid GrantAgreementId { get; set; }

        [Display(Name = "عنوان الشرط")]
        [Required]
        public string ConditionTitle { get; set; } = string.Empty;

        [Display(Name = "تفاصيل الشرط")]
        [Required]
        public string ConditionDetails { get; set; } = string.Empty;

        [Display(Name = "إلزامي")]
        public bool IsMandatory { get; set; }

        [Display(Name = "تاريخ الاستحقاق")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        [Display(Name = "تم التنفيذ")]
        public bool IsFulfilled { get; set; }

        [Display(Name = "تاريخ التنفيذ")]
        [DataType(DataType.Date)]
        public DateTime? FulfilledDate { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }
    }
}
