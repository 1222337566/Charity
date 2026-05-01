using InfrastrfuctureManagmentCore.Domains.Charity.Funders;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Funders
{
    public interface IGrantAgreementRepository
    {
        Task<List<GrantAgreement>> GetAllAsync();
        Task<List<GrantAgreement>> GetByFunderIdAsync(Guid funderId);
        Task<GrantAgreement?> GetByIdAsync(Guid id);
        Task<bool> AgreementNumberExistsAsync(string agreementNumber);
        Task<bool> AgreementNumberExistsAsync(string agreementNumber, Guid excludeId);
        Task AddAsync(GrantAgreement entity);
        Task UpdateAsync(GrantAgreement entity);
    }
}
