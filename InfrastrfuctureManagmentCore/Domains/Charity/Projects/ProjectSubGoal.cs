namespace InfrastrfuctureManagmentCore.Domains.Charity.Projects
{
    /// <summary>الهدف الفرعي — يرتبط بهدف رئيسي</summary>
    public class ProjectSubGoal
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid GoalId { get; set; }
        public ProjectGoal? Goal { get; set; }

        public int SortOrder { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? SuccessIndicator { get; set; }
        public string? TargetValue { get; set; }
        public string? AchievedValue { get; set; }
        public decimal ProgressPercent { get; set; }
        public string Status { get; set; } = "Active";
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public ICollection<ProjectSubGoalActivity> Activities { get; set; } = new List<ProjectSubGoalActivity>();
    }
}
