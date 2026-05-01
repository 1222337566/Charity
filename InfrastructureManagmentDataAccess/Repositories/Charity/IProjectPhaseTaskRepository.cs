using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Tasks;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects
{
    public interface IProjectPhaseTaskRepository
    {
        Task<List<ProjectPhaseTaskListItemVm>> GetByActivityIdAsync(Guid activityId);
        Task<List<ProjectPhaseTaskListItemVm>> GetByPhaseIdAsync(Guid phaseId);
        Task<ProjectPhaseTask?> GetByIdAsync(Guid id);
        Task AddAsync(ProjectPhaseTask entity);
        Task UpdateAsync(ProjectPhaseTask entity);
    }
}
