using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class ProjectBudgetLineRepository : IProjectBudgetLineRepository
    {
        private readonly AppDbContext _db;
        public ProjectBudgetLineRepository(AppDbContext db) => _db = db;

        public Task<List<ProjectBudgetLine>> GetByProjectIdAsync(Guid projectId)
            => _db.Set<ProjectBudgetLine>().AsNoTracking()
                .Where(x => x.ProjectId == projectId)
                .OrderBy(x => x.LineType).ThenBy(x => x.LineName)
                .ToListAsync();

        public Task<ProjectBudgetLine?> GetByIdAsync(Guid id)
            => _db.Set<ProjectBudgetLine>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(ProjectBudgetLine entity)
        {
            _db.Set<ProjectBudgetLine>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProjectBudgetLine entity)
        {
            _db.Set<ProjectBudgetLine>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
