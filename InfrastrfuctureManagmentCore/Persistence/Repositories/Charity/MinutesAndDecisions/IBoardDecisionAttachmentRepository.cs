using InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.MinutesAndDecisions
{
    public interface IBoardDecisionAttachmentRepository
    {
        Task<List<BoardDecisionAttachment>> GetByDecisionIdAsync(Guid boardDecisionId);
        Task<BoardDecisionAttachment?> GetByIdAsync(Guid id);
        Task AddAsync(BoardDecisionAttachment entity);
        Task DeleteAsync(BoardDecisionAttachment entity);
    }
}
