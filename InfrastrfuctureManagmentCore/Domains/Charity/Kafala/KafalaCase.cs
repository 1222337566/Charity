using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastrfuctureManagmentCore.Domains.Payments;

namespace InfrastrfuctureManagmentCore.Domains.Charity.Kafala
{
    public class KafalaCase
    {
        public Guid Id { get; set; }
        public string CaseNumber { get; set; } = string.Empty;

        public Guid SponsorId { get; set; }
        public KafalaSponsor? Sponsor { get; set; }

        public Guid BeneficiaryId { get; set; }
        public Beneficiary? Beneficiary { get; set; }

        public string SponsorshipType { get; set; } = "Orphan"; // Orphan / Monthly / Health / Education / Family
        public string Frequency { get; set; } = "Monthly"; // Monthly / Quarterly / SemiAnnual / Annual
        public decimal MonthlyAmount { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? LastCollectionDate { get; set; }
        public DateTime? LastDisbursementDate { get; set; }
        public DateTime? NextDueDate { get; set; }

        public Guid? PaymentMethodId { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }

        public Guid? FinancialAccountId { get; set; }
        public FinancialAccount? FinancialAccount { get; set; }

        public string Status { get; set; } = "Active"; // Draft / Active / Suspended / Closed
        public bool AutoIncludeInAidCycles { get; set; } = true;
        public string? Notes { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<KafalaPayment> Payments { get; set; } = new List<KafalaPayment>();
    }
}
