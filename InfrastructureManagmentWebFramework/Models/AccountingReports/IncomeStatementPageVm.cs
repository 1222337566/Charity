using InfrastrfuctureManagmentCore.Domains.Accounting.Reports;

namespace InfrastructureManagmentWebFramework.Models.AccountingReports
{
    public class IncomeStatementPageVm
    {
        public AccountingReportFilterVm Filter { get; set; } = new();
        public IncomeStatementResult Result { get; set; } = new();
    }
}
