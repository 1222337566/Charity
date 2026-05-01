using InfrastrfuctureManagmentCore.Domains.Charity.Funders;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Funders;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class GrantInstallmentRepository : IGrantInstallmentRepository
    {
        private readonly AppDbContext _db;

        public GrantInstallmentRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<GrantInstallment>> GetByGrantAgreementIdAsync(Guid grantAgreementId)
        {
            return await _db.Set<GrantInstallment>()
                .AsNoTracking()
                .Include(x => x.PaymentMethod)
                .Include(x => x.FinancialAccount)
                .Where(x => x.GrantAgreementId == grantAgreementId)
                .OrderBy(x => x.InstallmentNumber)
                .ToListAsync();
        }

        public async Task<GrantInstallment?> GetByIdAsync(Guid id)
        {
            return await _db.Set<GrantInstallment>()
                .Include(x => x.GrantAgreement)
                    .ThenInclude(x => x!.Funder)
                .Include(x => x.PaymentMethod)
                .Include(x => x.FinancialAccount)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> InstallmentNumberExistsAsync(Guid grantAgreementId, int installmentNumber, Guid? excludeId = null)
        {
            var query = _db.Set<GrantInstallment>().Where(x => x.GrantAgreementId == grantAgreementId && x.InstallmentNumber == installmentNumber);
            if (excludeId.HasValue)
                query = query.Where(x => x.Id != excludeId.Value);
            return await query.AnyAsync();
        }

        public async Task AddAsync(GrantInstallment installment)
        {
            await _db.Set<GrantInstallment>().AddAsync(installment);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(GrantInstallment installment)
        {
            _db.Set<GrantInstallment>().Update(installment);
            await _db.SaveChangesAsync();
        }
    }
}
