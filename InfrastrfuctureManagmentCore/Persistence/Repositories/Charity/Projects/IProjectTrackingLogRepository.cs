using InfrastrfuctureManagmentCore.Domains.Charity.Projects;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects
{
    public interface IProjectTrackingLogRepository
    {
        Task<List<ProjectTrackingLog>> GetByProjectIdAsync(Guid projectId);
        Task<ProjectTrackingLog?> GetByIdAsync(Guid id);
        Task AddAsync(ProjectTrackingLog entity);
        Task UpdateAsync(ProjectTrackingLog entity);
    }
}
