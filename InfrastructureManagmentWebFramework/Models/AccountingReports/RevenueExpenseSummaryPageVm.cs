using InfrastrfuctureManagmentCore.Domains.Accounting.Reports;

namespace InfrastructureManagmentWebFramework.Models.AccountingReports
{
    public class RevenueExpenseSummaryPageVm
    {
        public AccountingReportFilterVm Filter { get; set; } = new();
        public RevenueExpenseSummaryResult Result { get; set; } = new();
    }
}
