using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.Charity.Kafala;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Kafala;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity.Kafala
{
    public class KafalaSponsorRepository : IKafalaSponsorRepository
    {
        private readonly AppDbContext _db;
        public KafalaSponsorRepository(AppDbContext db) => _db = db;

        public Task<List<KafalaSponsor>> GetAllAsync() => _db.Set<KafalaSponsor>().AsNoTracking().OrderBy(x => x.FullName).ToListAsync();
        public Task<KafalaSponsor?> GetByIdAsync(Guid id) => _db.Set<KafalaSponsor>().FirstOrDefaultAsync(x => x.Id == id);
        public Task<bool> SponsorCodeExistsAsync(string sponsorCode, Guid? ignoreId = null)
            => _db.Set<KafalaSponsor>().AnyAsync(x => x.SponsorCode == sponsorCode && (!ignoreId.HasValue || x.Id != ignoreId.Value));
        public async Task AddAsync(KafalaSponsor entity) { _db.Set<KafalaSponsor>().Add(entity); await _db.SaveChangesAsync(); }
        public async Task UpdateAsync(KafalaSponsor entity) { _db.Set<KafalaSponsor>().Update(entity); await _db.SaveChangesAsync(); }
    }
}
