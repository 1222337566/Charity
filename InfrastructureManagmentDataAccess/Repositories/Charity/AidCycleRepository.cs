using InfrastrfuctureManagmentCore.Domains.Charity.AidCycles;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.AidCycles;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class AidCycleRepository : IAidCycleRepository
    {
        private readonly AppDbContext _db;

        public AidCycleRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<AidCycle>> GetAllAsync()
        {
            return await _db.Set<AidCycle>()
                .AsNoTracking()
                .Include(x => x.AidType)
                .OrderByDescending(x => x.PeriodYear)
                .ThenByDescending(x => x.PeriodMonth)
                .ThenByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<AidCycle?> GetByIdAsync(Guid id)
        {
            return await _db.Set<AidCycle>()
                .Include(x => x.AidType)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<AidCycle?> GetByIdWithBeneficiariesAsync(Guid id)
        {
            return await _db.Set<AidCycle>()
                .Include(x => x.AidType)
                .Include(x => x.Beneficiaries)
                    .ThenInclude(x => x.Beneficiary)
                .Include(x => x.Beneficiaries)
                    .ThenInclude(x => x.AidType)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> CycleNumberExistsAsync(string cycleNumber, Guid? excludeId = null)
        {
            var q = _db.Set<AidCycle>().Where(x => x.CycleNumber == cycleNumber.Trim());
            if (excludeId.HasValue)
                q = q.Where(x => x.Id != excludeId.Value);
            return await q.AnyAsync();
        }

        public async Task AddAsync(AidCycle entity)
        {
            await _db.Set<AidCycle>().AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(AidCycle entity)
        {
            _db.Set<AidCycle>().Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateTotalsAsync(Guid cycleId)
        {
            var cycle = await _db.Set<AidCycle>().FirstOrDefaultAsync(x => x.Id == cycleId);
            if (cycle == null)
                return;

            var lines = await _db.Set<AidCycleBeneficiary>()
                .Where(x => x.AidCycleId == cycleId)
                .ToListAsync();

            cycle.BeneficiariesCount = lines.Count;
            cycle.TotalPlannedAmount = lines.Sum(x => x.ApprovedAmount ?? x.ScheduledAmount ?? 0m);
            cycle.TotalDisbursedAmount = lines.Sum(x => x.DisbursedAmount ?? 0m);
            await _db.SaveChangesAsync();
        }
    }
}
