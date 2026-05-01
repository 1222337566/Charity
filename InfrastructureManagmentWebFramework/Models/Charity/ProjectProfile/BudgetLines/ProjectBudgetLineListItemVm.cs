namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.BudgetLines
{
    public class ProjectBudgetLineListItemVm
    {
        public Guid Id { get; set; }
        public string LineName { get; set; } = string.Empty;
        public string LineType { get; set; } = string.Empty;
        public decimal PlannedAmount { get; set; }
        public decimal ActualAmount { get; set; }
        public string? Notes { get; set; }
    }
}
