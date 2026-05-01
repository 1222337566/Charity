using InfrastrfuctureManagmentCore.Domains.HR;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.HR
{
    public interface IHrJobTitleRepository
    {
        Task<List<HrJobTitle>> GetAllAsync();
        Task<List<HrJobTitle>> GetActiveAsync();
        Task<HrJobTitle?> GetByIdAsync(Guid id);
        Task<bool> NameExistsAsync(string name, Guid? ignoreId = null);
        Task AddAsync(HrJobTitle entity);
        Task UpdateAsync(HrJobTitle entity);
    }
}
