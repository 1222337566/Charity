using InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.ProjectProposals
{
    public interface IProjectProposalRepository
    {
        Task<List<ProjectProposal>> GetAllAsync();
        Task<ProjectProposal?> GetByIdAsync(Guid id);
        Task<ProjectProposal?> GetByIdWithDetailsAsync(Guid id);
        Task AddAsync(ProjectProposal entity);
        Task UpdateAsync(ProjectProposal entity);
        Task<bool> ProposalNumberExistsAsync(string proposalNumber, Guid? excludeId = null);
        Task UpdateStatusAsync(Guid id, string status);
        Task<ProjectProposalAttachment?> GetAttachmentContentAsync(Guid attachmentId);
    }
}
