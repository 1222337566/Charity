using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Domains.Expenses;

namespace InfrastrfuctureManagmentCore.Domains.Accounting
{
    public class ProjectPhaseExpenseLink
    {
        public Guid Id { get; set; }
        public Guid ExpenseId { get; set; }
        public Expensex? Expense { get; set; }

        public Guid ProjectId { get; set; }
        public CharityProject? Project { get; set; }

        public Guid ProjectPhaseId { get; set; }
        public ProjectPhase? ProjectPhase { get; set; }

        public Guid? ProjectBudgetLineId { get; set; }
        public ProjectBudgetLine? ProjectBudgetLine { get; set; }

        public Guid? CostCenterId { get; set; }
        public CostCenter? CostCenter { get; set; }

        public bool IncludeInActualCost { get; set; } = true;
        public string? Notes { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
