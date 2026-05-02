using InfrastrfuctureManagmentCore.Domains.Charity.Stores;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Stores
{
    public interface ICharityStoreIssueRepository
    {
        Task<List<CharityStoreIssue>> GetAllAsync();
        Task<CharityStoreIssue?> GetByIdAsync(Guid id);
        Task<bool> NumberExistsAsync(string issueNumber);
        Task AddAsync(CharityStoreIssue entity);
    }
}
