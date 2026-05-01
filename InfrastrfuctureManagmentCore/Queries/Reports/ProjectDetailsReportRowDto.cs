namespace InfrastrfuctureManagmentCore.Queries.Reports
{
    public class ProjectDetailsReportRowDto
    {
        public Guid ProjectId { get; set; }
        public string ProjectCode { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
        public decimal Budget { get; set; }
        public decimal PlannedBudgetLines { get; set; }
        public decimal ActualBudgetLines { get; set; }
        public decimal AllocatedGrants { get; set; }
        public int BeneficiariesCount { get; set; }
        public int ActivitiesCount { get; set; }
        public decimal ExecutionPercentage { get; set; }
    }
}
