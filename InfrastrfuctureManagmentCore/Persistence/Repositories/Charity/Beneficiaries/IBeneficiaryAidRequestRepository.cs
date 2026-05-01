using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries
{
    public interface IBeneficiaryAidRequestRepository
    {
        Task<List<BeneficiaryAidRequest>> GetAllAsync();
        Task<List<BeneficiaryAidRequest>> GetAidTypeAsync(Guid aidtypeid);
        Task<List<BeneficiaryAidRequest>> GetByBeneficiaryIdAsync(Guid beneficiaryId);
        Task<BeneficiaryAidRequest?> GetByIdAsync(Guid id);
        Task AddAsync(BeneficiaryAidRequest entity);
        Task UpdateAsync(BeneficiaryAidRequest entity);
    }
}
