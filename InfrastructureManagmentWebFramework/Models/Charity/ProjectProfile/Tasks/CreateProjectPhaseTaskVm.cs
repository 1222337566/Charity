using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Tasks
{
    public class CreateProjectPhaseTaskVm
    {
        [Required]
        public Guid ProjectId { get; set; }
        [Required]
        public Guid PhaseId { get; set; }
        [Required]
        public Guid ActivityId { get; set; }
        [Display(Name = "الكود")]
        public string? Code { get; set; }
        [Required, MaxLength(200), Display(Name = "اسم المهمة")]
        public string Title { get; set; } = string.Empty;
        [Display(Name = "الوصف")]
        public string? Description { get; set; }
        [Display(Name = "الترتيب")]
        public int SortOrder { get; set; } = 1;
        [Required, Display(Name = "الحالة")]
        public string Status { get; set; } = "Todo";
        [Required, Display(Name = "الأولوية")]
        public string Priority { get; set; } = "Medium";
        [Display(Name = "البداية المخططة")]
        public DateTime? PlannedStartDate { get; set; }
        [Display(Name = "تاريخ الاستحقاق")]
        public DateTime? DueDate { get; set; }
        [Range(0, 100), Display(Name = "نسبة الإنجاز")]
        public decimal PercentComplete { get; set; }
        [Display(Name = "ساعات مقدرة")]
        public decimal EstimatedHours { get; set; }
        [Display(Name = "ساعات منفذة")]
        public decimal SpentHours { get; set; }
        [Display(Name = "اسم المسؤول")]
        public string? AssignedToName { get; set; }
        [Display(Name = "معرّف المستخدم المسؤول")]
        public string? AssignedToUserId { get; set; }
        [Display(Name = "تتطلب متابعة يومية")]
        public bool RequiresDailyFollowUp { get; set; } = true;
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }
        [Display(Name = "مفعل")]
        public bool IsActive { get; set; } = true;
    }
}
