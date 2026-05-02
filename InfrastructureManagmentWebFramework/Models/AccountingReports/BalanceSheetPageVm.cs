using InfrastrfuctureManagmentCore.Domains.Accounting.Reports;

namespace InfrastructureManagmentWebFramework.Models.AccountingReports
{
    public class BalanceSheetPageVm
    {
        public AccountingReportFilterVm Filter { get; set; } = new();
        public BalanceSheetResult Result { get; set; } = new();
    }
}
