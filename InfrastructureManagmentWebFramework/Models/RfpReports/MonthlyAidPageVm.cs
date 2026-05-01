using InfrastructureManagmentWebFramework.Models.RfpReports;

namespace InfrastructureManagmentWebFramework.Models.RfpReports
{
    public class MonthlyAidPageVm
    {
        public RfpReportFilterVm Filter { get; set; } = new();
        public List<InfrastrfuctureManagmentCore.Domains.Charity.Reports.MonthlyAidReportRow> Rows { get; set; } = new();
    }
}
