using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class BeneficiaryAidDisbursementRepository : IBeneficiaryAidDisbursementRepository
    {
        private readonly AppDbContext _db;

        public BeneficiaryAidDisbursementRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<BeneficiaryAidDisbursement>> GetAllAsync()
        {
            return await _db.Set<BeneficiaryAidDisbursement>()
                .AsNoTracking()
                .Include(x => x.Beneficiary)
                .Include(x => x.AidType)
                .Include(x => x.PaymentMethod)
                .Include(x => x.FinancialAccount)
                .Include(x => x.AidRequest)
                .Include(x => x.FundingLines)
                    .ThenInclude(x => x.DonationAllocation)
                        .ThenInclude(x => x!.Donation)
                .OrderByDescending(x => x.DisbursementDate)
                .ThenByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<List<BeneficiaryAidDisbursement>> GetByBeneficiaryIdAsync(Guid beneficiaryId)
        {
            return await _db.Set<BeneficiaryAidDisbursement>()
                .AsNoTracking()
                .Include(x => x.Beneficiary)
                .Include(x => x.AidType)
                .Include(x => x.PaymentMethod)
                .Include(x => x.FinancialAccount)
                .Include(x => x.AidRequest)
                .Include(x => x.FundingLines)
                    .ThenInclude(x => x.DonationAllocation)
                        .ThenInclude(x => x!.Donation)
                .Where(x => x.BeneficiaryId == beneficiaryId)
                .OrderByDescending(x => x.DisbursementDate)
                .ThenByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<BeneficiaryAidDisbursement?> GetByIdAsync(Guid id)
        {
            return await _db.Set<BeneficiaryAidDisbursement>()
                .Include(x => x.Beneficiary)
                .Include(x => x.AidType)
                .Include(x => x.PaymentMethod)
                .Include(x => x.FinancialAccount)
                .Include(x => x.AidRequest)
                .Include(x => x.FundingLines)
                    .ThenInclude(x => x.DonationAllocation)
                        .ThenInclude(x => x!.Donation)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(BeneficiaryAidDisbursement entity)
        {
            await _db.Set<BeneficiaryAidDisbursement>().AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(BeneficiaryAidDisbursement entity)
        {
            _db.Set<BeneficiaryAidDisbursement>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
