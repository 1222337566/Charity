using InfrastrfuctureManagmentCore.Domains.Charity.Funders;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Funders;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class GrantConditionRepository : IGrantConditionRepository
    {
        private readonly AppDbContext _db;

        public GrantConditionRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<GrantCondition>> GetByGrantAgreementIdAsync(Guid grantAgreementId)
        {
            return await _db.Set<GrantCondition>()
                .AsNoTracking()
                .Where(x => x.GrantAgreementId == grantAgreementId)
                .OrderBy(x => x.IsFulfilled)
                .ThenBy(x => x.DueDate)
                .ThenBy(x => x.ConditionTitle)
                .ToListAsync();
        }

        public async Task<GrantCondition?> GetByIdAsync(Guid id)
        {
            return await _db.Set<GrantCondition>()
                .Include(x => x.GrantAgreement)
                    .ThenInclude(x => x!.Funder)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(GrantCondition condition)
        {
            await _db.Set<GrantCondition>().AddAsync(condition);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(GrantCondition condition)
        {
            _db.Set<GrantCondition>().Update(condition);
            await _db.SaveChangesAsync();
        }
    }
}
