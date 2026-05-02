namespace InfrastrfuctureManagmentCore.Domains.Charity.Projects
{
    public class ProjectPhaseMilestone
    {
        public Guid Id { get; set; }
        public Guid ProjectPhaseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Status { get; set; } = "Pending";
        public decimal ProgressPercent { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public ProjectPhase? Phase { get; set; }
    }
}
