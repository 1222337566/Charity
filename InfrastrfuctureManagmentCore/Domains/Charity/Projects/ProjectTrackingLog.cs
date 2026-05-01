namespace InfrastrfuctureManagmentCore.Domains.Charity.Projects
{
    public class ProjectTrackingLog
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? ProjectPhaseId { get; set; }
        public DateTime EntryDate { get; set; } = DateTime.Today;
        public string EntryType { get; set; } = "Update";
        public string Title { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string? Status { get; set; }
        public decimal? ProgressPercent { get; set; }
        public bool RequiresAttention { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public CharityProject? Project { get; set; }
        public ProjectPhase? Phase { get; set; }
    }
}
