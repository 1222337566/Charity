using InfrastrfuctureManagmentCore.Domains.Charity.Reports;

namespace InfrastructureManagmentWebFramework.Models.RfpReports
{
    public class ExecutiveDashboardVm
    {
        public RfpReportFilterVm Filter { get; set; } = new();
        public RfpDashboardSummary Summary { get; set; } = new();
    }
}
