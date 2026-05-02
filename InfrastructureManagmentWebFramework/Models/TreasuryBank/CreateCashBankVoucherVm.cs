using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfrastructureManagmentWebFramework.Models.TreasuryBank
{
    public class CreateCashBankVoucherVm
    {
        [Required]
        [Display(Name = "التاريخ")]
        public DateTime VoucherDate { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "حساب الخزينة / البنك")]
        public Guid FinancialAccountId { get; set; }

        [Required]
        [Display(Name = "الحساب المقابل")]
        public Guid OppositeAccountId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "المبلغ يجب أن يكون أكبر من صفر")]
        [Display(Name = "المبلغ")]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "البيان")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> CashBankAccounts { get; set; } = new();
        public List<SelectListItem> OppositeAccounts { get; set; } = new();
    }
}
