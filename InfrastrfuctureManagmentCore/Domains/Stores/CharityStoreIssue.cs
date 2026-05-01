using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Domains.Warehouses;

namespace InfrastrfuctureManagmentCore.Domains.Charity.Stores
{
    public class CharityStoreIssue
    {
        public Guid Id { get; set; }
        public string IssueNumber { get; set; } = string.Empty;
        public Guid? SourceId { get; set; }
        public Guid WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }
        public DateTime IssueDate { get; set; } = DateTime.Today;
        public string IssueType { get; set; } = "Internal";

        public Guid? BeneficiaryId { get; set; }
        public Beneficiary? Beneficiary { get; set; }

        public Guid? ProjectId { get; set; }
        public CharityProject? Project { get; set; }

        public string? IssuedToName { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public string? UpdatedByUserId { get; set; }
        public string? UpdatedByUserName { get; set; }

        public ICollection<CharityStoreIssueLine> Lines { get; set; } = new List<CharityStoreIssueLine>();
        public string? ApprovalStatus { get; set; }
        public string? ApprovedByUserId { get; set; }
        public string? ApprovedByUserName { get; set; }
        public string? RejectedByUserId { get; set; }
        public string? RejectedByUserName { get; set; }
        public string? ApprovalNotes { get; set; }
        public DateTime? ApprovedAtUtc { get; set; }
        public DateTime? RejectedAtUtc { get; set; }
        public string? SourceType { get; set; }
    }
}
