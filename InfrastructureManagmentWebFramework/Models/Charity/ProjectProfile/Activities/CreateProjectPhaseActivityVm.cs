using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Activities
{
    public class CreateProjectPhaseActivityVm
    {
        [Required]
        public Guid ProjectId { get; set; }
        [Required]
        public Guid PhaseId { get; set; }
        [Display(Name = "الكود")]
        public string? Code { get; set; }
        [Required, MaxLength(200), Display(Name = "اسم النشاط")]
        public string Title { get; set; } = string.Empty;
        [Display(Name = "الوصف")]
        public string? Description { get; set; }
        [Display(Name = "الترتيب")]
        public int SortOrder { get; set; } = 1;
        [Required, Display(Name = "البداية المخططة")]
        public DateTime PlannedStartDate { get; set; } = DateTime.Today;
        [Required, Display(Name = "النهاية المخططة")]
        public DateTime PlannedEndDate { get; set; } = DateTime.Today.AddDays(7);
        [Display(Name = "البداية الفعلية")]
        public DateTime? ActualStartDate { get; set; }
        [Display(Name = "النهاية الفعلية")]
        public DateTime? ActualEndDate { get; set; }
        [Required, Display(Name = "الحالة")]
        public string Status { get; set; } = "Planned";
        [Required, Display(Name = "الأولوية")]
        public string Priority { get; set; } = "Medium";
        [Range(0, 100), Display(Name = "نسبة الإنجاز")]
        public decimal ProgressPercent { get; set; }
        [Display(Name = "ساعات مخططة")]
        public decimal PlannedHours { get; set; }
        [Display(Name = "ساعات فعلية")]
        public decimal ActualHours { get; set; }
        [Display(Name = "المسؤول")]
        public string? ResponsiblePersonName { get; set; }
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }
        [Display(Name = "مفعل")]
        public bool IsActive { get; set; } = true;
    }
}
