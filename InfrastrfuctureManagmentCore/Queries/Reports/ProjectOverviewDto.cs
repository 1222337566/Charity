namespace InfrastrfuctureManagmentCore.Queries.Reports
{
    public class ProjectOverviewDto
    {
        public Guid ProjectId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public decimal PlannedBudgetLines { get; set; }
        public decimal ActualBudgetLines { get; set; }
        public decimal AllocatedGrants { get; set; }
        public int BeneficiariesCount { get; set; }
        public int ActivitiesCount { get; set; }
    }
}
