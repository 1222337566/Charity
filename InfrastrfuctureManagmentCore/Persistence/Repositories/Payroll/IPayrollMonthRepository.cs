using InfrastrfuctureManagmentCore.Domains.Payroll;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Payroll
{
    public interface IPayrollMonthRepository
    {
        Task<List<PayrollMonth>> GetAllAsync();
        Task<PayrollMonth?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(int year, int month, Guid? ignoreId = null);
        Task AddAsync(PayrollMonth entity);
        Task UpdateAsync(PayrollMonth entity);
    }
}
