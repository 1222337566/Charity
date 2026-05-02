namespace InfrastructureManagmentWebFramework.Models.Charity.Projects
{
    public class ProjectDetailsVm
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Budget { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? TargetBeneficiariesCount { get; set; }
        public string? Location { get; set; }
        public string? Objectives { get; set; }
        public string? Kpis { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public int BudgetLinesCount { get; set; }
        public int ActivitiesCount { get; set; }
        public int BeneficiariesCount { get; set; }
        public int GrantsCount { get; set; }
        public decimal PlannedBudgetLinesTotal { get; set; }
        public decimal ActualBudgetLinesTotal { get; set; }
    }
}
