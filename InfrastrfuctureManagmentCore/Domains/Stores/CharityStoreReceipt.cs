using InfrastrfuctureManagmentCore.Domains.Warehouses;

namespace InfrastrfuctureManagmentCore.Domains.Charity.Stores
{
    public class CharityStoreReceipt
    {
        public Guid Id { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public Guid WarehouseId { get; set; }
        public Guid? SourceId { get; set; }
        public Warehouse? Warehouse { get; set; }
        public DateTime ReceiptDate { get; set; } = DateTime.Today;
        public string? SourceType { get; set; } = "Purchase";
        public string? SourceName { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public string? UpdatedByUserId { get; set; }
        public string? UpdatedByUserName { get; set; }

        public ICollection<CharityStoreReceiptLine> Lines { get; set; } = new List<CharityStoreReceiptLine>();
        public string? ApprovalStatus { get; set; }
        public string? ApprovedByUserId { get; set; }
        public string? ApprovedByUserName { get; set; }
        public string? RejectedByUserId { get; set; }
        public string? RejectedByUserName { get; set; }
        public string? ApprovalNotes { get; set; }
        public DateTime? ApprovedAtUtc { get; set; }
        public DateTime? RejectedAtUtc { get; set; }
    }
}
