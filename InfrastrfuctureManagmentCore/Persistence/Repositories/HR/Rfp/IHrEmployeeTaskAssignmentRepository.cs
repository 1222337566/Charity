using InfrastrfuctureManagmentCore.Domains.HR.Rfp;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.HR.Rfp
{
    public interface IHrEmployeeTaskAssignmentRepository
    {
        Task<List<HrEmployeeTaskAssignment>> GetAllAsync(Guid? employeeId = null);
        Task<HrEmployeeTaskAssignment?> GetByIdAsync(Guid id);
        Task AddAsync(HrEmployeeTaskAssignment entity);
        Task UpdateAsync(HrEmployeeTaskAssignment entity);
    }
}
