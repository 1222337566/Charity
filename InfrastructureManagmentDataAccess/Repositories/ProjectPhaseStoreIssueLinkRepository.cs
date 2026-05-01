using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories
{
    public class ProjectPhaseStoreIssueLinkRepository : IProjectPhaseStoreIssueLinkRepository
    {
        private readonly AppDbContext _db;
        public ProjectPhaseStoreIssueLinkRepository(AppDbContext db) => _db = db;

        public Task<List<ProjectPhaseStoreIssueLink>> GetByProjectIdAsync(Guid projectId) =>
            _db.Set<ProjectPhaseStoreIssueLink>()
                .Include(x => x.StoreIssue).ThenInclude(x => x!.Warehouse)
                .Include(x => x.Project).Include(x => x.ProjectPhase).Include(x => x.CostCenter)
                .Where(x => x.ProjectId == projectId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync();

        public Task<List<ProjectPhaseStoreIssueLink>> GetByPhaseIdAsync(Guid phaseId) =>
            _db.Set<ProjectPhaseStoreIssueLink>()
                .Include(x => x.StoreIssue).ThenInclude(x => x!.Warehouse)
                .Include(x => x.Project).Include(x => x.ProjectPhase).Include(x => x.CostCenter)
                .Where(x => x.ProjectPhaseId == phaseId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync();

        public Task<ProjectPhaseStoreIssueLink?> GetByStoreIssueIdAsync(Guid storeIssueId) =>
            _db.Set<ProjectPhaseStoreIssueLink>()
                .Include(x => x.StoreIssue).ThenInclude(x => x!.Warehouse)
                .Include(x => x.Project).Include(x => x.ProjectPhase).Include(x => x.CostCenter)
                .FirstOrDefaultAsync(x => x.StoreIssueId == storeIssueId);

        public async Task AddAsync(ProjectPhaseStoreIssueLink entity)
        {
            _db.Set<ProjectPhaseStoreIssueLink>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _db.Set<ProjectPhaseStoreIssueLink>().FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return;
            _db.Set<ProjectPhaseStoreIssueLink>().Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
