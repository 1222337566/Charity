namespace InfrastructureManagmentWebFramework.Models.Charity.Stores.Receipts
{
    public class StoreReceiptDetailsVm
    {
        public Guid Id { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public DateTime ReceiptDate { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public string SourceType { get; set; } = string.Empty;
        public string? SourceName { get; set; }
        public string? Notes { get; set; }
        public string ApprovalStatus { get; set; } = string.Empty;
        public string ApprovalStatusName { get; set; } = string.Empty;
        public string? ApprovedByUserId { get; set; }
        public DateTime? ApprovedAtUtc { get; set; }
        public string? RejectedByUserId { get; set; }
        public DateTime? RejectedAtUtc { get; set; }
        public string? ApprovalNotes { get; set; }
        public bool CanApprove { get; set; }
        public bool CanReject { get; set; }
        public List<StoreReceiptLineVm> Lines { get; set; } = new();
    }

    public class StoreReceiptLineVm
    {
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Total => Quantity * UnitCost;
        public DateTime? ExpiryDate { get; set; }
        public string? BatchNo { get; set; }
        public string? Notes { get; set; }
    }
}
