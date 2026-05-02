using InfrastrfuctureManagmentCore.Domains.Charity.StockAdvanced;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.StockAdvanced
{
    public interface IStockDisposalVoucherRepository
    {
        Task<List<StockDisposalVoucher>> GetAllAsync();
        Task<StockDisposalVoucher?> GetByIdAsync(Guid id);
        Task AddAsync(StockDisposalVoucher entity);
    }
}
