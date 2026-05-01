using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Tracking
{
    public class CreateProjectTrackingLogVm
    {
        [Required]
        public Guid ProjectId { get; set; }
        [Display(Name = "المرحلة")]
        public Guid? ProjectPhaseId { get; set; }
        [Required, Display(Name = "تاريخ المتابعة")]
        public DateTime EntryDate { get; set; } = DateTime.Today;
        [Required, Display(Name = "نوع السجل")]
        public string EntryType { get; set; } = "Update";
        [Required, MaxLength(200), Display(Name = "العنوان")]
        public string Title { get; set; } = string.Empty;
        [Required, Display(Name = "التفاصيل")]
        public string Details { get; set; } = string.Empty;
        [Display(Name = "الحالة")]
        public string? Status { get; set; }
        [Range(0,100), Display(Name = "نسبة الإنجاز")]
        public decimal? ProgressPercent { get; set; }
        [Display(Name = "يحتاج انتباه")]
        public bool RequiresAttention { get; set; }
    }
}
