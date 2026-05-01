using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;

namespace InfrastrfuctureManagmentCore.Domains.Charity.AidCycles
{
    public class AidCycle
    {
        public Guid Id { get; set; }
        public string CycleNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string CycleType { get; set; } = "Monthly"; // Monthly / Seasonal / Emergency / Sponsorship

        public Guid? AidTypeId { get; set; }
        public AidTypeLookup? AidType { get; set; }

        public int? PeriodYear { get; set; }
        public int? PeriodMonth { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime PlannedDisbursementDate { get; set; }

        public string Status { get; set; } = "Draft"; // Draft / Generated / Approved / Disbursed / Closed / Cancelled
        public int BeneficiariesCount { get; set; }
        public decimal? TotalPlannedAmount { get; set; }
        public decimal? TotalDisbursedAmount { get; set; }

        public string? Notes { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public string? ApprovedByUserId { get; set; }
        public DateTime? ApprovedAtUtc { get; set; }
        public DateTime? ClosedAtUtc { get; set; }

        public ICollection<AidCycleBeneficiary> Beneficiaries { get; set; } = new List<AidCycleBeneficiary>();
    }
}
