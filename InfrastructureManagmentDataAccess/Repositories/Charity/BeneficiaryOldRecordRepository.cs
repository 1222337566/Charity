using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class BeneficiaryOldRecordRepository : IBeneficiaryOldRecordRepository
    {
        private readonly AppDbContext _db;

        public BeneficiaryOldRecordRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<BeneficiaryOldRecord>> GetByBeneficiaryIdAsync(Guid beneficiaryId)
        {
            return await _db.BeneficiaryOldRecords
                .AsNoTracking()
                .Where(x => x.BeneficiaryId == beneficiaryId)
                .OrderByDescending(x => x.RecordDate)
                .ToListAsync();
        }

        public async Task<BeneficiaryOldRecord?> GetByIdAsync(Guid id)
        {
            return await _db.BeneficiaryOldRecords
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(BeneficiaryOldRecord entity)
        {
            await _db.BeneficiaryOldRecords.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(BeneficiaryOldRecord entity)
        {
            _db.BeneficiaryOldRecords.Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
