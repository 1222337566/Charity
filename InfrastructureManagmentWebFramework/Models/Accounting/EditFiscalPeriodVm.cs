using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Accounting
{
    public class EditFiscalPeriodVm
    {
        public Guid Id { get; set; }

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
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "إلى تاريخ")]
        public DateTime EndDate { get; set; }

        [Display(Name = "هي الفترة الحالية")]
        public bool IsCurrent { get; set; }

        [Display(Name = "مقفلة")]
        public bool IsClosed { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }
    }
}
