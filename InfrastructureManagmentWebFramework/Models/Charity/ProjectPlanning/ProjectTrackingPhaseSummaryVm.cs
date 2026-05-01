namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectPlanning
{
    public class ProjectTrackingPhaseSummaryVm
    {
        public Guid PhaseId { get; set; }
        public string PhaseName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal ProgressPercent { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public bool RequiresAttention { get; set; }
    }
}
