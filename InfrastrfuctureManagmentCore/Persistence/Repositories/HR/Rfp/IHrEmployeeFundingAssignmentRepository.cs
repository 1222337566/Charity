using InfrastrfuctureManagmentCore.Domains.HR.Rfp;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.HR.Rfp
{
    public interface IHrEmployeeFundingAssignmentRepository
    {
        Task<List<HrEmployeeFundingAssignment>> GetAllAsync(Guid? employeeId = null);
        Task<HrEmployeeFundingAssignment?> GetByIdAsync(Guid id);
        Task AddAsync(HrEmployeeFundingAssignment entity);
        Task UpdateAsync(HrEmployeeFundingAssignment entity);
    }
}
