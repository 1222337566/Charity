using InfrastrfuctureManagmentCore.Domains.Charity.Projects;

namespace InfrastructureManagmentDataAccess.Repositories.Charity;

public interface IProjectActivityRepository
{
    Task<List<ProjectActivity>> GetByProjectIdAsync(Guid projectId);
    Task<ProjectActivity?> GetByIdAsync(Guid id);
    Task AddAsync(ProjectActivity entity);
    Task UpdateAsync(ProjectActivity entity);
}
