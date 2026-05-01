using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Phases
{
    public class CreateProjectPhaseVm
    {
        [Required]
        public Guid ProjectId { get; set; }
        [Display(Name = "الكود")]
        public string? Code { get; set; }
        [Required, MaxLength(200), Display(Name = "اسم المرحلة")]
        public string Name { get; set; } = string.Empty;
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
        [Range(0,100), Display(Name = "نسبة الإنجاز")]
        public decimal ProgressPercent { get; set; }
        [Display(Name = "تكلفة مخططة")]
        public decimal PlannedCost { get; set; }
        [Display(Name = "تكلفة فعلية")]
        public decimal ActualCost { get; set; }
        [Display(Name = "مسؤول المتابعة")]
        public string? ResponsiblePersonName { get; set; }
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }
        [Display(Name = "مفعل")]
        public bool IsActive { get; set; } = true;
    }
}
