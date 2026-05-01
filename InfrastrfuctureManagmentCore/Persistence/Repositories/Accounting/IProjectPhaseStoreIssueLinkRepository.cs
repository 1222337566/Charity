using InfrastrfuctureManagmentCore.Domains.Accounting;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting
{
    public interface IProjectPhaseStoreIssueLinkRepository
    {
        Task<List<ProjectPhaseStoreIssueLink>> GetByProjectIdAsync(Guid projectId);
        Task<List<ProjectPhaseStoreIssueLink>> GetByPhaseIdAsync(Guid phaseId);
        Task<ProjectPhaseStoreIssueLink?> GetByStoreIssueIdAsync(Guid storeIssueId);
        Task AddAsync(ProjectPhaseStoreIssueLink entity);
        Task DeleteAsync(Guid id);
    }
}
