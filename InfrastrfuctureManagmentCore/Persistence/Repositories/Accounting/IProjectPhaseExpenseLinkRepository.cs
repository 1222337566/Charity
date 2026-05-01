using InfrastrfuctureManagmentCore.Domains.Accounting;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting
{
    public interface IProjectPhaseExpenseLinkRepository
    {
        Task<List<ProjectPhaseExpenseLink>> GetByProjectIdAsync(Guid projectId);
        Task<List<ProjectPhaseExpenseLink>> GetByPhaseIdAsync(Guid phaseId);
        Task<ProjectPhaseExpenseLink?> GetByExpenseIdAsync(Guid expenseId);
        Task AddAsync(ProjectPhaseExpenseLink entity);
        Task DeleteAsync(Guid id);
    }
}
