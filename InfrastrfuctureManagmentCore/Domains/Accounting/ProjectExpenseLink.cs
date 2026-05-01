using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Domains.Expenses;

namespace InfrastrfuctureManagmentCore.Domains.Accounting
{
    public class ProjectExpenseLink
    {
        public Guid Id { get; set; }
        public Guid ExpenseId { get; set; }
        public Expensex? Expense { get; set; }

        public Guid ProjectId { get; set; }
        public CharityProject? Project { get; set; }

        public Guid? ProjectBudgetLineId { get; set; }
        public ProjectBudgetLine? ProjectBudgetLine { get; set; }

        public Guid? CostCenterId { get; set; }
        public CostCenter? CostCenter { get; set; }

        public string? Notes { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
