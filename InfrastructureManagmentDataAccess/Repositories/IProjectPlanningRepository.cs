using InfrastructureManagmentWebFramework.Models.Charity.ProjectPlanning;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects
{
    public interface IProjectPlanningRepository
    {
        Task<ProjectPlanningCalendarVm?> BuildCalendarAsync(Guid projectId, int year, int month);
        Task<ProjectPlanningTimelineVm?> BuildTimelineAsync(Guid projectId);
        Task<ProjectTrackingBoardVm?> BuildTrackingBoardAsync(Guid projectId);
    }
}
