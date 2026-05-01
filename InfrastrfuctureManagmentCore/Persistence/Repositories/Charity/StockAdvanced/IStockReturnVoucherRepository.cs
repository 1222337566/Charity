using InfrastrfuctureManagmentCore.Domains.Charity.StockAdvanced;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.StockAdvanced
{
    public interface IStockReturnVoucherRepository
    {
        Task<List<StockReturnVoucher>> GetAllAsync();
        Task<StockReturnVoucher?> GetByIdAsync(Guid id);
        Task AddAsync(StockReturnVoucher entity);
    }
}
