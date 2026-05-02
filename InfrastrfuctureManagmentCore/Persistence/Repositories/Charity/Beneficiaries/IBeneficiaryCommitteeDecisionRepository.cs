using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries
{
    public interface IBeneficiaryCommitteeDecisionRepository
    {
        Task<List<BeneficiaryCommitteeDecision>> GetByBeneficiaryIdAsync(Guid beneficiaryId);
        Task<BeneficiaryCommitteeDecision?> GetByIdAsync(Guid id);
        Task AddAsync(BeneficiaryCommitteeDecision entity);
        Task UpdateAsync(BeneficiaryCommitteeDecision entity);
    }
}
