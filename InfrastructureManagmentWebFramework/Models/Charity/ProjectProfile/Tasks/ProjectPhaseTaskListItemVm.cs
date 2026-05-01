namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Tasks
{
    public class ProjectPhaseTaskListItemVm
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid PhaseId { get; set; }
        public Guid ActivityId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public decimal PercentComplete { get; set; }
        public decimal EstimatedHours { get; set; }
        public decimal SpentHours { get; set; }
        public string? AssignedToName { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? LastDailyUpdateAtUtc { get; set; }
    }
}
