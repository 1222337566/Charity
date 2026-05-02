using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfrastructureManagmentWebFramework.Models.TreasuryBank
{
    public class TreasuryBankMovementVm
    {
        public Guid? FinancialAccountId { get; set; }
        public string? FinancialAccountName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal ClosingBalance { get; set; }

        public List<SelectListItem> CashBankAccounts { get; set; } = new();
        public List<TreasuryBankMovementRowVm> Rows { get; set; } = new();
    }

    public class TreasuryBankMovementRowVm
    {
        public DateTime EntryDate { get; set; }
        public string EntryNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SourceType { get; set; } = string.Empty;
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal RunningBalance { get; set; }
    }
}
