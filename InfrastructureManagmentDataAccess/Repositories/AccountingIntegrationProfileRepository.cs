using InfrastrfuctureManagmentCore.Domains.Accounting.Integration;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories
{
    public class AccountingIntegrationProfileRepository : IAccountingIntegrationProfileRepository
    {
        private readonly AppDbContext _db;

        public AccountingIntegrationProfileRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<AccountingIntegrationProfile>> GetAllAsync()
        {
            return await _db.Set<AccountingIntegrationProfile>()
                .AsNoTracking()
                .Include(x => x.DebitAccount)
                .Include(x => x.CreditAccount)
                .Include(x => x.DefaultCostCenter)
                .OrderBy(x => x.SourceType)
                .ThenBy(x => x.Priority)
                .ThenBy(x => x.ProfileNameAr)
                .ToListAsync();
        }

        public async Task<AccountingIntegrationProfile?> GetByIdAsync(Guid id)
        {
            return await _db.Set<AccountingIntegrationProfile>()
                .Include(x => x.DebitAccount)
                .Include(x => x.CreditAccount)
                .Include(x => x.DefaultCostCenter)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<AccountingIntegrationProfile?> GetBySourceTypeAsync(string sourceType)
        {
            return await _db.Set<AccountingIntegrationProfile>()
                .Where(x => x.SourceType == sourceType && x.IsActive)
                .OrderBy(x => x.Priority)
                .ThenByDescending(x => x.MatchAidTypeId.HasValue)
                .ThenByDescending(x => x.MatchDonationType != null && x.MatchDonationType != "")
                .ThenByDescending(x => x.MatchTargetingScopeCode != null && x.MatchTargetingScopeCode != "")
                .ThenBy(x => x.ProfileNameAr)
                .FirstOrDefaultAsync();
        }

        public async Task<List<AccountingIntegrationProfile>> GetActiveBySourceTypeAsync(string sourceType)
        {
            return await _db.Set<AccountingIntegrationProfile>()
                .AsNoTracking()
                .Where(x => x.SourceType == sourceType && x.IsActive)
                .OrderBy(x => x.Priority)
                .ThenBy(x => x.ProfileNameAr)
                .ToListAsync();
        }

        public async Task AddAsync(AccountingIntegrationProfile entity)
        {
            await _db.Set<AccountingIntegrationProfile>().AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(AccountingIntegrationProfile entity)
        {
            _db.Set<AccountingIntegrationProfile>().Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
