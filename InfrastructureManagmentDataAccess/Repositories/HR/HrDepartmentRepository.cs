using InfrastrfuctureManagmentCore.Domains.HR;
using InfrastrfuctureManagmentCore.Persistence.Repositories.HR;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.HR
{
    public class HrDepartmentRepository : IHrDepartmentRepository
    {
        private readonly AppDbContext _db;
        public HrDepartmentRepository(AppDbContext db) => _db = db;

        public Task<List<HrDepartment>> GetAllAsync()
            => _db.Set<HrDepartment>().AsNoTracking().OrderBy(x => x.Name).ToListAsync();

        public Task<List<HrDepartment>> GetActiveAsync()
            => _db.Set<HrDepartment>().AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Name).ToListAsync();

        public Task<HrDepartment?> GetByIdAsync(Guid id)
            => _db.Set<HrDepartment>().FirstOrDefaultAsync(x => x.Id == id);

        public Task<bool> NameExistsAsync(string name, Guid? ignoreId = null)
            => _db.Set<HrDepartment>().AnyAsync(x => x.Name == name && (!ignoreId.HasValue || x.Id != ignoreId.Value));

        public async Task AddAsync(HrDepartment entity)
        {
            _db.Set<HrDepartment>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(HrDepartment entity)
        {
            _db.Set<HrDepartment>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
