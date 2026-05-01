using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Accounting
{
    public class CreateFiscalPeriodVm
    {
        [Required]
        [Display(Name = "كود الفترة")]
        public string PeriodCode { get; set; } = string.Empty;

        [Required]
        [Display(Name = "اسم الفترة")]
        public string PeriodNameAr { get; set; } = string.Empty;

        [Display(Name = "الاسم الإنجليزي")]
        public string? PeriodNameEn { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "من تاريخ")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        [Display(Name = "إلى تاريخ")]
        public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(12).AddDays(-1);

        [Display(Name = "هي الفترة الحالية")]
        public bool IsCurrent { get; set; } = true;

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }
    }
}
