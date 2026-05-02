namespace InfrastrfuctureManagmentCore.Domains.Charity.Projects
{
    public class ProjectPhaseTask
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid PhaseId { get; set; }
        public Guid ActivityId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SortOrder { get; set; }
        public string Status { get; set; } = "Todo";
        public string Priority { get; set; } = "Medium";
        public DateTime? PlannedStartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? StartedAtUtc { get; set; }
        public DateTime? CompletedAtUtc { get; set; }
        public decimal PercentComplete { get; set; }
        public decimal EstimatedHours { get; set; }
        public decimal SpentHours { get; set; }
        public string? AssignedToUserId { get; set; }
        public string? AssignedToName { get; set; }
        public bool RequiresDailyFollowUp { get; set; } = true;
        public DateTime? LastDailyUpdateAtUtc { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        public ProjectPhase? Phase { get; set; }
        public ProjectPhaseActivity? Activity { get; set; }
        public ICollection<ProjectTaskDailyUpdate> DailyUpdates { get; set; } = new List<ProjectTaskDailyUpdate>();
    }
}
