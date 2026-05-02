using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories
{
    public class ProjectPhaseExpenseLinkRepository : IProjectPhaseExpenseLinkRepository
    {
        private readonly AppDbContext _db;
        public ProjectPhaseExpenseLinkRepository(AppDbContext db) => _db = db;

        public Task<List<ProjectPhaseExpenseLink>> GetByProjectIdAsync(Guid projectId) =>
            _db.Set<ProjectPhaseExpenseLink>()
                .Include(x => x.Expense).Include(x => x.Project).Include(x => x.ProjectPhase).Include(x => x.ProjectBudgetLine).Include(x => x.CostCenter)
                .Where(x => x.ProjectId == projectId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync();

        public Task<List<ProjectPhaseExpenseLink>> GetByPhaseIdAsync(Guid phaseId) =>
            _db.Set<ProjectPhaseExpenseLink>()
                .Include(x => x.Expense).Include(x => x.Project).Include(x => x.ProjectPhase).Include(x => x.ProjectBudgetLine).Include(x => x.CostCenter)
                .Where(x => x.ProjectPhaseId == phaseId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync();

        public Task<ProjectPhaseExpenseLink?> GetByExpenseIdAsync(Guid expenseId) =>
            _db.Set<ProjectPhaseExpenseLink>()
                .Include(x => x.Expense).Include(x => x.Project).Include(x => x.ProjectPhase).Include(x => x.ProjectBudgetLine).Include(x => x.CostCenter)
                .FirstOrDefaultAsync(x => x.ExpenseId == expenseId);

        public async Task AddAsync(ProjectPhaseExpenseLink entity)
        {
            _db.Set<ProjectPhaseExpenseLink>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _db.Set<ProjectPhaseExpenseLink>().FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return;
            _db.Set<ProjectPhaseExpenseLink>().Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
