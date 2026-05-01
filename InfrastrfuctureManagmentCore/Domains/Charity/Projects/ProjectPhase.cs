namespace InfrastrfuctureManagmentCore.Domains.Charity.Projects
{
    public class ProjectPhase
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SortOrder { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public string Status { get; set; } = "Planned";
        public decimal ProgressPercent { get; set; }
        public decimal PlannedCost { get; set; }
        public decimal ActualCost { get; set; }
        public Guid? CostCenterId { get; set; }
        public string? ResponsiblePersonName { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        public CharityProject? Project { get; set; }
        public ICollection<ProjectPhaseMilestone> Milestones { get; set; } = new List<ProjectPhaseMilestone>();
        public ICollection<ProjectTrackingLog> TrackingLogs { get; set; } = new List<ProjectTrackingLog>();
    }
}
