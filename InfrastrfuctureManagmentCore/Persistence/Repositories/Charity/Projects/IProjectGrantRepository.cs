using InfrastrfuctureManagmentCore.Domains.Charity.Projects;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects
{
    public interface IProjectGrantRepository
    {
        Task<List<ProjectGrant>> GetByProjectIdAsync(Guid projectId);
        Task<ProjectGrant?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid projectId, Guid grantAgreementId);
        Task AddAsync(ProjectGrant entity);
        Task UpdateAsync(ProjectGrant entity);
    }
}
