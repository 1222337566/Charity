using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries;

public interface IBeneficiaryDocumentRepository
{
    Task<List<BeneficiaryDocument>> GetListAsync(Guid? beneficiaryId = null);
    Task<List<BeneficiaryDocument>> GetListByBeneficiaryAsync(Guid beneficiaryId);
    Task<BeneficiaryDocument?> GetByIdAsync(Guid id);
    Task AddAsync(BeneficiaryDocument entity);
    Task DeleteAsync(Guid id);
}
