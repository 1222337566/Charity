namespace InfrastrfuctureManagmentCore.Queries.Reports
{
    public class StoreItemMovementDetailReportRowDto
    {
        public Guid? WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public Guid ItemId { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public decimal ReceivedQuantity { get; set; }
        public decimal IssuedQuantity { get; set; }
        public decimal NetQuantity => ReceivedQuantity - IssuedQuantity;
    }
}
