namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectPlanning
{
    public class ProjectPlanningTimelineRowVm
    {
        public Guid PhaseId { get; set; }
        public string PhaseName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal ProgressPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal OffsetPercent { get; set; }
        public decimal WidthPercent { get; set; }
    }
}
