using InfrastrfuctureManagmentCore.Domains.Accounting;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting
{
    public interface IFiscalPeriodRepository
    {
        Task<List<FiscalPeriod>> GetAllAsync();
        Task<FiscalPeriod?> GetByIdAsync(Guid id);
        Task<FiscalPeriod?> GetCurrentAsync();
        Task<bool> CodeExistsAsync(string code);
        Task<bool> CodeExistsAsync(string code, Guid excludeId);
        Task<bool> HasOverlappingPeriodAsync(DateTime startDate, DateTime endDate, Guid? excludeId = null);
        Task AddAsync(FiscalPeriod entity);
        Task UpdateAsync(FiscalPeriod entity);
        Task ClearCurrentFlagAsync(Guid exceptId);
    }
}
