using InfrastrfuctureManagmentCore.Domains.Charity.Funders;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Funders
{
    public interface IGrantInstallmentRepository
    {
        Task<List<GrantInstallment>> GetByGrantAgreementIdAsync(Guid grantAgreementId);
        Task<GrantInstallment?> GetByIdAsync(Guid id);
        Task<bool> InstallmentNumberExistsAsync(Guid grantAgreementId, int installmentNumber, Guid? excludeId = null);
        Task AddAsync(GrantInstallment installment);
        Task UpdateAsync(GrantInstallment installment);
    }
}
