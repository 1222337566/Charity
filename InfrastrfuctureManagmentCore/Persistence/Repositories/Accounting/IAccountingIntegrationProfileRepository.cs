using InfrastrfuctureManagmentCore.Domains.Accounting.Integration;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting
{
    public interface IAccountingIntegrationProfileRepository
    {
        Task<List<AccountingIntegrationProfile>> GetAllAsync();
        Task<AccountingIntegrationProfile?> GetByIdAsync(Guid id);
        Task<AccountingIntegrationProfile?> GetBySourceTypeAsync(string sourceType);
        Task<List<AccountingIntegrationProfile>> GetActiveBySourceTypeAsync(string sourceType);
        Task AddAsync(AccountingIntegrationProfile entity);
        Task UpdateAsync(AccountingIntegrationProfile entity);
        Task SaveChangesAsync();
    }
}
