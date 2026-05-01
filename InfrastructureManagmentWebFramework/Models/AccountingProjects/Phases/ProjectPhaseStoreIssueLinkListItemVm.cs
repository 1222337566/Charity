namespace InfrastructureManagmentWebFramework.Models.AccountingProjects.Phases
{
    public class ProjectPhaseStoreIssueLinkListItemVm
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid PhaseId { get; set; }
        public string IssueNumber { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public decimal Amount { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string PhaseName { get; set; } = string.Empty;
        public string? CostCenterName { get; set; }
        public bool IncludeInActualCost { get; set; }
        public string? Notes { get; set; }
    }
}
