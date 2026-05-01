using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Domains.Financial;

namespace InfrastrfuctureManagmentCore.Domains.Accounting.Integration
{
    /// <summary>
    /// Defines the accounting debit/credit template for an operational event.
    /// It is more specific than AccountingIntegrationProfile because donations
    /// need different posting rules by TargetingScopeCode and PurposeName.
    /// </summary>
    public class AccountingPostingProfile
    {
        public Guid Id { get; set; }

        public string Code { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string? NameEn { get; set; }
        public string? Description { get; set; }

        public string ModuleCode { get; set; } = string.Empty;
        public string EventCode { get; set; } = string.Empty;

        public string? DonationType { get; set; }
        public string? TargetingScopeCode { get; set; }
        public string? PurposeName { get; set; }

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
        public string? Notes { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
