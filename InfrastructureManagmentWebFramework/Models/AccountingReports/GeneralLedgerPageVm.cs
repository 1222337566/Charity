using InfrastrfuctureManagmentCore.Domains.Accounting.Reports;

namespace InfrastructureManagmentWebFramework.Models.AccountingReports
{
    public class GeneralLedgerPageVm
    {
        public AccountingReportFilterVm Filter { get; set; } = new();
        public List<GeneralLedgerSummaryRow> Rows { get; set; } = new();
        public decimal OpeningBalanceTotal => Rows.Sum(x => x.OpeningBalance);
        public decimal PeriodDebitTotal => Rows.Sum(x => x.PeriodDebit);
        public decimal PeriodCreditTotal => Rows.Sum(x => x.PeriodCredit);
        public decimal ClosingBalanceTotal => Rows.Sum(x => x.ClosingBalance);
    }
}
