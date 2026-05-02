namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Tracking
{
    public class ProjectTrackingLogListItemVm
    {
        public Guid Id { get; set; }
        public Guid? ProjectPhaseId { get; set; }
        public DateTime EntryDate { get; set; }
        public string EntryType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string? Status { get; set; }
        public decimal? ProgressPercent { get; set; }
        public bool RequiresAttention { get; set; }
    }
}
