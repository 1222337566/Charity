using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;

namespace InfrastrfuctureManagmentCore.Domains.Charity.Donors
{
    public class DonationAllocation
    {
        public Guid Id { get; set; }

        public Guid DonationId { get; set; }
        public Donation? Donation { get; set; }

        public DateTime AllocatedDate { get; set; } = DateTime.Today;

        public Guid? AidRequestId { get; set; }
        public BeneficiaryAidRequest? AidRequest { get; set; }

        public Guid? BeneficiaryId { get; set; }
        public Beneficiary? Beneficiary { get; set; }

        public Guid? DonationInKindItemId { get; set; }
        public DonationInKindItem? DonationInKindItem { get; set; }

        public decimal? AllocatedQuantity { get; set; }
        public decimal? Amount { get; set; }

        public string ApprovalStatus { get; set; } = "Pending"; // Pending | Approved | Rejected
        public DateTime? ApprovedAtUtc { get; set; }
        public string? ApprovedByUserId { get; set; }
        public DateTime? RejectedAtUtc { get; set; }
        public string? RejectedByUserId { get; set; }
        public string? ApprovalNotes { get; set; }

        public ICollection<BeneficiaryAidDisbursementFundingLine> DisbursementFundingLines { get; set; } = new List<BeneficiaryAidDisbursementFundingLine>();

        public string? Notes { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
        public string? UpdatedByUserId { get; set; }
        public string? UpdatedByUserName { get; set; }
        public Guid? AidRequestLineId { get; set; }
        public BeneficiaryAidRequestLine? AidRequestLine { get; set; }
    }
}
