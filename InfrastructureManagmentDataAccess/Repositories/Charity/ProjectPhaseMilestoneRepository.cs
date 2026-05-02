using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class ProjectPhaseMilestoneRepository : IProjectPhaseMilestoneRepository
    {
        private readonly AppDbContext _db;
        public ProjectPhaseMilestoneRepository(AppDbContext db) => _db = db;

        public Task<List<ProjectPhaseMilestone>> GetByPhaseIdAsync(Guid phaseId)
            => _db.Set<ProjectPhaseMilestone>().AsNoTracking()
                .Where(x => x.ProjectPhaseId == phaseId)
                .OrderBy(x => x.DueDate).ThenBy(x => x.Title)
                .ToListAsync();

        public Task<ProjectPhaseMilestone?> GetByIdAsync(Guid id)
            => _db.Set<ProjectPhaseMilestone>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(ProjectPhaseMilestone entity)
        {
            _db.Set<ProjectPhaseMilestone>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProjectPhaseMilestone entity)
        {
            _db.Set<ProjectPhaseMilestone>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
