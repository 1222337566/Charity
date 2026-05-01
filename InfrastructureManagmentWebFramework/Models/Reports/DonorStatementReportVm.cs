using InfrastrfuctureManagmentCore.Queries.Reports;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Reports
{
    public class DonorStatementReportVm
    {
        public Guid? DonorId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        public List<SelectListItem> Donors { get; set; } = new();
        public List<DonorStatementReportRowDto> Rows { get; set; } = new();

        public decimal TotalDonations => Rows.Sum(x => x.DonationAmount);
        public decimal TotalAllocated => Rows.Sum(x => x.AllocatedAmount);
        public decimal TotalRemaining => Rows.Sum(x => x.RemainingAmount);
    }
}
