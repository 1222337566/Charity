namespace InfrastrfuctureManagmentCore.Domains.Charity.Projects
{
    /// <summary>
    /// نشاط مرتبط بهدف فرعي ومرحلة تنفيذ
    /// الهدف الفرعي → ينقسم إلى أنشطة
    /// كل نشاط → يُنفَّذ في مرحلة معينة
    /// </summary>
    public class ProjectSubGoalActivity
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid SubGoalId { get; set; }
        public ProjectSubGoal? SubGoal { get; set; }

        public int SortOrder { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ResponsiblePersonName { get; set; }

        public DateTime? PlannedStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }

        public string Status { get; set; } = "Planned"; // Planned|InProgress|Completed|Cancelled
        public string Priority { get; set; } = "Medium"; // Low|Medium|High|Critical
        public decimal ProgressPercent { get; set; }
        public decimal PlannedCost { get; set; }
        public decimal ActualCost { get; set; }
        public decimal PlannedHours { get; set; }
        public decimal ActualHours { get; set; }

        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
        public string? TargetGroup { get; set; }
        public int PlannedQuantity { get; set; }
        public int PlannedDurationDays { get; set; }
        public string? PerformanceIndicator { get; set; }
        public string? VerificationMeans { get; set; }
        public string? TargetAchievement { get; set; }
        public string? QuantityUnit { get; set; }
        public int PlannedHoursPerDay { get; set; }
        public string? TargetGroupDescription { get; set; }
        public int ActualQuantity { get; set; }
        public List<ActivityPhaseAssignment> PhaseAssignments { get; set; }
    }
}
