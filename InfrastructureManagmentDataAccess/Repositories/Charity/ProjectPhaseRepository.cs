using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class ProjectPhaseRepository : IProjectPhaseRepository
    {
        private readonly AppDbContext _db;
        public ProjectPhaseRepository(AppDbContext db) => _db = db;

        public Task<List<ProjectPhase>> GetByProjectIdAsync(Guid projectId)
            => _db.Set<ProjectPhase>().AsNoTracking()
                .Where(x => x.ProjectId == projectId)
                .OrderBy(x => x.SortOrder).ThenBy(x => x.PlannedStartDate).ThenBy(x => x.Name)
                .ToListAsync();

        public Task<ProjectPhase?> GetByIdAsync(Guid id)
            => _db.Set<ProjectPhase>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(ProjectPhase entity)
        {
            _db.Set<ProjectPhase>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProjectPhase entity)
        {
            _db.Set<ProjectPhase>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
