using InfrastrfuctureManagmentCore.Domains.Charity.Projects;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects
{
    public interface IProjectPhaseRepository
    {
        Task<List<ProjectPhase>> GetByProjectIdAsync(Guid projectId);
        Task<ProjectPhase?> GetByIdAsync(Guid id);
        Task AddAsync(ProjectPhase entity);
        Task UpdateAsync(ProjectPhase entity);
    }
}
