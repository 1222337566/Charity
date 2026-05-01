using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class BeneficiaryFamilyMemberRepository : IBeneficiaryFamilyMemberRepository
    {
        private readonly AppDbContext _db;

        public BeneficiaryFamilyMemberRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<BeneficiaryFamilyMember>> GetByBeneficiaryIdAsync(Guid beneficiaryId)
        {
            return await _db.BeneficiaryFamilyMembers
                .AsNoTracking()
                .Include(x => x.Gender)
                .Where(x => x.BeneficiaryId == beneficiaryId)
                .OrderBy(x => x.FullName)
                .ToListAsync();
        }

        public async Task<BeneficiaryFamilyMember?> GetByIdAsync(Guid id)
        {
            return await _db.BeneficiaryFamilyMembers
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(BeneficiaryFamilyMember entity)
        {
            await _db.BeneficiaryFamilyMembers.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(BeneficiaryFamilyMember entity)
        {
            _db.BeneficiaryFamilyMembers.Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
