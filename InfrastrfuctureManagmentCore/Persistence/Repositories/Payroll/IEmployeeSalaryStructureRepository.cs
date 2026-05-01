using InfrastrfuctureManagmentCore.Domains.Payroll;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Payroll
{
    public interface IEmployeeSalaryStructureRepository
    {
        Task<List<EmployeeSalaryStructure>> GetByEmployeeIdAsync(Guid employeeId);
        Task<EmployeeSalaryStructure?> GetByIdAsync(Guid id);
        Task AddAsync(EmployeeSalaryStructure entity);
        Task UpdateAsync(EmployeeSalaryStructure entity);
    }
}
