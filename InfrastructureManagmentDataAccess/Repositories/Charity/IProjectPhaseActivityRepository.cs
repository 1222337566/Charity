using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Activities;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects
{
    public interface IProjectPhaseActivityRepository
    {
        Task<List<ProjectPhaseActivityListItemVm>> GetByPhaseIdAsync(Guid phaseId);
        Task<ProjectPhaseActivity?> GetByIdAsync(Guid id);
        Task AddAsync(ProjectPhaseActivity entity);
        Task UpdateAsync(ProjectPhaseActivity entity);
    }
}
