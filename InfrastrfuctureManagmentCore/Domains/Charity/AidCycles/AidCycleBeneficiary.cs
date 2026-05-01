using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;

namespace InfrastrfuctureManagmentCore.Domains.Charity.AidCycles
{
    public class AidCycleBeneficiary
    {
        public Guid Id { get; set; }

        public Guid AidCycleId { get; set; }
        public AidCycle? AidCycle { get; set; }

        public Guid BeneficiaryId { get; set; }
        public Beneficiary? Beneficiary { get; set; }

        public Guid? CommitteeDecisionId { get; set; }
        public BeneficiaryCommitteeDecision? CommitteeDecision { get; set; }

        public Guid AidTypeId { get; set; }
        public AidTypeLookup? AidType { get; set; }

        public decimal? ScheduledAmount { get; set; }
        public decimal? ApprovedAmount { get; set; }
        public decimal? DisbursedAmount { get; set; }

        public string Status { get; set; } = "Eligible"; // Eligible / Deferred / Disbursed / Stopped / Cancelled / Pending
        public DateTime? LastDisbursementDate { get; set; }
        public DateTime? NextDueDate { get; set; }
        public Guid? DisbursementId { get; set; }

        public string? Notes { get; set; }
        public string? StopReason { get; set; }

        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}