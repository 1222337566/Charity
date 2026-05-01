using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastrfuctureManagmentCore.Domains.Payments;

namespace InfrastrfuctureManagmentCore.Domains.Charity.Kafala
{
    public class KafalaPayment
    {
        public Guid Id { get; set; }
        public Guid KafalaCaseId { get; set; }
        public KafalaCase? KafalaCase { get; set; }

        public Guid SponsorId { get; set; }
        public KafalaSponsor? Sponsor { get; set; }

        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string PeriodLabel { get; set; } = string.Empty;
        public string Direction { get; set; } = "Received"; // Received / Disbursed / Adjustment

        public Guid? PaymentMethodId { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }

        public Guid? FinancialAccountId { get; set; }
        public FinancialAccount? FinancialAccount { get; set; }

        public Guid? AidCycleId { get; set; }
        public string? ReferenceNumber { get; set; }
        public string Status { get; set; } = "Confirmed"; // Draft / Confirmed / Reversed
        public string? Notes { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
