namespace InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.Documents
{
    public class BeneficiaryDocumentListItemVm
    {
        public Guid Id { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string? DocumentNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? FilePath { get; set; }
        public bool IsVerified { get; set; }
        public string? Notes { get; set; }
    }
}
