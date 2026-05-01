using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class BeneficiaryCommitteeDecisionRepository : IBeneficiaryCommitteeDecisionRepository
    {
        private readonly AppDbContext _db;

        public BeneficiaryCommitteeDecisionRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<BeneficiaryCommitteeDecision>> GetByBeneficiaryIdAsync(Guid beneficiaryId)
        {
            return await _db.BeneficiaryCommitteeDecisions
                .AsNoTracking()
                .Include(x => x.ApprovedAidType)
                .Where(x => x.BeneficiaryId == beneficiaryId)
                .OrderByDescending(x => x.DecisionDate)
                .ToListAsync();
        }

        public async Task<BeneficiaryCommitteeDecision?> GetByIdAsync(Guid id)
        {
            return await _db.BeneficiaryCommitteeDecisions
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(BeneficiaryCommitteeDecision entity)
        {
            await _db.BeneficiaryCommitteeDecisions.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(BeneficiaryCommitteeDecision entity)
        {
            _db.BeneficiaryCommitteeDecisions.Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
