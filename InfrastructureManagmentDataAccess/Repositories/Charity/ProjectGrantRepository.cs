using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class ProjectGrantRepository : IProjectGrantRepository
    {
        private readonly AppDbContext _db;
        public ProjectGrantRepository(AppDbContext db) => _db = db;

        public Task<List<ProjectGrant>> GetByProjectIdAsync(Guid projectId)
            => _db.Set<ProjectGrant>().AsNoTracking()
                .Include(x => x.GrantAgreement)
                .ThenInclude(x => x!.Funder)
                .Where(x => x.ProjectId == projectId)
                .OrderByDescending(x => x.AllocatedDate)
                .ToListAsync();

        public Task<ProjectGrant?> GetByIdAsync(Guid id)
            => _db.Set<ProjectGrant>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        public Task<bool> ExistsAsync(Guid projectId, Guid grantAgreementId)
            => _db.Set<ProjectGrant>().AnyAsync(x => x.ProjectId == projectId && x.GrantAgreementId == grantAgreementId);

        public async Task AddAsync(ProjectGrant entity)
        {
            _db.Set<ProjectGrant>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProjectGrant entity)
        {
            _db.Set<ProjectGrant>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
