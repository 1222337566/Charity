using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class CharityProjectRepository : ICharityProjectRepository
    {
        private readonly AppDbContext _db;
        public CharityProjectRepository(AppDbContext db) => _db = db;

        public async Task<List<CharityProject>> SearchAsync(string? q, string? status, bool? isActive)
        {
            var query = _db.Set<CharityProject>().AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(x => x.Code.Contains(q) || x.Name.Contains(q));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(x => x.Status == status);

            if (isActive.HasValue)
                query = query.Where(x => x.IsActive == isActive.Value);

            return await query.OrderByDescending(x => x.StartDate).ThenBy(x => x.Name).ToListAsync();
        }

        public Task<CharityProject?> GetByIdAsync(Guid id)
            => _db.Set<CharityProject>()
                .AsNoTracking()
                .Include(x => x.BudgetLines)
                .Include(x => x.Activities)
                .Include(x => x.Beneficiaries)
                .Include(x => x.Grants)
                .FirstOrDefaultAsync(x => x.Id == id);

        public Task<bool> CodeExistsAsync(string code)
            => _db.Set<CharityProject>().AnyAsync(x => x.Code == code);

        public Task<bool> CodeExistsAsync(string code, Guid excludeId)
            => _db.Set<CharityProject>().AnyAsync(x => x.Code == code && x.Id != excludeId);

        public async Task AddAsync(CharityProject entity)
        {
            _db.Set<CharityProject>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(CharityProject entity)
        {
            _db.Set<CharityProject>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
