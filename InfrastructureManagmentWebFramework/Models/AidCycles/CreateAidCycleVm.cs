using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.AidCycles
{
    public class CreateAidCycleVm
    {
        [Required]
        [Display(Name = "رقم الدورة")]
        public string CycleNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "اسم الدورة")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "نوع الدورة")]
        public string CycleType { get; set; } = "Monthly";

        [Display(Name = "نوع المساعدة")]
        public Guid? AidTypeId { get; set; }

        [Display(Name = "السنة")]
        public int? PeriodYear { get; set; }

        [Display(Name = "الشهر")]
        public int? PeriodMonth { get; set; }

        [Display(Name = "من تاريخ")]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [Display(Name = "إلى تاريخ")]
        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        [Required]
        [Display(Name = "تاريخ الصرف المخطط")]
        [DataType(DataType.Date)]
        public DateTime PlannedDisbursementDate { get; set; } = DateTime.Today;

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> AidTypes { get; set; } = new();
        public List<SelectListItem> CycleTypes { get; set; } = new();
    }
}
