namespace InfrastructureManagmentWebFramework.Models.AccountingProjects.Phases
{
    public class ProjectPhaseExpenseLinkListItemVm
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid PhaseId { get; set; }
        public string ExpenseNumber { get; set; } = string.Empty;
        public DateTime ExpenseDateUtc { get; set; }
        public decimal Amount { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string PhaseName { get; set; } = string.Empty;
        public string? BudgetLineName { get; set; }
        public string? CostCenterName { get; set; }
        public bool IncludeInActualCost { get; set; }
        public string? Notes { get; set; }
    }
}
