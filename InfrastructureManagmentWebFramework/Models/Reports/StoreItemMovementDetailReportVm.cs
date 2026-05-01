using InfrastrfuctureManagmentCore.Queries.Reports;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Reports
{
    public class StoreItemMovementDetailReportVm
    {
        public Guid? WarehouseId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        public List<SelectListItem> Warehouses { get; set; } = new();
        public List<StoreItemMovementDetailReportRowDto> Rows { get; set; } = new();

        public decimal TotalReceived => Rows.Sum(x => x.ReceivedQuantity);
        public decimal TotalIssued => Rows.Sum(x => x.IssuedQuantity);
        public decimal TotalNet => Rows.Sum(x => x.NetQuantity);
    }
}
