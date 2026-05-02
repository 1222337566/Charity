using InfrastrfuctureManagmentCore.Domains.Charity.Projects;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects
{
    public interface IProjectBudgetLineRepository
    {
        Task<List<ProjectBudgetLine>> GetByProjectIdAsync(Guid projectId);
        Task<ProjectBudgetLine?> GetByIdAsync(Guid id);
        Task AddAsync(ProjectBudgetLine entity);
        Task UpdateAsync(ProjectBudgetLine entity);
    }
}
