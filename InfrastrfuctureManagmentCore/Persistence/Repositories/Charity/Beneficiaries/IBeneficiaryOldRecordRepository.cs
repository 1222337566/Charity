using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries
{
    public interface IBeneficiaryOldRecordRepository
    {
        Task<List<BeneficiaryOldRecord>> GetByBeneficiaryIdAsync(Guid beneficiaryId);
        Task<BeneficiaryOldRecord?> GetByIdAsync(Guid id);
        Task AddAsync(BeneficiaryOldRecord entity);
        Task UpdateAsync(BeneficiaryOldRecord entity);
    }
}
