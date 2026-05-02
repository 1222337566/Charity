using InfrastrfuctureManagmentCore.Domains.Payroll;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Payroll;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Payroll
{
    public class SalaryItemDefinitionRepository : ISalaryItemDefinitionRepository
    {
        private readonly AppDbContext _db;
        public SalaryItemDefinitionRepository(AppDbContext db) => _db = db;

        public Task<List<SalaryItemDefinition>> GetAllAsync()
            => _db.Set<SalaryItemDefinition>().AsNoTracking().OrderBy(x => x.Name).ToListAsync();

        public Task<List<SalaryItemDefinition>> GetActiveAsync()
            => _db.Set<SalaryItemDefinition>().AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Name).ToListAsync();

        public Task<SalaryItemDefinition?> GetByIdAsync(Guid id)
            => _db.Set<SalaryItemDefinition>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        public Task<bool> NameExistsAsync(string name, Guid? ignoreId = null)
            => _db.Set<SalaryItemDefinition>().AnyAsync(x => x.Name == name && (!ignoreId.HasValue || x.Id != ignoreId.Value));

        public async Task AddAsync(SalaryItemDefinition entity)
        {
            _db.Set<SalaryItemDefinition>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(SalaryItemDefinition entity)
        {
            _db.Set<SalaryItemDefinition>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
