using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastrfuctureManagmentCore.Domains.Payments;

namespace InfrastrfuctureManagmentCore.Domains.Charity.Donors
{
    public class Donation
    {
        public Guid Id { get; set; }

        public string DonationNumber { get; set; } = string.Empty;

        public Guid DonorId { get; set; }
        public Donor? Donor { get; set; }

        public DateTime DonationDate { get; set; } = DateTime.UtcNow.Date;
        public string DonationType { get; set; } = "نقدي";

        public Guid? AidTypeId { get; set; }
        public AidTypeLookup? AidType { get; set; }

        public decimal? Amount { get; set; }

        public Guid? PaymentMethodId { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }

        public Guid? FinancialAccountId { get; set; }
        public FinancialAccount? FinancialAccount { get; set; }

        public bool IsRestricted { get; set; }
        public string? CampaignName { get; set; }

        // Explicit donation targeting fields to replace fallback based on
        // IsRestricted/CampaignName over time.
        public string TargetingScopeCode { get; set; } = "SpecificRequests";
        public string? GeneralPurposeName { get; set; }

        public string? ReceiptNumber { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Notes { get; set; }

        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public string? UpdatedByUserId { get; set; }
        public string? UpdatedByUserName { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
