using InfrastrfuctureManagmentCore.Queries.Reports;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Reports
{
    public class BeneficiaryAidDetailReportVm
    {
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        public List<BeneficiaryAidDetailReportRowDto> Rows { get; set; } = new();
        public decimal TotalRequested => Rows.Sum(x => x.RequestedAmount);
        public decimal TotalDisbursed => Rows.Sum(x => x.DisbursedAmount);
    }
}
