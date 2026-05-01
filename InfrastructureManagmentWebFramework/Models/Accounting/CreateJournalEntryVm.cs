using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Accounting
{
    public class CreateJournalEntryVm
    {
        [Required]
        [Display(Name = "رقم القيد")]
        public string EntryNumber { get; set; } = $"JV-{DateTime.Now:yyyyMMddHHmmss}";

        [Required]
        [Display(Name = "تاريخ القيد")]
        [DataType(DataType.Date)]
        public DateTime EntryDate { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "الفترة المالية")]
        public Guid? FiscalPeriodId { get; set; }

        [Required]
        [Display(Name = "بيان القيد")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        [Display(Name = "نوع المصدر")]
        public string? SourceType { get; set; }

        [Display(Name = "معرف المصدر")]
        public Guid? SourceId { get; set; }

        public List<SelectListItem> FiscalPeriods { get; set; } = new();
    }
}
