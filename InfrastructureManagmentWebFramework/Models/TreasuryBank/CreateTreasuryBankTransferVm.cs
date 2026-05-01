using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfrastructureManagmentWebFramework.Models.TreasuryBank
{
    public class CreateTreasuryBankTransferVm
    {
        [Required]
        [Display(Name = "التاريخ")]
        public DateTime TransferDate { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "من حساب")]
        public Guid FromFinancialAccountId { get; set; }

        [Required]
        [Display(Name = "إلى حساب")]
        public Guid ToFinancialAccountId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "المبلغ يجب أن يكون أكبر من صفر")]
        [Display(Name = "المبلغ")]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "البيان")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> CashBankAccounts { get; set; } = new();
    }
}
