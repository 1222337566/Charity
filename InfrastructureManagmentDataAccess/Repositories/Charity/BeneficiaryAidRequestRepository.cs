using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class BeneficiaryAidRequestRepository : IBeneficiaryAidRequestRepository
    {
        private readonly AppDbContext _db;

        public BeneficiaryAidRequestRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<BeneficiaryAidRequest>> GetAllAsync()
        {
            return await _db.Set<BeneficiaryAidRequest>()
                .AsNoTracking()
                .Include(x => x.AidType)
                .Include(x => x.Beneficiary)
                .OrderByDescending(x => x.RequestDate)
                .ThenByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<List<BeneficiaryAidRequest>> GetByBeneficiaryIdAsync(Guid beneficiaryId)
        {
            return await _db.Set<BeneficiaryAidRequest>()
                .AsNoTracking()
                .Include(x => x.AidType)
                .Include(x => x.Beneficiary)
                .Where(x => x.BeneficiaryId == beneficiaryId)
                .OrderByDescending(x => x.RequestDate)
                .ThenByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<BeneficiaryAidRequest?> GetByIdAsync(Guid id)
        {
            return await _db.Set<BeneficiaryAidRequest>()
                .Include(x => x.AidType)
                .Include(x => x.Beneficiary)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(BeneficiaryAidRequest entity)
        {
            await _db.Set<BeneficiaryAidRequest>().AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(BeneficiaryAidRequest entity)
        {
            _db.Set<BeneficiaryAidRequest>().Update(entity);
            await _db.SaveChangesAsync();
        }

        public async  Task<List<BeneficiaryAidRequest>> GetAidTypeAsync(Guid aidtypeid)
        {
            return await _db.Set<BeneficiaryAidRequest>()
                .AsNoTracking()
                .Where(x=> x.AidTypeId == aidtypeid)
                .Include(x => x.AidType)
                .Include(x => x.Beneficiary)
                .OrderByDescending(x => x.RequestDate)
                .ThenByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }
    }
}
