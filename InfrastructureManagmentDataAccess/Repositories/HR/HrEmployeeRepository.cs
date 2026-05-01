using InfrastrfuctureManagmentCore.Domains.HR;
using InfrastrfuctureManagmentCore.Persistence.Repositories.HR;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.HR
{
    public class HrEmployeeRepository : IHrEmployeeRepository
    {
        private readonly AppDbContext _db;
        public HrEmployeeRepository(AppDbContext db) => _db = db;

        public Task<List<HrEmployee>> GetActiveAsync()
            => _db.Set<HrEmployee>()
                .AsNoTracking()
                .Include(x => x.Department)
                .Include(x => x.JobTitle)
                .Where(x => x.IsActive)
                .OrderBy(x => x.FullName)
                .ToListAsync();

        public Task<HrEmployee?> GetByIdAsync(Guid id)
            => _db.Set<HrEmployee>()
                .AsNoTracking()
                .Include(x => x.Department)
                .Include(x => x.JobTitle)
                .Include(x => x.AttendanceRecords.OrderByDescending(a => a.AttendanceDate).Take(10))
                .ThenInclude(a => a.Shift)
                .FirstOrDefaultAsync(x => x.Id == id);

        public Task<bool> CodeExistsAsync(string code, Guid? ignoreId = null)
            => _db.Set<HrEmployee>().AnyAsync(x => x.Code == code && (!ignoreId.HasValue || x.Id != ignoreId.Value));

        public async Task AddAsync(HrEmployee entity)
        {
            _db.Set<HrEmployee>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(HrEmployee entity)
        {
            _db.Set<HrEmployee>().Update(entity);
            await _db.SaveChangesAsync();
        }

        public Task<List<HrEmployee>> SearchAsync(string? q, string? status, bool? isActive)
        {
            var query = _db.Set<HrEmployee>()
                .AsNoTracking()
                .Include(x => x.Department)
                .Include(x => x.JobTitle)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(x => x.FullName.Contains(q) || x.Code.Contains(q) || (x.NationalId ?? "").Contains(q) || (x.PhoneNumber ?? "").Contains(q));
            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(x => x.Status == status);
            if (isActive.HasValue)
                query = query.Where(x => x.IsActive == isActive.Value);

            return query.OrderBy(x => x.FullName).ToListAsync();
        }
    }
}
