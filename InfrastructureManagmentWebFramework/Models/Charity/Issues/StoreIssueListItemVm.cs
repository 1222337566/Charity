namespace InfrastructureManagmentWebFramework.Models.Charity.Stores.Issues
{
    public class StoreIssueListItemVm
    {
        public Guid Id { get; set; }
        public string IssueNumber { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public string IssueType { get; set; } = string.Empty;
        public string? BeneficiaryName { get; set; }
        public string? ProjectName { get; set; }
        public string? IssuedToName { get; set; }
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
