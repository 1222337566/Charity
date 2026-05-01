using InfrastrfuctureManagmentCore.Domains.HR;
using InfrastrfuctureManagmentCore.Persistence.Repositories.HR;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.HR
{
    public class HrJobTitleRepository : IHrJobTitleRepository
    {
        private readonly AppDbContext _db;
        public HrJobTitleRepository(AppDbContext db) => _db = db;

        public Task<List<HrJobTitle>> GetAllAsync()
            => _db.Set<HrJobTitle>().AsNoTracking().OrderBy(x => x.Name).ToListAsync();

        public Task<List<HrJobTitle>> GetActiveAsync()
            => _db.Set<HrJobTitle>().AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Name).ToListAsync();

        public Task<HrJobTitle?> GetByIdAsync(Guid id)
            => _db.Set<HrJobTitle>().FirstOrDefaultAsync(x => x.Id == id);

        public Task<bool> NameExistsAsync(string name, Guid? ignoreId = null)
            => _db.Set<HrJobTitle>().AnyAsync(x => x.Name == name && (!ignoreId.HasValue || x.Id != ignoreId.Value));

        public async Task AddAsync(HrJobTitle entity)
        {
            _db.Set<HrJobTitle>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(HrJobTitle entity)
        {
            _db.Set<HrJobTitle>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
