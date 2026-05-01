using InfrastrfuctureManagmentCore.Domains.HR;
using InfrastrfuctureManagmentCore.Persistence.Repositories.HR;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.HR
{
    public class HrAttendanceRecordRepository : IHrAttendanceRecordRepository
    {
        private readonly AppDbContext _db;
        public HrAttendanceRecordRepository(AppDbContext db) => _db = db;

        public Task<HrAttendanceRecord?> GetByIdAsync(Guid id)
            => _db.Set<HrAttendanceRecord>()
                .AsNoTracking()
                .Include(x => x.Employee)
                    .ThenInclude(x => x.Department)
                .Include(x => x.Shift)
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(HrAttendanceRecord entity)
        {
            _db.Set<HrAttendanceRecord>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(HrAttendanceRecord entity)
        {
            _db.Set<HrAttendanceRecord>().Update(entity);
            await _db.SaveChangesAsync();
        }

        public Task<List<HrAttendanceRecord>> SearchAsync(Guid? employeeId, DateTime? fromDate, DateTime? toDate, string? status)
        {
            var query = _db.Set<HrAttendanceRecord>()
                .AsNoTracking()
                .Include(x => x.Employee)
                    .ThenInclude(x => x.Department)
                .Include(x => x.Shift)
                .AsQueryable();

            if (employeeId.HasValue)
                query = query.Where(x => x.EmployeeId == employeeId.Value);
            if (fromDate.HasValue)
                query = query.Where(x => x.AttendanceDate.Date >= fromDate.Value.Date);
            if (toDate.HasValue)
                query = query.Where(x => x.AttendanceDate.Date <= toDate.Value.Date);
            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(x => x.Status == status);

            return query.OrderByDescending(x => x.AttendanceDate).ThenByDescending(x => x.CreatedAtUtc).ToListAsync();
        }
    }
}
