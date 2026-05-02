using InfrastrfuctureManagmentCore.Queries.Reports;

namespace InfrastructureManagmentWebFramework.Models.Reports
{
    public class BeneficiaryStatusReportVm
    {
        public List<BeneficiaryStatusReportRowDto> Rows { get; set; } = new();
    }
}
