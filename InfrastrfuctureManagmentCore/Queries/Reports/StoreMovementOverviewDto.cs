namespace InfrastrfuctureManagmentCore.Queries.Reports
{
    public class StoreMovementOverviewDto
    {
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public int ReceiptsCount { get; set; }
        public decimal ReceiptQuantity { get; set; }
        public int IssuesCount { get; set; }
        public decimal IssueQuantity { get; set; }
    }
}
