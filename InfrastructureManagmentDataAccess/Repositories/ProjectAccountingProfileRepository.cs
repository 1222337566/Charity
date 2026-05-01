using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories
{
    public class ProjectAccountingProfileRepository : IProjectAccountingProfileRepository
    {
        private readonly AppDbContext _db;
        public ProjectAccountingProfileRepository(AppDbContext db) => _db = db;

        public Task<List<ProjectAccountingProfile>> GetAllAsync() =>
            _db.Set<ProjectAccountingProfile>()
                .Include(x => x.Project)
                .Include(x => x.DefaultCostCenter)
                .Include(x => x.DefaultRevenueAccount)
                .Include(x => x.DefaultExpenseAccount)
                .OrderBy(x => x.Project!.Name)
                .ToListAsync();

        public Task<ProjectAccountingProfile?> GetByIdAsync(Guid id) =>
            _db.Set<ProjectAccountingProfile>()
                .Include(x => x.Project)
                .Include(x => x.DefaultCostCenter)
                .Include(x => x.DefaultRevenueAccount)
                .Include(x => x.DefaultExpenseAccount)
                .FirstOrDefaultAsync(x => x.Id == id);

        public Task<ProjectAccountingProfile?> GetByProjectIdAsync(Guid projectId) =>
            _db.Set<ProjectAccountingProfile>()
                .Include(x => x.Project)
                .Include(x => x.DefaultCostCenter)
                .Include(x => x.DefaultRevenueAccount)
                .Include(x => x.DefaultExpenseAccount)
                .FirstOrDefaultAsync(x => x.ProjectId == projectId);

        public async Task AddAsync(ProjectAccountingProfile entity)
        {
            _db.Set<ProjectAccountingProfile>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProjectAccountingProfile entity)
        {
            _db.Set<ProjectAccountingProfile>().Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _db.Set<ProjectAccountingProfile>().FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return;
            _db.Set<ProjectAccountingProfile>().Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
