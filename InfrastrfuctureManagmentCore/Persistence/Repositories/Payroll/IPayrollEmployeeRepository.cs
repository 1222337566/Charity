using InfrastrfuctureManagmentCore.Domains.Payroll;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Payroll
{
    public interface IPayrollEmployeeRepository
    {
        Task<List<PayrollEmployee>> GetByPayrollMonthIdAsync(Guid payrollMonthId);
        Task<PayrollEmployee?> GetByIdAsync(Guid id);
        Task GenerateForMonthAsync(Guid payrollMonthId);
    }
}
