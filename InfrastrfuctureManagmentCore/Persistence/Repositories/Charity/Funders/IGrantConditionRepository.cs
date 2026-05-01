using InfrastrfuctureManagmentCore.Domains.Charity.Funders;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Funders
{
    public interface IGrantConditionRepository
    {
        Task<List<GrantCondition>> GetByGrantAgreementIdAsync(Guid grantAgreementId);
        Task<GrantCondition?> GetByIdAsync(Guid id);
        Task AddAsync(GrantCondition condition);
        Task UpdateAsync(GrantCondition condition);
    }
}
