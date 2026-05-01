using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile;

namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectPlanning
{
    public class ProjectPlanningCalendarVm
    {
        public ProjectHeaderVm ProjectHeader { get; set; } = new();
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public List<ProjectPlanningCalendarDayVm> Days { get; set; } = new();
    }
}
