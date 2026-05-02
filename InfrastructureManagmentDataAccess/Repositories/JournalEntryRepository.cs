using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories
{
    public class JournalEntryRepository : IJournalEntryRepository
    {
        private readonly AppDbContext _db;

        public JournalEntryRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<JournalEntry>> GetAllAsync()
        {
            return await _db.Set<JournalEntry>()
                .AsNoTracking()
                .Include(x => x.FiscalPeriod)
                .Include(x => x.Lines)
                .OrderByDescending(x => x.EntryDate)
                .ThenByDescending(x => x.EntryNumber)
                .ToListAsync();
        }

        public async Task<JournalEntry?> GetByIdAsync(Guid id)
        {
            return await _db.Set<JournalEntry>()
                .Include(x => x.FiscalPeriod)
                .Include(x => x.Lines)
                    .ThenInclude(x => x.FinancialAccount)
                .Include(x => x.Lines)
                    .ThenInclude(x => x.CostCenter)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<JournalEntryLine?> GetLineByIdAsync(Guid lineId)
        {
            return _db.Set<JournalEntryLine>()
                .Include(x => x.JournalEntry)
                .Include(x => x.FinancialAccount)
                .Include(x => x.CostCenter)
                .FirstOrDefaultAsync(x => x.Id == lineId);
        }

        public Task<bool> EntryNumberExistsAsync(string entryNumber)
        {
            return _db.Set<JournalEntry>().AnyAsync(x => x.EntryNumber == entryNumber);
        }

        public Task<bool> EntryNumberExistsAsync(string entryNumber, Guid excludeId)
        {
            return _db.Set<JournalEntry>().AnyAsync(x => x.EntryNumber == entryNumber && x.Id != excludeId);
        }

        public async Task AddAsync(JournalEntry entity)
        {
            await _db.Set<JournalEntry>().AddAsync(entity);
            await EnsurePeriodIsOpenAsync((DateTime)entity.CreatedAtUtc);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(JournalEntry entity)
        {
            _db.Set<JournalEntry>().Update(entity);
            await EnsurePeriodIsOpenAsync((DateTime)entity.UpdatedAtUtc);
            await _db.SaveChangesAsync();
        }

        public async Task AddLineAsync(JournalEntryLine entity)
        {
            await _db.Set<JournalEntryLine>().AddAsync(entity);
            await EnsurePeriodIsOpenAsync((DateTime)entity.UpdatedAtUtc);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateLineAsync(JournalEntryLine entity)
        {
            _db.Set<JournalEntryLine>().Update(entity);
            await EnsurePeriodIsOpenAsync((DateTime)entity.UpdatedAtUtc);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteLineAsync(JournalEntryLine entity)
        {
            _db.Set<JournalEntryLine>().Remove(entity);
            await EnsurePeriodIsOpenAsync((DateTime)entity.UpdatedAtUtc);
            await _db.SaveChangesAsync();
        }
        private async Task EnsurePeriodIsOpenAsync(DateTime entryDateUtc)
        {
            var closedPeriod = await _db.Set<FiscalPeriod>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.IsClosed &&
                    entryDateUtc >= x.StartDate &&
                    entryDateUtc <= x.EndDate);

            if (closedPeriod != null)
                throw new InvalidOperationException($"لا يمكن إنشاء أو تعديل قيد داخل فترة مالية مقفلة: {closedPeriod.PeriodNameAr}");
        }
        public Task SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }
    }
}
