using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.TaskUpdates;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects
{
    public interface IProjectTaskDailyUpdateRepository
    {
        Task<List<ProjectTaskDailyUpdateListItemVm>> GetByTaskIdAsync(Guid taskId);
        Task<ProjectTaskDailyUpdate?> GetByIdAsync(Guid id);
        Task AddAsync(ProjectTaskDailyUpdate entity);
        Task UpdateAsync(ProjectTaskDailyUpdate entity);
    }
}
