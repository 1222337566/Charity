using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Milestones
{
    public class CreateProjectPhaseMilestoneVm
    {
        [Required]
        public Guid ProjectPhaseId { get; set; }
        [Required, MaxLength(200), Display(Name = "عنوان الـ milestone")]
        public string Title { get; set; } = string.Empty;
        [Display(Name = "الوصف")]
        public string? Description { get; set; }
        [Required, Display(Name = "تاريخ الاستحقاق")]
        public DateTime DueDate { get; set; } = DateTime.Today;
        [Display(Name = "تاريخ الإنجاز")]
        public DateTime? CompletedDate { get; set; }
        [Required, Display(Name = "الحالة")]
        public string Status { get; set; } = "Pending";
        [Range(0,100), Display(Name = "نسبة الإنجاز")]
        public decimal ProgressPercent { get; set; }
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }
        [Display(Name = "مفعل")]
        public bool IsActive { get; set; } = true;
    }
}
