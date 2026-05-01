namespace InfrastructureManagmentWebFramework.Models.AccountingProjects
{
    public class ProjectExpenseLinkListItemVm
    {
        public Guid Id { get; set; }
        public string ExpenseNumber { get; set; } = string.Empty;
        public DateTime ExpenseDateUtc { get; set; }
        public decimal Amount { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string? BudgetLineName { get; set; }
        public string? CostCenterName { get; set; }
        public string? Notes { get; set; }
    }
}
