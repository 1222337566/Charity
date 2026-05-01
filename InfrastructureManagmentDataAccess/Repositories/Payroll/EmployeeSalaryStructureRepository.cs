using InfrastrfuctureManagmentCore.Domains.Payroll;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Payroll;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Payroll
{
    public class EmployeeSalaryStructureRepository : IEmployeeSalaryStructureRepository
    {
        private readonly AppDbContext _db;
        public EmployeeSalaryStructureRepository(AppDbContext db) => _db = db;

        public Task<List<EmployeeSalaryStructure>> GetByEmployeeIdAsync(Guid employeeId)
            => _db.Set<EmployeeSalaryStructure>()
                .AsNoTracking()
                .Include(x => x.SalaryItemDefinition)
                .Where(x => x.EmployeeId == employeeId)
                .OrderByDescending(x => x.IsActive)
                .ThenBy(x => x.SalaryItemDefinition!.Name)
                .ToListAsync();

        public Task<EmployeeSalaryStructure?> GetByIdAsync(Guid id)
            => _db.Set<EmployeeSalaryStructure>()
                .AsNoTracking()
                .Include(x => x.SalaryItemDefinition)
                .Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(EmployeeSalaryStructure entity)
        {
            _db.Set<EmployeeSalaryStructure>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(EmployeeSalaryStructure entity)
        {
            _db.Set<EmployeeSalaryStructure>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
