using InfrastrfuctureManagmentCore.Domains.Charity.Projects;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects
{
    public interface IProjectPhaseMilestoneRepository
    {
        Task<List<ProjectPhaseMilestone>> GetByPhaseIdAsync(Guid phaseId);
        Task<ProjectPhaseMilestone?> GetByIdAsync(Guid id);
        Task AddAsync(ProjectPhaseMilestone entity);
        Task UpdateAsync(ProjectPhaseMilestone entity);
    }
}
