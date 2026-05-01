using InfrastrfuctureManagmentCore.Domains.Payroll;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Payroll;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Payroll
{
    public class PayrollMonthRepository : IPayrollMonthRepository
    {
        private readonly AppDbContext _db;
        public PayrollMonthRepository(AppDbContext db) => _db = db;

        public Task<List<PayrollMonth>> GetAllAsync()
            => _db.Set<PayrollMonth>()
                .AsNoTracking()
                .Include(x => x.Employees)
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Month)
                .ToListAsync();

        public Task<PayrollMonth?> GetByIdAsync(Guid id)
            => _db.Set<PayrollMonth>()
                .AsNoTracking()
                .Include(x => x.Employees)
                .FirstOrDefaultAsync(x => x.Id == id);

        public Task<bool> ExistsAsync(int year, int month, Guid? ignoreId = null)
            => _db.Set<PayrollMonth>().AnyAsync(x => x.Year == year && x.Month == month && (!ignoreId.HasValue || x.Id != ignoreId.Value));

        public async Task AddAsync(PayrollMonth entity)
        {
            _db.Set<PayrollMonth>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(PayrollMonth entity)
        {
            _db.Set<PayrollMonth>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
