using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastrfuctureManagmentCore.Domains.Payments;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Lookups;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class CharityLookupRepository : ICharityLookupRepository
    {
        private readonly AppDbContext _db;

        public CharityLookupRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task<List<AidTypeLookup>> GetAidTypesAsync()
            => _db.Set<AidTypeLookup>()
                .AsNoTracking()
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.NameAr)
                .ToListAsync();

        public Task<List<PaymentMethod>> GetActivePaymentMethodsAsync()
            => _db.Set<PaymentMethod>()
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.MethodNameAr)
                .ToListAsync();

        public Task<List<FinancialAccount>> GetActivePostingFinancialAccountsAsync()
            => _db.Set<FinancialAccount>()
                .AsNoTracking()
                .Where(x => x.IsActive && x.IsPosting)
                .OrderBy(x => x.AccountCode)
                .ThenBy(x => x.AccountNameAr)
                .ToListAsync();
    }
}
