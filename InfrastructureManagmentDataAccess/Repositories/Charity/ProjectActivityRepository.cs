using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class ProjectActivityRepository : IProjectActivityRepository
    {
        private readonly AppDbContext _db;
        public ProjectActivityRepository(AppDbContext db) => _db = db;

        public Task<List<ProjectActivity>> GetByProjectIdAsync(Guid projectId)
            => _db.Set<ProjectActivity>().AsNoTracking()
                .Where(x => x.ProjectId == projectId)
                .OrderByDescending(x => x.PlannedDate).ThenBy(x => x.Title)
                .ToListAsync();

        public Task<ProjectActivity?> GetByIdAsync(Guid id)
            => _db.Set<ProjectActivity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(ProjectActivity entity)
        {
            _db.Set<ProjectActivity>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProjectActivity entity)
        {
            _db.Set<ProjectActivity>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
