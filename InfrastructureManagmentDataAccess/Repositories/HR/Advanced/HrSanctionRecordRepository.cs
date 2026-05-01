using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.HR.Advanced;
using InfrastrfuctureManagmentCore.Persistence.Repositories.HR.Advanced;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.HR.Advanced
{
    public class HrSanctionRecordRepository : IHrSanctionRecordRepository
    {
        private readonly AppDbContext _db;
        public HrSanctionRecordRepository(AppDbContext db) => _db = db;

        public async Task<List<HrSanctionRecord>> GetAllAsync(Guid? employeeId = null)
        {
            var query = _db.Set<HrSanctionRecord>()
                .AsNoTracking()
                .Include(x => x.Employee)
                .AsQueryable();

            if (employeeId.HasValue)
                query = query.Where(x => x.EmployeeId == employeeId.Value);

            return await query.OrderByDescending(x => x.SanctionDate).ToListAsync();
        }

        public Task<HrSanctionRecord?> GetByIdAsync(Guid id)
            => _db.Set<HrSanctionRecord>().FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(HrSanctionRecord entity)
        {
            _db.Set<HrSanctionRecord>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(HrSanctionRecord entity)
        {
            _db.Set<HrSanctionRecord>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
