using InfrastrfuctureManagmentCore.Domains.HR.Advanced;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.HR.Advanced
{
    public interface IHrSanctionRecordRepository
    {
        Task<List<HrSanctionRecord>> GetAllAsync(Guid? employeeId = null);
        Task<HrSanctionRecord?> GetByIdAsync(Guid id);
        Task AddAsync(HrSanctionRecord entity);
        Task UpdateAsync(HrSanctionRecord entity);
    }
}
