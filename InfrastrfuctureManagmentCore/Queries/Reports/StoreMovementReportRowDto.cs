namespace InfrastrfuctureManagmentCore.Queries.Reports
{
    public class StoreMovementReportRowDto
    {
        public string WarehouseName { get; set; } = string.Empty;
        public int ReceiptsCount { get; set; }
        public decimal ReceiptQuantity { get; set; }
        public int IssuesCount { get; set; }
        public decimal IssueQuantity { get; set; }
    }
}
