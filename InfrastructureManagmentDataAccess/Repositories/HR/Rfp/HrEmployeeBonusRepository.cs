using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.HR.Rfp;
using InfrastrfuctureManagmentCore.Persistence.Repositories.HR.Rfp;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.HR.Rfp
{
    public class HrEmployeeBonusRepository : IHrEmployeeBonusRepository
    {
        private readonly AppDbContext _db;
        public HrEmployeeBonusRepository(AppDbContext db) => _db = db;

        public async Task<List<HrEmployeeBonus>> GetAllAsync(Guid? employeeId = null)
        {
            var query = _db.Set<HrEmployeeBonus>().AsNoTracking().Include(x => x.Employee).AsQueryable();
            if (employeeId.HasValue)
                query = query.Where(x => x.EmployeeId == employeeId.Value);
            return await query.OrderByDescending(x => x.CreatedAtUtc).ToListAsync();
        }

        public Task<HrEmployeeBonus?> GetByIdAsync(Guid id)
            => _db.Set<HrEmployeeBonus>().FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(HrEmployeeBonus entity)
        {
            _db.Set<HrEmployeeBonus>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(HrEmployeeBonus entity)
        {
            _db.Set<HrEmployeeBonus>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
