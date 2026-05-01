using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile;

namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectPlanning
{
    public class ProjectPlanningTimelineVm
    {
        public ProjectHeaderVm ProjectHeader { get; set; } = new();
        public DateTime RangeStart { get; set; }
        public DateTime RangeEnd { get; set; }
        public List<ProjectPlanningTimelineRowVm> Rows { get; set; } = new();
    }
}
