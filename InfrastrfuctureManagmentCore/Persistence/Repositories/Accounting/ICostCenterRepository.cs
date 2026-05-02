using InfrastrfuctureManagmentCore.Domains.Accounting;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting
{
    public interface ICostCenterRepository
    {
        Task<List<CostCenter>> GetAllAsync();
        Task<List<CostCenter>> GetAllParentsAsync();
        Task<CostCenter?> GetByIdAsync(Guid id);
        Task<bool> CodeExistsAsync(string code);
        Task<bool> CodeExistsAsync(string code, Guid excludeId);
        Task<bool> HasChildrenAsync(Guid id);
        Task<bool> HasActiveChildrenAsync(Guid id);
        Task AddAsync(CostCenter entity);
        Task UpdateAsync(CostCenter entity);
    }
}
