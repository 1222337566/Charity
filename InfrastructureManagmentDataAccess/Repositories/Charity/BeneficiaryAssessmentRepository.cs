using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class BeneficiaryAssessmentRepository : IBeneficiaryAssessmentRepository
    {
        private readonly AppDbContext _db;

        public BeneficiaryAssessmentRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<BeneficiaryAssessment>> GetByBeneficiaryIdAsync(Guid beneficiaryId)
        {
            return await _db.BeneficiaryAssessments
                .AsNoTracking()
                .Include(x => x.RecommendedAidType)
                .Where(x => x.BeneficiaryId == beneficiaryId)
                .OrderByDescending(x => x.VisitDate)
                .ToListAsync();
        }

        public async Task<BeneficiaryAssessment?> GetByIdAsync(Guid id)
        {
            return await _db.BeneficiaryAssessments
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(BeneficiaryAssessment entity)
        {
            await _db.BeneficiaryAssessments.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(BeneficiaryAssessment entity)
        {
            _db.BeneficiaryAssessments.Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
