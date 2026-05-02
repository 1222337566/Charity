using InfrastrfuctureManagmentCore.Domains.Charity.Donors;

namespace InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries
{
    public class BeneficiaryAidDisbursementFundingLine
    {
        public Guid Id { get; set; }

        public Guid DisbursementId { get; set; }
        public BeneficiaryAidDisbursement? Disbursement { get; set; }

        public Guid DonationAllocationId { get; set; }
        public DonationAllocation? DonationAllocation { get; set; }

        public decimal AmountConsumed { get; set; }

        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
