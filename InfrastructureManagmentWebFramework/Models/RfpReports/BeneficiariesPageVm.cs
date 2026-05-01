using InfrastructureManagmentWebFramework.Models.RfpReports;

namespace InfrastructureManagmentWebFramework.Models.RfpReports
{
    public class BeneficiariesPageVm
    {
        public RfpReportFilterVm Filter { get; set; } = new();
        public List<InfrastrfuctureManagmentCore.Domains.Charity.Reports.BeneficiaryReportRow> Rows { get; set; } = new();
    }
}
