using System;
using System.ComponentModel.DataAnnotations;

namespace Skote.ViewModels.Charity.Beneficiaries
{
    public class CreateBeneficiaryCategoryAssignmentVm
    {
        [Required]
        public Guid BeneficiaryId { get; set; }

        [Required(ErrorMessage = "برجاء اختيار التصنيف")]
        [Display(Name = "التصنيف")]
        public Guid CategoryId { get; set; }

        [Display(Name = "المشروع")]
        public Guid? ProjectId { get; set; }

        [Display(Name = "النشاط")]
        public Guid? ProjectActivityId { get; set; }

        [Required]
        [Display(Name = "الحالة")]
        public string Status { get; set; } = "Waiting";

        [Display(Name = "تاريخ الإضافة")]
        public DateTime AssignedAtUtc { get; set; } = DateTime.UtcNow;

        [Display(Name = "ملاحظات")]
        [MaxLength(1000)]
        public string? Notes { get; set; }
    }
}
