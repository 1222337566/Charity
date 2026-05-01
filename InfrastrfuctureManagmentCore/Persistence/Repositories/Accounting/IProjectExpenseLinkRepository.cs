using InfrastrfuctureManagmentCore.Domains.Accounting;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting
{
    public interface IProjectExpenseLinkRepository
    {
        Task<List<ProjectExpenseLink>> GetAllAsync();
        Task<List<ProjectExpenseLink>> GetByProjectIdAsync(Guid projectId);
        Task<ProjectExpenseLink?> GetByIdAsync(Guid id);
        Task<ProjectExpenseLink?> GetByExpenseIdAsync(Guid expenseId);
        Task AddAsync(ProjectExpenseLink entity);
        Task UpdateAsync(ProjectExpenseLink entity);
        Task DeleteAsync(Guid id);
    }
}
