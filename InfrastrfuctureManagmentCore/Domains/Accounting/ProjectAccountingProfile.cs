using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Domains.Financial;

namespace InfrastrfuctureManagmentCore.Domains.Accounting
{
    public class ProjectAccountingProfile
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public CharityProject? Project { get; set; }

        public Guid? DefaultCostCenterId { get; set; }
        public CostCenter? DefaultCostCenter { get; set; }

        public Guid? DefaultRevenueAccountId { get; set; }
        public FinancialAccount? DefaultRevenueAccount { get; set; }

        public Guid? DefaultExpenseAccountId { get; set; }
        public FinancialAccount? DefaultExpenseAccount { get; set; }

        public bool AutoTagJournalLinesWithProject { get; set; } = true;
        public bool AutoUseProjectCostCenter { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
