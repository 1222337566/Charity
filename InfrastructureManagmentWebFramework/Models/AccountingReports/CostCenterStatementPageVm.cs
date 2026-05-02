using InfrastrfuctureManagmentCore.Domains.Accounting.Reports;

namespace InfrastructureManagmentWebFramework.Models.AccountingReports
{
    public class CostCenterStatementPageVm
    {
        public AccountingReportFilterVm Filter { get; set; } = new();
        public CostCenterStatementResult? Statement { get; set; }
    }
}
