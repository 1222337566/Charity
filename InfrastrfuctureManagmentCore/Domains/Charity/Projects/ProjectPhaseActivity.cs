namespace InfrastrfuctureManagmentCore.Domains.Charity.Projects
{
    public class ProjectPhaseActivity
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid PhaseId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SortOrder { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public string Status { get; set; } = "Planned";
        public string Priority { get; set; } = "Medium";
        public decimal ProgressPercent { get; set; }
        public decimal PlannedHours { get; set; }
        public decimal ActualHours { get; set; }
        public string? ResponsiblePersonName { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        public ProjectPhase? Phase { get; set; }
        public ICollection<ProjectPhaseTask> Tasks { get; set; } = new List<ProjectPhaseTask>();
    }
}
