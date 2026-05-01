using InfrastrfuctureManagmentCore.Domains.HR.Rfp;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.HR.Rfp
{
    public interface IHrEmployeeBonusRepository
    {
        Task<List<HrEmployeeBonus>> GetAllAsync(Guid? employeeId = null);
        Task<HrEmployeeBonus?> GetByIdAsync(Guid id);
        Task AddAsync(HrEmployeeBonus entity);
        Task UpdateAsync(HrEmployeeBonus entity);
    }
}
