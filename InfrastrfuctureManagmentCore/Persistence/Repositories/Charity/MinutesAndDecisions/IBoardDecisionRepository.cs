using InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.MinutesAndDecisions
{
    public interface IBoardDecisionRepository
    {
        Task<List<BoardDecision>> GetByMeetingIdAsync(Guid boardMeetingId);
        Task<BoardDecision?> GetByIdAsync(Guid id);
        Task<BoardDecision?> GetByIdWithDetailsAsync(Guid id);
        Task AddAsync(BoardDecision entity);
        Task<IQueryable<BoardDecision>> GetAll();
        Task UpdateAsync(BoardDecision entity);
        Task<bool> DecisionNumberExistsAsync(string decisionNumber, Guid? excludeId = null);
    }
}
