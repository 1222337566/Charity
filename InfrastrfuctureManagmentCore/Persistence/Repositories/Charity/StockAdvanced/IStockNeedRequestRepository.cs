using InfrastrfuctureManagmentCore.Domains.Charity.StockAdvanced;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.StockAdvanced
{
    public interface IStockNeedRequestRepository
    {
        Task<List<StockNeedRequest>> GetAllAsync();
        Task<StockNeedRequest?> GetByIdAsync(Guid id);
        Task AddAsync(StockNeedRequest entity);
        Task UpdateAsync(StockNeedRequest entity);
    }
}
