using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.TaskUpdates
{
    public class CreateProjectTaskDailyUpdateVm
    {
        [Required]
        public Guid TaskId { get; set; }
        [Display(Name = "تاريخ المتابعة")]
        public DateTime UpdateDate { get; set; } = DateTime.Today;
        [Required, Display(Name = "الحالة")]
        public string Status { get; set; } = "InProgress";
        [Range(0,100), Display(Name = "نسبة الإنجاز")]
        public decimal ProgressPercent { get; set; }
        [Display(Name = "الساعات المنفذة")]
        public decimal HoursSpent { get; set; }
        [Display(Name = "وصف ما تم")]
        public string? Note { get; set; }
        [Display(Name = "المعوقات")]
        public string? BlockerNote { get; set; }
        [Display(Name = "الخطوة التالية")]
        public string? NextAction { get; set; }
    }
}
