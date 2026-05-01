using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.HR.Rfp;
using InfrastrfuctureManagmentCore.Persistence.Repositories.HR.Rfp;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.HR.Rfp
{
    public class HrEmployeeContractRepository : IHrEmployeeContractRepository
    {
        private readonly AppDbContext _db;
        public HrEmployeeContractRepository(AppDbContext db) => _db = db;

        public async Task<List<HrEmployeeContract>> GetAllAsync(Guid? employeeId = null)
        {
            var query = _db.Set<HrEmployeeContract>().AsNoTracking().Include(x => x.Employee).AsQueryable();
            if (employeeId.HasValue)
                query = query.Where(x => x.EmployeeId == employeeId.Value);
            return await query.OrderByDescending(x => x.CreatedAtUtc).ToListAsync();
        }

        public Task<HrEmployeeContract?> GetByIdAsync(Guid id)
            => _db.Set<HrEmployeeContract>().FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(HrEmployeeContract entity)
        {
            _db.Set<HrEmployeeContract>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(HrEmployeeContract entity)
        {
            _db.Set<HrEmployeeContract>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
