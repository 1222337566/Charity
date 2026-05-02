namespace InfrastructureManagmentWebFramework.Models.Charity.Stores.Issues
{
    public class StoreIssueDetailsVm
    {
        public Guid Id { get; set; }
        public string IssueNumber { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public string IssueType { get; set; } = string.Empty;
        public string? BeneficiaryName { get; set; }
        public string? ProjectName { get; set; }
        public string? IssuedToName { get; set; }
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
        public List<StoreIssueLineVm> Lines { get; set; } = new();
    }

    public class StoreIssueLineVm
    {
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Total => Quantity * UnitCost;
        public string? Notes { get; set; }
    }
}
