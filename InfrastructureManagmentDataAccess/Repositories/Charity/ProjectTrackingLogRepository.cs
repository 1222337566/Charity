using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class ProjectTrackingLogRepository : IProjectTrackingLogRepository
    {
        private readonly AppDbContext _db;
        public ProjectTrackingLogRepository(AppDbContext db) => _db = db;

        public Task<List<ProjectTrackingLog>> GetByProjectIdAsync(Guid projectId)
            => _db.Set<ProjectTrackingLog>().AsNoTracking()
                .Where(x => x.ProjectId == projectId)
                .OrderByDescending(x => x.EntryDate).ThenByDescending(x => x.CreatedAtUtc)
                .ToListAsync();

        public Task<ProjectTrackingLog?> GetByIdAsync(Guid id)
            => _db.Set<ProjectTrackingLog>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(ProjectTrackingLog entity)
        {
            _db.Set<ProjectTrackingLog>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProjectTrackingLog entity)
        {
            _db.Set<ProjectTrackingLog>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
