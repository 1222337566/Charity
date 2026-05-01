namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectPlanning
{
    public class ProjectPlanningCalendarDayVm
    {
        public DateTime Date { get; set; }
        public bool InCurrentMonth { get; set; }
        public List<ProjectPlanningCalendarItemVm> Items { get; set; } = new();
    }
}
