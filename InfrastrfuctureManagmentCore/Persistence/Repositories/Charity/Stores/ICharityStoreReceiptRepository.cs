using InfrastrfuctureManagmentCore.Domains.Charity.Stores;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Stores
{
    public interface ICharityStoreReceiptRepository
    {
        Task<List<CharityStoreReceipt>> GetAllAsync();
        Task<CharityStoreReceipt?> GetByIdAsync(Guid id);
        Task<bool> NumberExistsAsync(string receiptNumber);
        Task AddAsync(CharityStoreReceipt entity);
    }
}
