using InfrastrfuctureManagmentCore.Domains.Accounting.Reports;

namespace InfrastructureManagmentWebFramework.Models.AccountingReports
{
    public class AccountStatementPageVm
    {
        public AccountingReportFilterVm Filter { get; set; } = new();
        public AccountStatementResult? Statement { get; set; }
    }
}
