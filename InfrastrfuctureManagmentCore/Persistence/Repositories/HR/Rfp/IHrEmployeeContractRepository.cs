using InfrastrfuctureManagmentCore.Domains.HR.Rfp;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.HR.Rfp
{
    public interface IHrEmployeeContractRepository
    {
        Task<List<HrEmployeeContract>> GetAllAsync(Guid? employeeId = null);
        Task<HrEmployeeContract?> GetByIdAsync(Guid id);
        Task AddAsync(HrEmployeeContract entity);
        Task UpdateAsync(HrEmployeeContract entity);
    }
}
