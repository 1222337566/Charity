using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries
{
    public interface IBeneficiaryFamilyMemberRepository
    {
        Task<List<BeneficiaryFamilyMember>> GetByBeneficiaryIdAsync(Guid beneficiaryId);
        Task<BeneficiaryFamilyMember?> GetByIdAsync(Guid id);
        Task AddAsync(BeneficiaryFamilyMember entity);
        Task UpdateAsync(BeneficiaryFamilyMember entity);
    }
}
