using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.HR.Rfp;
using InfrastrfuctureManagmentCore.Persistence.Repositories.HR.Rfp;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.HR.Rfp
{
    public class HrEmployeeTaskAssignmentRepository : IHrEmployeeTaskAssignmentRepository
    {
        private readonly AppDbContext _db;
        public HrEmployeeTaskAssignmentRepository(AppDbContext db) => _db = db;

        public async Task<List<HrEmployeeTaskAssignment>> GetAllAsync(Guid? employeeId = null)
        {
            var query = _db.Set<HrEmployeeTaskAssignment>().AsNoTracking().Include(x => x.Employee).AsQueryable();
            if (employeeId.HasValue)
                query = query.Where(x => x.EmployeeId == employeeId.Value);
            return await query.OrderByDescending(x => x.CreatedAtUtc).ToListAsync();
        }

        public Task<HrEmployeeTaskAssignment?> GetByIdAsync(Guid id)
            => _db.Set<HrEmployeeTaskAssignment>().FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(HrEmployeeTaskAssignment entity)
        {
            _db.Set<HrEmployeeTaskAssignment>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(HrEmployeeTaskAssignment entity)
        {
            _db.Set<HrEmployeeTaskAssignment>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
