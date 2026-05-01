using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries
{
    public interface IBeneficiaryAssessmentRepository
    {
        Task<List<BeneficiaryAssessment>> GetByBeneficiaryIdAsync(Guid beneficiaryId);
        Task<BeneficiaryAssessment?> GetByIdAsync(Guid id);
        Task AddAsync(BeneficiaryAssessment entity);
        Task UpdateAsync(BeneficiaryAssessment entity);
    }
}
