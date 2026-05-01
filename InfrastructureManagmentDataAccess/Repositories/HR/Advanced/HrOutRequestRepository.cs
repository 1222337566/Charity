using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.HR.Advanced;
using InfrastrfuctureManagmentCore.Persistence.Repositories.HR.Advanced;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.HR.Advanced
{
    public class HrOutRequestRepository : IHrOutRequestRepository
    {
        private readonly AppDbContext _db;
        public HrOutRequestRepository(AppDbContext db) => _db = db;

        public async Task<List<HrOutRequest>> GetAllAsync(Guid? employeeId = null, string? status = null)
        {
            var query = _db.Set<HrOutRequest>()
                .AsNoTracking()
                .Include(x => x.Employee)
                .AsQueryable();

            if (employeeId.HasValue)
                query = query.Where(x => x.EmployeeId == employeeId.Value);
            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(x => x.Status == status);

            return await query.OrderByDescending(x => x.OutDate).ThenByDescending(x => x.FromTime).ToListAsync();
        }

        public Task<HrOutRequest?> GetByIdAsync(Guid id)
            => _db.Set<HrOutRequest>().FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(HrOutRequest entity)
        {
            _db.Set<HrOutRequest>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(HrOutRequest entity)
        {
            _db.Set<HrOutRequest>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
