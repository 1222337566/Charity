using InfrastrfuctureManagmentCore.Domains.HR.Advanced;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.HR.Advanced
{
    public interface IHrEmployeeMovementRepository
    {
        Task<List<HrEmployeeMovement>> GetAllAsync(Guid? employeeId = null);
        Task<HrEmployeeMovement?> GetByIdAsync(Guid id);
        Task AddAsync(HrEmployeeMovement entity);
        Task UpdateAsync(HrEmployeeMovement entity);
    }
}
