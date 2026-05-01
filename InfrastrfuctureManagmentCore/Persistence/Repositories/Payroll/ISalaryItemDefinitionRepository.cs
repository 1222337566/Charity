using InfrastrfuctureManagmentCore.Domains.Payroll;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Payroll
{
    public interface ISalaryItemDefinitionRepository
    {
        Task<List<SalaryItemDefinition>> GetAllAsync();
        Task<List<SalaryItemDefinition>> GetActiveAsync();
        Task<SalaryItemDefinition?> GetByIdAsync(Guid id);
        Task<bool> NameExistsAsync(string name, Guid? ignoreId = null);
        Task AddAsync(SalaryItemDefinition entity);
        Task UpdateAsync(SalaryItemDefinition entity);
    }
}
