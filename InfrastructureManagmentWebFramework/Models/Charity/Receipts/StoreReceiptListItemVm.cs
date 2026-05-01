namespace InfrastructureManagmentWebFramework.Models.Charity.Stores.Receipts
{
    public class StoreReceiptListItemVm
    {
        public Guid Id { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public DateTime ReceiptDate { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public string SourceType { get; set; } = string.Empty;
        public string? SourceName { get; set; }
        public int LinesCount { get; set; }
        public decimal TotalAmount { get; set; }
        public string ApprovalStatus { get; set; } = string.Empty;
        public string ApprovalStatusName { get; set; } = string.Empty;
        public string? ApprovedByUserId { get; set; }
        public DateTime? ApprovedAtUtc { get; set; }
        public DateTime? RejectedAtUtc { get; set; }
        public string? ApprovalNotes { get; set; }
    }
}
