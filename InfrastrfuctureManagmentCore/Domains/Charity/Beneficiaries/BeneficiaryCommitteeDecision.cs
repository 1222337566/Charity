using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;

namespace InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries
{
    public class BeneficiaryCommitteeDecision
    {
        public Guid Id { get; set; }
        public Guid BeneficiaryId { get; set; }
        public Beneficiary? Beneficiary { get; set; }

        public DateTime DecisionDate { get; set; }
        public string DecisionType { get; set; } = string.Empty;

        public Guid? ApprovedAidTypeId { get; set; }
        public AidTypeLookup? ApprovedAidType { get; set; }

        public decimal? ApprovedAmount { get; set; }
        public int? DurationInMonths { get; set; }
        public string? CommitteeNotes { get; set; }
        public string? ApprovedByUserId { get; set; }
        public string? ApprovedByUserName { get; set; }

        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public string? UpdatedByUserId { get; set; }
        public string? UpdatedByUserName { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public bool ApprovedStatus { get; set; }
    }
}
