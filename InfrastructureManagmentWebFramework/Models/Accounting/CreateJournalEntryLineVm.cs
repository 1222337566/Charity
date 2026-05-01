using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Accounting
{
    public class CreateJournalEntryLineVm
    {
        [Required]
        public Guid JournalEntryId { get; set; }

        [Required]
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
        public decimal DebitAmount { get; set; }

        [Display(Name = "دائن")]
        [Range(0, 999999999)]
        public decimal CreditAmount { get; set; }

        public List<SelectListItem> Accounts { get; set; } = new();
        public List<SelectListItem> CostCenters { get; set; } = new();
    }
}
