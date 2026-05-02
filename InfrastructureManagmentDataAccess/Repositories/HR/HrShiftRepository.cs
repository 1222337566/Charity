using InfrastrfuctureManagmentCore.Domains.HR;
using InfrastrfuctureManagmentCore.Persistence.Repositories.HR;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.HR
{
    public class HrShiftRepository : IHrShiftRepository
    {
        private readonly AppDbContext _db;
        public HrShiftRepository(AppDbContext db) => _db = db;

        public Task<List<HrShift>> GetAllAsync()
            => _db.Set<HrShift>().AsNoTracking().OrderBy(x => x.Name).ToListAsync();

        public Task<List<HrShift>> GetActiveAsync()
            => _db.Set<HrShift>().AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Name).ToListAsync();

        public Task<HrShift?> GetByIdAsync(Guid id)
            => _db.Set<HrShift>().FirstOrDefaultAsync(x => x.Id == id);

        public Task<bool> NameExistsAsync(string name, Guid? ignoreId = null)
            => _db.Set<HrShift>().AnyAsync(x => x.Name == name && (!ignoreId.HasValue || x.Id != ignoreId.Value));

        public async Task AddAsync(HrShift entity)
        {
            _db.Set<HrShift>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(HrShift entity)
        {
            _db.Set<HrShift>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
