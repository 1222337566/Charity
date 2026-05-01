using InfrastrfuctureManagmentCore.Domains.Charity.Funders;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Funders;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class FunderRepository : IFunderRepository
    {
        private readonly AppDbContext _db;

        public FunderRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Funder>> SearchAsync(string? q, string? funderType, bool? isActive)
        {
            var query = _db.Set<Funder>()
                .AsNoTracking()
                .Include(x => x.GrantAgreements)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(x => x.Code.Contains(q) || x.Name.Contains(q) || (x.ContactPerson != null && x.ContactPerson.Contains(q)) || (x.PhoneNumber != null && x.PhoneNumber.Contains(q)));
            }

            if (!string.IsNullOrWhiteSpace(funderType))
                query = query.Where(x => x.FunderType == funderType);

            if (isActive.HasValue)
                query = query.Where(x => x.IsActive == isActive.Value);

            return await query.OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<Funder?> GetByIdAsync(Guid id)
        {
            return await _db.Set<Funder>()
                .Include(x => x.GrantAgreements.OrderByDescending(g => g.AgreementDate))
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> CodeExistsAsync(string code, Guid? excludeId = null)
        {
            var query = _db.Set<Funder>().Where(x => x.Code == code);
            if (excludeId.HasValue)
                query = query.Where(x => x.Id != excludeId.Value);
            return await query.AnyAsync();
        }

        public async Task AddAsync(Funder funder)
        {
            await _db.Set<Funder>().AddAsync(funder);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Funder funder)
        {
            _db.Set<Funder>().Update(funder);
            await _db.SaveChangesAsync();
        }
    }
}
