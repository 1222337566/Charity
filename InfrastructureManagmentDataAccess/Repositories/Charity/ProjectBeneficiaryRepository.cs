using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class ProjectBeneficiaryRepository : IProjectBeneficiaryRepository
    {
        private readonly AppDbContext _db;
        public ProjectBeneficiaryRepository(AppDbContext db) => _db = db;

        public Task<List<ProjectBeneficiary>> GetByProjectIdAsync(Guid projectId)
            => _db.Set<ProjectBeneficiary>().AsNoTracking()
                .Include(x => x.Beneficiary)
                .Where(x => x.ProjectId == projectId)
                .OrderByDescending(x => x.EnrollmentDate)
                .ToListAsync();

        public Task<ProjectBeneficiary?> GetByIdAsync(Guid id)
            => _db.Set<ProjectBeneficiary>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        public Task<bool> ExistsAsync(Guid projectId, Guid beneficiaryId)
            => _db.Set<ProjectBeneficiary>().AnyAsync(x => x.ProjectId == projectId && x.BeneficiaryId == beneficiaryId);

        public async Task AddAsync(ProjectBeneficiary entity)
        {
            _db.Set<ProjectBeneficiary>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProjectBeneficiary entity)
        {
            _db.Set<ProjectBeneficiary>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
