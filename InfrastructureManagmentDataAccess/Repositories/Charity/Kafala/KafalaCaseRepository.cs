using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.Charity.Kafala;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Kafala;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity.Kafala
{
    public class KafalaCaseRepository : IKafalaCaseRepository
    {
        private readonly AppDbContext _db;
        public KafalaCaseRepository(AppDbContext db) => _db = db;

        public async Task<List<KafalaCase>> GetAllAsync(Guid? sponsorId = null)
        {
            var query = _db.Set<KafalaCase>().AsNoTracking()
                .Include(x => x.Sponsor)
                .Include(x => x.Beneficiary)
                .AsQueryable();

            if (sponsorId.HasValue)
                query = query.Where(x => x.SponsorId == sponsorId.Value);

            return await query.OrderByDescending(x => x.CreatedAtUtc).ToListAsync();
        }

        public Task<List<KafalaCase>> GetActiveDueCasesAsync(DateTime dueBeforeUtc)
            => _db.Set<KafalaCase>().AsNoTracking()
                .Include(x => x.Sponsor)
                .Include(x => x.Beneficiary)
                .Where(x => x.Status == "Active" && x.AutoIncludeInAidCycles && x.NextDueDate.HasValue && x.NextDueDate.Value.Date <= dueBeforeUtc.Date)
                .OrderBy(x => x.NextDueDate)
                .ToListAsync();

        public Task<KafalaCase?> GetByIdAsync(Guid id)
            => _db.Set<KafalaCase>().FirstOrDefaultAsync(x => x.Id == id);

        public Task<KafalaCase?> GetByIdWithDetailsAsync(Guid id)
            => _db.Set<KafalaCase>()
                .Include(x => x.Sponsor)
                .Include(x => x.Beneficiary)
                .Include(x => x.PaymentMethod)
                .Include(x => x.FinancialAccount)
                .FirstOrDefaultAsync(x => x.Id == id);

        public Task<bool> CaseNumberExistsAsync(string caseNumber, Guid? ignoreId = null)
            => _db.Set<KafalaCase>().AnyAsync(x => x.CaseNumber == caseNumber && (!ignoreId.HasValue || x.Id != ignoreId.Value));

        public async Task<KafalaCase> AddAsync(KafalaCase entity) { var enti=_db.Set<KafalaCase>().Add(entity); await _db.SaveChangesAsync(); return enti.Entity; }
        public async Task UpdateAsync(KafalaCase entity) { _db.Set<KafalaCase>().Update(entity); await _db.SaveChangesAsync(); }
    }
}
