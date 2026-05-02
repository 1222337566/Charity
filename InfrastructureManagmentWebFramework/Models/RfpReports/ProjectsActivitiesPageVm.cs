using InfrastructureManagmentWebFramework.Models.RfpReports;

namespace InfrastructureManagmentWebFramework.Models.RfpReports
{
    public class ProjectsActivitiesPageVm
    {
        public RfpReportFilterVm Filter { get; set; } = new();
        public List<InfrastrfuctureManagmentCore.Domains.Charity.Reports.ProjectActivityReportRow> Rows { get; set; } = new();
    }
}
