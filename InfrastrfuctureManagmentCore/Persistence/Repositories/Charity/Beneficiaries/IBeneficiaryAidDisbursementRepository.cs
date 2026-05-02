using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries
{
    public interface IBeneficiaryAidDisbursementRepository
    {
        Task<List<BeneficiaryAidDisbursement>> GetAllAsync();
        Task<List<BeneficiaryAidDisbursement>> GetByBeneficiaryIdAsync(Guid beneficiaryId);
        Task<BeneficiaryAidDisbursement?> GetByIdAsync(Guid id);
        Task AddAsync(BeneficiaryAidDisbursement entity);
        Task UpdateAsync(BeneficiaryAidDisbursement entity);
    }
}
