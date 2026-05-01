using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Accounting
{
    public class CreateManualJournalEntryVm
    {
        [Required]
        [Display(Name = "رقم القيد")]
        public string EntryNumber { get; set; } = string.Empty;

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

        [Display(Name = "ترحيل القيد فور الحفظ")]
        public bool PostImmediately { get; set; }

        public List<CreateManualJournalEntryLineVm> Lines { get; set; } = new()
        {
            new CreateManualJournalEntryLineVm(),
            new CreateManualJournalEntryLineVm()
        };

        public List<SelectListItem> FiscalPeriods { get; set; } = new();
        public List<SelectListItem> Accounts { get; set; } = new();
        public List<SelectListItem> CostCenters { get; set; } = new();
        public List<SelectListItem> Projects { get; set; } = new();

        public decimal TotalDebit => Lines.Sum(x => x.DebitAmount ?? 0m);
        public decimal TotalCredit => Lines.Sum(x => x.CreditAmount ?? 0m);
    }

    public class CreateManualJournalEntryLineVm
    {
        [Display(Name = "الحساب")]
        public Guid? FinancialAccountId { get; set; }

        [Display(Name = "مركز التكلفة")]
        public Guid? CostCenterId { get; set; }

        [Display(Name = "المشروع")]
        public Guid? ProjectId { get; set; }

        [Display(Name = "البيان")]
        public string? Description { get; set; }

        [Display(Name = "مدين")]
        [Range(0, 999999999)]
        public decimal? DebitAmount { get; set; }

        [Display(Name = "دائن")]
        [Range(0, 999999999)]
        public decimal? CreditAmount { get; set; }
    }
}
