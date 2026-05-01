namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectPlanning
{
    public class ProjectPlanningCalendarItemVm
    {
        public string Kind { get; set; } = string.Empty;
        public Guid ReferenceId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CssClass { get; set; } = "info";
    }
}
