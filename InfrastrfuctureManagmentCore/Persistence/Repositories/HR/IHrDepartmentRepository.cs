using InfrastrfuctureManagmentCore.Domains.HR;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.HR
{
    public interface IHrDepartmentRepository
    {
        Task<List<HrDepartment>> GetAllAsync();
        Task<List<HrDepartment>> GetActiveAsync();
        Task<HrDepartment?> GetByIdAsync(Guid id);
        Task<bool> NameExistsAsync(string name, Guid? ignoreId = null);
        Task AddAsync(HrDepartment entity);
        Task UpdateAsync(HrDepartment entity);
    }
}
