using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastrfuctureManagmentCore.Domains.Accounting;

namespace InfrastrfuctureManagmentCore.Domains.Accounting.Integration
{
    public class AccountingIntegrationProfile
    {
        public Guid Id { get; set; }
        public string SourceType { get; set; } = string.Empty;
        public string? EventCode { get; set; }
        public string? EventNameAr { get; set; }
        public string ProfileNameAr { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Optional matching conditions. Empty/null means: applies to all.
        public string? MatchDonationType { get; set; }
        public string? MatchTargetingScopeCode { get; set; }
        public string? MatchPurposeName { get; set; }
        public Guid? MatchAidTypeId { get; set; }
        public string? MatchStoreMovementType { get; set; }

        public int Priority { get; set; } = 100;

        public Guid? DebitAccountId { get; set; }
        public FinancialAccount? DebitAccount { get; set; }

        public Guid? CreditAccountId { get; set; }
        public FinancialAccount? CreditAccount { get; set; }

        public bool UseSourceFinancialAccountAsDebit { get; set; }
        public bool UseSourceFinancialAccountAsCredit { get; set; }

        public Guid? DefaultCostCenterId { get; set; }
        public CostCenter? DefaultCostCenter { get; set; }

        public bool AutoPost { get; set; } = true;
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
