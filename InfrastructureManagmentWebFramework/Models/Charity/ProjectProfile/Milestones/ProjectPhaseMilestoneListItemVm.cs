namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Milestones
{
    public class ProjectPhaseMilestoneListItemVm
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal ProgressPercent { get; set; }
        public string? Notes { get; set; }
    }
}
