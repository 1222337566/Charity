using InfrastructureManagmentWebFramework.Models.Charity.HumanitarianResearch;
using InfrastrfuctureManagmentCore.Domains.Charity.HumanitarianResearch;
using System.Security.Claims;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.HumanitarianResearch;

public interface IBeneficiaryHumanitarianResearchRepository
{
    Task<IReadOnlyList<BeneficiaryHumanitarianResearch>> GetAllAsync();
    Task<BeneficiaryHumanitarianResearch?> GetByIdAsync(Guid id);
    Task<BeneficiaryHumanitarianResearch?> GetByIdWithDetailsAsync(Guid id);
    Task<BeneficiaryHumanitarianResearch?> GetByIdForWorkflowAsync(Guid id);
    
     Task<bool> CodeExistsAsync(string code);
    Task<bool> ResearchNumberExistsAsync(string researchNumber);

    Task AddAsync(BeneficiaryHumanitarianResearch entity);
    Task AddCommitteeEvaluationAsync(BeneficiaryHumanitarianResearchCommitteeEvaluation entity);
    Task UpdateBeneficiaryStatusAsync(Guid beneficiaryId, Guid statusId);
    Task UpdateAsync(BeneficiaryHumanitarianResearch entity);
    Task SaveAsync();

    Task<IReadOnlyList<BeneficiaryHumanitarianResearchListItemVm>> GetListAsync();
    Task<IReadOnlyList<BeneficiaryHumanitarianResearchListItemVm>> GetListByBeneficiaryAsync(Guid beneficiaryId);
    Task<Guid> CreateAsync(CreateBeneficiaryHumanitarianResearchVm vm, ClaimsPrincipal user);
}
