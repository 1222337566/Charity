using InfrastrfuctureManagmentCore.Queries.Reports;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Reports
{
    public class DonationReportVm
    {
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        public List<DonationReportRowDto> Rows { get; set; } = new();
        public decimal TotalAmount => Rows.Sum(x => x.Amount);
    }
}
