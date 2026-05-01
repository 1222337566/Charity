using InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.MinutesAndDecisions;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class BoardDecisionRepository : IBoardDecisionRepository
    {
        private readonly AppDbContext _db;

        public BoardDecisionRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<BoardDecision>> GetByMeetingIdAsync(Guid boardMeetingId)
        {
            return await _db.Set<BoardDecision>()
                .Where(x => x.BoardMeetingId == boardMeetingId)
                .AsNoTracking()
                .OrderBy(x => x.DecisionNumber)
                .ToListAsync();
        }

        public async Task<BoardDecision?> GetByIdAsync(Guid id)
        {
            return await _db.Set<BoardDecision>().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<BoardDecision?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _db.Set<BoardDecision>()
                .Include(x => x.BoardMeeting)
                .Include(x => x.FollowUps)
                .Include(x => x.Attachments)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(BoardDecision entity)
        {
            _db.Set<BoardDecision>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(BoardDecision entity)
        {
            _db.Set<BoardDecision>().Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> DecisionNumberExistsAsync(string decisionNumber, Guid? excludeId = null)
        {
            return await _db.Set<BoardDecision>()
                .AnyAsync(x => x.DecisionNumber == decisionNumber &&
                    (!excludeId.HasValue || x.Id != excludeId.Value));
        }

        public async Task<IQueryable<BoardDecision>> GetAll()
        {
            return _db.Set<InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions.BoardDecision>();
        }
    }
}
