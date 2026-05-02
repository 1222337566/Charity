using InfrastrfuctureManagmentCore.Domains.HR.Advanced;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.HR.Advanced
{
    public interface IHrOutRequestRepository
    {
        Task<List<HrOutRequest>> GetAllAsync(Guid? employeeId = null, string? status = null);
        Task<HrOutRequest?> GetByIdAsync(Guid id);
        Task AddAsync(HrOutRequest entity);
        Task UpdateAsync(HrOutRequest entity);
    }
}
