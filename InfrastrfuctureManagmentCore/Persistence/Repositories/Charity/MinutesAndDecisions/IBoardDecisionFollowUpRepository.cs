using InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.MinutesAndDecisions
{
    public interface IBoardDecisionFollowUpRepository
    {
        Task<List<BoardDecisionFollowUp>> GetByDecisionIdAsync(Guid boardDecisionId);
        Task AddAsync(BoardDecisionFollowUp entity);
    }
}
