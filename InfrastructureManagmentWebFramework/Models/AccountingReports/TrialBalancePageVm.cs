using InfrastrfuctureManagmentCore.Domains.Accounting.Reports;

namespace InfrastructureManagmentWebFramework.Models.AccountingReports
{
    public class TrialBalancePageVm
    {
        public AccountingReportFilterVm Filter { get; set; } = new();
        public List<TrialBalanceRow> Rows { get; set; } = new();
        public decimal TotalDebit => Rows.Sum(x => x.TotalDebit);
        public decimal TotalCredit => Rows.Sum(x => x.TotalCredit);
    }
}
