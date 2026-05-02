using InfrastrfuctureManagmentCore.Domains.HR;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.HR
{
    public interface IHrShiftRepository
    {
        Task<List<HrShift>> GetAllAsync();
        Task<List<HrShift>> GetActiveAsync();
        Task<HrShift?> GetByIdAsync(Guid id);
        Task<bool> NameExistsAsync(string name, Guid? ignoreId = null);
        Task AddAsync(HrShift entity);
        Task UpdateAsync(HrShift entity);
    }
}
