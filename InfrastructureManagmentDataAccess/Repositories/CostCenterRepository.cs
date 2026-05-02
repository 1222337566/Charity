using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories
{
    public class CostCenterRepository : ICostCenterRepository
    {
        private readonly AppDbContext _db;

        public CostCenterRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<CostCenter>> GetAllAsync()
        {
            return await _db.Set<CostCenter>()
                .AsNoTracking()
                .OrderBy(x => x.CostCenterCode)
                .ToListAsync();
        }

        public async Task<List<CostCenter>> GetAllParentsAsync()
        {
            return await _db.Set<CostCenter>()
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.CostCenterCode)
                .ToListAsync();
        }

        public Task<CostCenter?> GetByIdAsync(Guid id)
        {
            return _db.Set<CostCenter>().FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<bool> CodeExistsAsync(string code)
        {
            return _db.Set<CostCenter>().AnyAsync(x => x.CostCenterCode == code);
        }

        public Task<bool> CodeExistsAsync(string code, Guid excludeId)
        {
            return _db.Set<CostCenter>().AnyAsync(x => x.CostCenterCode == code && x.Id != excludeId);
        }

        public Task<bool> HasChildrenAsync(Guid id)
        {
            return _db.Set<CostCenter>().AnyAsync(x => x.ParentCostCenterId == id);
        }

        public Task<bool> HasActiveChildrenAsync(Guid id)
        {
            return _db.Set<CostCenter>().AnyAsync(x => x.ParentCostCenterId == id && x.IsActive);
        }

        public async Task AddAsync(CostCenter entity)
        {
            await _db.Set<CostCenter>().AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(CostCenter entity)
        {
            _db.Set<CostCenter>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
