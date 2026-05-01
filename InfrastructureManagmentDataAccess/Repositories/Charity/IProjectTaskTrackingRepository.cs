using InfrastructureManagmentWebFramework.Models.Charity.ProjectPlanning.Tasks;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects
{
    public interface IProjectTaskTrackingRepository
    {
        Task<ProjectTaskBoardVm?> BuildBoardAsync(Guid projectId);
        Task<ProjectTaskDailyFollowUpVm?> BuildDailyFollowUpAsync(Guid projectId, DateTime date);
    }
}
