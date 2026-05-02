using InfrastrfuctureManagmentCore.Domains.Charity.Funders;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Funders;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class GrantAgreementRepository : IGrantAgreementRepository
    {
        private readonly AppDbContext _db;

        public GrantAgreementRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<GrantAgreement>> GetAllAsync()
        {
            return await _db.Set<GrantAgreement>()
                .AsNoTracking()
                .Include(x => x.Funder)
                .OrderByDescending(x => x.AgreementDate)
                .ThenBy(x => x.AgreementNumber)
                .ToListAsync();
        }

        public async Task<List<GrantAgreement>> GetByFunderIdAsync(Guid funderId)
        {
            return await _db.Set<GrantAgreement>()
                .AsNoTracking()
                .Include(x => x.Installments)
                .Include(x => x.Conditions)
                .Where(x => x.FunderId == funderId)
                .OrderByDescending(x => x.AgreementDate)
                .ThenByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<GrantAgreement?> GetByIdAsync(Guid id)
        {
            return await _db.Set<GrantAgreement>()
                .Include(x => x.Funder)
                .Include(x => x.Installments.OrderBy(i => i.InstallmentNumber))
                .Include(x => x.Conditions.OrderBy(c => c.IsFulfilled).ThenBy(c => c.DueDate))
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> AgreementNumberExistsAsync(string agreementNumber)
            => await _db.Set<GrantAgreement>().AnyAsync(x => x.AgreementNumber == agreementNumber);

        public async Task<bool> AgreementNumberExistsAsync(string agreementNumber, Guid excludeId)
            => await _db.Set<GrantAgreement>().AnyAsync(x => x.AgreementNumber == agreementNumber && x.Id != excludeId);

        public async Task AddAsync(GrantAgreement agreement)
        {
            await _db.Set<GrantAgreement>().AddAsync(agreement);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(GrantAgreement agreement)
        {
            _db.Set<GrantAgreement>().Update(agreement);
            await _db.SaveChangesAsync();
        }
    }
}
