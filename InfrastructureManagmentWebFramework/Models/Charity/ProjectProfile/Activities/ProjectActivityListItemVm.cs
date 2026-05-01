namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Activities
{
    public class ProjectActivityListItemVm
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime PlannedDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal PlannedCost { get; set; }
        public decimal ActualCost { get; set; }
        public string? Notes { get; set; }
    }
}
