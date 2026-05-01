using InfrastrfuctureManagmentCore.Domains.HR;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.HR
{
    public interface IHrEmployeeRepository
    {
        Task<List<HrEmployee>> SearchAsync(string? q, string? status, bool? isActive);
        Task<List<HrEmployee>> GetActiveAsync();
        Task<HrEmployee?> GetByIdAsync(Guid id);
        Task<bool> CodeExistsAsync(string code, Guid? ignoreId = null);
        Task AddAsync(HrEmployee entity);
        Task UpdateAsync(HrEmployee entity);
    }
}
