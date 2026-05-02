using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectPlanning.Tasks
{
    public class ProjectTaskBoardVm
    {
        public ProjectHeaderVm ProjectHeader { get; set; } = new();
        public List<ProjectTaskBoardColumnVm> Columns { get; set; } = new();
        public int OverdueCount { get; set; }
        public int NeedsFollowUpCount { get; set; }
    }

    public class ProjectTaskBoardColumnVm
    {
        public string Status { get; set; } = string.Empty;
        public List<ProjectPhaseTaskListItemVm> Tasks { get; set; } = new();
    }
}
