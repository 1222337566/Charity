using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.Charity.Kafala;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Kafala;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity.Kafala
{
    public class KafalaPaymentRepository : IKafalaPaymentRepository
    {
        private readonly AppDbContext _db;
        public KafalaPaymentRepository(AppDbContext db) => _db = db;

        public async Task<List<KafalaPayment>> GetAllAsync(Guid? kafalaCaseId = null, Guid? sponsorId = null, Guid? aidCycleId = null)
        {
            var query = _db.Set<KafalaPayment>().AsNoTracking()
                .Include(x => x.KafalaCase)
                .ThenInclude(x => x!.Beneficiary)
                .Include(x => x.Sponsor)
                .Include(x => x.PaymentMethod)
                .Include(x => x.FinancialAccount)
                .AsQueryable();

            if (kafalaCaseId.HasValue)
                query = query.Where(x => x.KafalaCaseId == kafalaCaseId.Value);
            if (sponsorId.HasValue)
                query = query.Where(x => x.SponsorId == sponsorId.Value);
            if (aidCycleId.HasValue)
                query = query.Where(x => x.AidCycleId == aidCycleId.Value);

            return await query.OrderByDescending(x => x.PaymentDate).ThenByDescending(x => x.CreatedAtUtc).ToListAsync();
        }

        public Task<KafalaPayment?> GetByIdAsync(Guid id)
            => _db.Set<KafalaPayment>().FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(KafalaPayment entity) { _db.Set<KafalaPayment>().Add(entity); await _db.SaveChangesAsync(); }
        public async Task UpdateAsync(KafalaPayment entity) { _db.Set<KafalaPayment>().Update(entity); await _db.SaveChangesAsync(); }
    }
}
