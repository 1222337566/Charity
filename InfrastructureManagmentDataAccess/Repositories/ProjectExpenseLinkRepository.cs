using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories
{
    public class ProjectExpenseLinkRepository : IProjectExpenseLinkRepository
    {
        private readonly AppDbContext _db;
        public ProjectExpenseLinkRepository(AppDbContext db) => _db = db;

        public Task<List<ProjectExpenseLink>> GetAllAsync() =>
            _db.Set<ProjectExpenseLink>()
                .Include(x => x.Expense).Include(x => x.Project).Include(x => x.ProjectBudgetLine).Include(x => x.CostCenter)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync();

        public Task<List<ProjectExpenseLink>> GetByProjectIdAsync(Guid projectId) =>
            _db.Set<ProjectExpenseLink>()
                .Include(x => x.Expense).Include(x => x.Project).Include(x => x.ProjectBudgetLine).Include(x => x.CostCenter)
                .Where(x => x.ProjectId == projectId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync();

        public Task<ProjectExpenseLink?> GetByIdAsync(Guid id) =>
            _db.Set<ProjectExpenseLink>()
                .Include(x => x.Expense).Include(x => x.Project).Include(x => x.ProjectBudgetLine).Include(x => x.CostCenter)
                .FirstOrDefaultAsync(x => x.Id == id);

        public Task<ProjectExpenseLink?> GetByExpenseIdAsync(Guid expenseId) =>
            _db.Set<ProjectExpenseLink>()
                .Include(x => x.Expense).Include(x => x.Project).Include(x => x.ProjectBudgetLine).Include(x => x.CostCenter)
                .FirstOrDefaultAsync(x => x.ExpenseId == expenseId);

        public async Task AddAsync(ProjectExpenseLink entity)
        {
            _db.Set<ProjectExpenseLink>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProjectExpenseLink entity)
        {
            _db.Set<ProjectExpenseLink>().Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _db.Set<ProjectExpenseLink>().FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return;
            _db.Set<ProjectExpenseLink>().Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
