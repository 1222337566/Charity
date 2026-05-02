using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.HR.Shifts
{
    public class CreateShiftVm
    {
        [Required, Display(Name = "اسم الشيفت")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "من")]
        public TimeSpan StartTime { get; set; }

        [Display(Name = "إلى")]
        public TimeSpan EndTime { get; set; }

        [Display(Name = "سماح بالدقائق")]
        public int GraceMinutes { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; } = true;
    }
}
