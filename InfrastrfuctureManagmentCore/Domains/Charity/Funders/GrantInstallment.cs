using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastrfuctureManagmentCore.Domains.Payments;

namespace InfrastrfuctureManagmentCore.Domains.Charity.Funders
{
    public class GrantInstallment
    {
        public Guid Id { get; set; }

        public Guid GrantAgreementId { get; set; }
        public GrantAgreement? GrantAgreement { get; set; }

        public int InstallmentNumber { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public decimal? ReceivedAmount { get; set; }
        public DateTime? ReceivedDate { get; set; }

        public Guid? PaymentMethodId { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }

        public Guid? FinancialAccountId { get; set; }
        public FinancialAccount? FinancialAccount { get; set; }

        public string Status { get; set; } = "Planned";
        public string? ReferenceNumber { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
        public bool IsPaid { get; set; }
    }
}
