using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.HR.Rfp;
using InfrastrfuctureManagmentCore.Persistence.Repositories.HR.Rfp;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.HR.Rfp
{
    public class HrEmployeeFundingAssignmentRepository : IHrEmployeeFundingAssignmentRepository
    {
        private readonly AppDbContext _db;
        public HrEmployeeFundingAssignmentRepository(AppDbContext db) => _db = db;

        public async Task<List<HrEmployeeFundingAssignment>> GetAllAsync(Guid? employeeId = null)
        {
            var query = _db.Set<HrEmployeeFundingAssignment>().AsNoTracking().Include(x => x.Employee).AsQueryable();
            if (employeeId.HasValue)
                query = query.Where(x => x.EmployeeId == employeeId.Value);
            return await query.OrderByDescending(x => x.CreatedAtUtc).ToListAsync();
        }

        public Task<HrEmployeeFundingAssignment?> GetByIdAsync(Guid id)
            => _db.Set<HrEmployeeFundingAssignment>().FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(HrEmployeeFundingAssignment entity)
        {
            _db.Set<HrEmployeeFundingAssignment>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(HrEmployeeFundingAssignment entity)
        {
            _db.Set<HrEmployeeFundingAssignment>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
