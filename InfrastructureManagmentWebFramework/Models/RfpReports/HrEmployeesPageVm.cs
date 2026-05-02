using InfrastructureManagmentWebFramework.Models.RfpReports;

namespace InfrastructureManagmentWebFramework.Models.RfpReports
{
    public class HrEmployeesPageVm
    {
        public RfpReportFilterVm Filter { get; set; } = new();
        public List<InfrastrfuctureManagmentCore.Domains.Charity.Reports.HrEmployeeReportRow> Rows { get; set; } = new();
    }
}
