namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Activities
{
    public class ProjectPhaseActivityListItemVm
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid PhaseId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public decimal ProgressPercent { get; set; }
        public decimal PlannedHours { get; set; }
        public decimal ActualHours { get; set; }
        public string? ResponsiblePersonName { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public int OpenTasksCount { get; set; }
    }
}
