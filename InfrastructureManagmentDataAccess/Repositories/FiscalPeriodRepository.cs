using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories
{
    public class FiscalPeriodRepository : IFiscalPeriodRepository
    {
        private readonly AppDbContext _db;

        public FiscalPeriodRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<FiscalPeriod>> GetAllAsync()
        {
            return await _db.Set<FiscalPeriod>()
                .AsNoTracking()
                .OrderByDescending(x => x.StartDate)
                .ToListAsync();
        }

        public Task<FiscalPeriod?> GetByIdAsync(Guid id)
        {
            return _db.Set<FiscalPeriod>().FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<FiscalPeriod?> GetCurrentAsync()
        {
            return _db.Set<FiscalPeriod>().AsNoTracking().FirstOrDefaultAsync(x => x.IsCurrent);
        }

        public Task<bool> CodeExistsAsync(string code)
        {
            return _db.Set<FiscalPeriod>().AnyAsync(x => x.PeriodCode == code);
        }

        public Task<bool> CodeExistsAsync(string code, Guid excludeId)
        {
            return _db.Set<FiscalPeriod>().AnyAsync(x => x.PeriodCode == code && x.Id != excludeId);
        }

        public Task<bool> HasOverlappingPeriodAsync(DateTime startDate, DateTime endDate, Guid? excludeId = null)
        {
            var query = _db.Set<FiscalPeriod>().AsQueryable();
            if (excludeId.HasValue)
                query = query.Where(x => x.Id != excludeId.Value);

            return query.AnyAsync(x => startDate <= x.EndDate && endDate >= x.StartDate);
        }

        public async Task AddAsync(FiscalPeriod entity)
        {
            await _db.Set<FiscalPeriod>().AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(FiscalPeriod entity)
        {
            _db.Set<FiscalPeriod>().Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task ClearCurrentFlagAsync(Guid exceptId)
        {
            var current = await _db.Set<FiscalPeriod>().Where(x => x.IsCurrent && x.Id != exceptId).ToListAsync();
            foreach (var item in current)
            {
                item.IsCurrent = false;
                item.UpdatedAtUtc = DateTime.UtcNow;
            }
            await _db.SaveChangesAsync();
        }
    }
}
