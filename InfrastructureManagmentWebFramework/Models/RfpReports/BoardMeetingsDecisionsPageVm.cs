using InfrastructureManagmentWebFramework.Models.RfpReports;

namespace InfrastructureManagmentWebFramework.Models.RfpReports
{
    public class BoardMeetingsDecisionsPageVm
    {
        public RfpReportFilterVm Filter { get; set; } = new();
        public List<InfrastrfuctureManagmentCore.Domains.Charity.Reports.BoardDecisionReportRow> Rows { get; set; } = new();
    }
}
