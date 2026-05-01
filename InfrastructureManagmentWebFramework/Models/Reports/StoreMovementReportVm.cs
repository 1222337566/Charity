using InfrastrfuctureManagmentCore.Queries.Reports;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Reports
{
    public class StoreMovementReportVm
    {
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        public List<StoreMovementReportRowDto> Rows { get; set; } = new();
        public decimal TotalReceivedQty => Rows.Sum(x => x.ReceiptQuantity);
        public decimal TotalIssuedQty => Rows.Sum(x => x.IssueQuantity);
    }
}
