using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastrfuctureManagmentCore.Domains.Payments;

namespace InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries
{
    public class BeneficiaryAidDisbursement
    {
        public Guid Id { get; set; }
        public Guid BeneficiaryId { get; set; }
        public Beneficiary? Beneficiary { get; set; }

        public Guid? AidRequestId { get; set; }
        public BeneficiaryAidRequest? AidRequest { get; set; }

        public Guid AidTypeId { get; set; }
        public AidTypeLookup? AidType { get; set; }

        public DateTime DisbursementDate { get; set; }
        public decimal? Amount { get; set; }

        public Guid? PaymentMethodId { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }

        public Guid? FinancialAccountId { get; set; }
        public FinancialAccount? FinancialAccount { get; set; }

        public Guid? StoreIssueId { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? DonationId { get; set; }
        public Guid? GrantAgreementId { get; set; }

        public string ApprovalStatus { get; set; } = "Approved"; // Pending | Approved | Rejected
        public DateTime? ApprovedAtUtc { get; set; }
        public string? ApprovedByUserId { get; set; }
        public string? ApprovedByUserName { get; set; }
        public DateTime? RejectedAtUtc { get; set; }
        public string? RejectedByUserId { get; set; }
        public string? RejectedByUserName { get; set; }

        public string ExecutionStatus { get; set; } = "FullyDisbursed"; // Available | PartiallyDisbursed | FullyDisbursed | Cancelled
        public decimal? ExecutedAmount { get; set; }
        public DateTime? ExecutedAtUtc { get; set; }
        public string? ExecutedByUserId { get; set; }
        public string? ExecutedByUserName { get; set; }

        public string? SourceType { get; set; }
        public Guid? SourceId { get; set; }

        public ICollection<BeneficiaryAidDisbursementFundingLine> FundingLines { get; set; } = new List<BeneficiaryAidDisbursementFundingLine>();

        public string? Notes { get; set; }
        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public string? UpdatedByUserId { get; set; }
        public string? UpdatedByUserName { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public Guid? AidRequestLineId { get; set; }
        public BeneficiaryAidRequestLine? AidRequestLine { get; set; }
    }
}
