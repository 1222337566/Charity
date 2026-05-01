namespace InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.AidDisbursements
{
    public class BeneficiaryAidDisbursementListItemVm
    {
        public Guid Id { get; set; }
        public Guid BeneficiaryId { get; set; }
        public string? BeneficiaryCode { get; set; }
        public string? BeneficiaryName { get; set; }
        public DateTime DisbursementDate { get; set; }
        public string? AidType { get; set; }
        public decimal? Amount { get; set; }
        public decimal? ExecutedAmount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? FinancialAccount { get; set; }
        public string? LinkedRequestStatus { get; set; }
        public string? ProjectName { get; set; }
        public string? DonationNumber { get; set; }
        public string? FundingTraceSummary { get; set; }
        public string? GrantAgreementNumber { get; set; }
        public bool IsJournalPosted { get; set; }
        public string? Notes { get; set; }
        public string? ApprovalStatusCode { get; set; }
        public string? ApprovalStatusName { get; set; }
        public DateTime? ApprovedAtUtc { get; set; }
        public DateTime? RejectedAtUtc { get; set; }
        public string? ExecutionStatusCode { get; set; }
        public string? ExecutionStatusName { get; set; }
        public DateTime? ExecutedAtUtc { get; set; }
        public bool CanExecuteCash { get; set; }
        public bool IsInKindDisbursement { get; set; }
        public bool CanCreateStoreIssue { get; set; }
        public Guid? StoreIssueId { get; set; }
        public string? StoreIssueNumber { get; set; }
     
    }
}
