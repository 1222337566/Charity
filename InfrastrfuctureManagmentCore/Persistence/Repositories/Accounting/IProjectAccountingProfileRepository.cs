using InfrastrfuctureManagmentCore.Domains.Accounting;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting
{
    public interface IProjectAccountingProfileRepository
    {
        Task<List<ProjectAccountingProfile>> GetAllAsync();
        Task<ProjectAccountingProfile?> GetByIdAsync(Guid id);
        Task<ProjectAccountingProfile?> GetByProjectIdAsync(Guid projectId);
        Task AddAsync(ProjectAccountingProfile entity);
        Task UpdateAsync(ProjectAccountingProfile entity);
        Task DeleteAsync(Guid id);
    }
}
