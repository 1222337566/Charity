using InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.MinutesAndDecisions;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class BoardDecisionFollowUpRepository : IBoardDecisionFollowUpRepository
    {
        private readonly AppDbContext _db;

        public BoardDecisionFollowUpRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<BoardDecisionFollowUp>> GetByDecisionIdAsync(Guid boardDecisionId)
        {
            return await _db.Set<BoardDecisionFollowUp>()
                .Where(x => x.BoardDecisionId == boardDecisionId)
                .AsNoTracking()
                .OrderByDescending(x => x.FollowUpDate)
                .ThenByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task AddAsync(BoardDecisionFollowUp entity)
        {
            _db.Set<BoardDecisionFollowUp>().Add(entity);
            await _db.SaveChangesAsync();
        }
    }
}
