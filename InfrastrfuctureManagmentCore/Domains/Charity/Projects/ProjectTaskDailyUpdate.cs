namespace InfrastrfuctureManagmentCore.Domains.Charity.Projects
{
    public class ProjectTaskDailyUpdate
    {
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Status { get; set; } = "InProgress";
        public decimal ProgressPercent { get; set; }
        public decimal HoursSpent { get; set; }
        public string? Note { get; set; }
        public string? BlockerNote { get; set; }
        public string? NextAction { get; set; }
        public string? CreatedByUserId { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        public ProjectPhaseTask? Task { get; set; }
    }
}
