using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.HR.Advanced;
using InfrastrfuctureManagmentCore.Persistence.Repositories.HR.Advanced;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.HR.Advanced
{
    public class HrEmployeeMovementRepository : IHrEmployeeMovementRepository
    {
        private readonly AppDbContext _db;
        public HrEmployeeMovementRepository(AppDbContext db) => _db = db;

        public async Task<List<HrEmployeeMovement>> GetAllAsync(Guid? employeeId = null)
        {
            var query = _db.Set<HrEmployeeMovement>()
                .AsNoTracking()
                .Include(x => x.Employee)
                .Include(x => x.FromDepartment)
                .Include(x => x.ToDepartment)
                .Include(x => x.FromJobTitle)
                .Include(x => x.ToJobTitle)
                .AsQueryable();

            if (employeeId.HasValue)
                query = query.Where(x => x.EmployeeId == employeeId.Value);

            return await query.OrderByDescending(x => x.EffectiveDate).ToListAsync();
        }

        public Task<HrEmployeeMovement?> GetByIdAsync(Guid id)
            => _db.Set<HrEmployeeMovement>().FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(HrEmployeeMovement entity)
        {
            _db.Set<HrEmployeeMovement>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(HrEmployeeMovement entity)
        {
            _db.Set<HrEmployeeMovement>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
