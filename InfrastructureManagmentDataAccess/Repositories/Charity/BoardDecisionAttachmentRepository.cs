using InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.MinutesAndDecisions;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class BoardDecisionAttachmentRepository : IBoardDecisionAttachmentRepository
    {
        private readonly AppDbContext _db;

        public BoardDecisionAttachmentRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task<List<BoardDecisionAttachment>> GetByDecisionIdAsync(Guid boardDecisionId)
        {
            return _db.Set<BoardDecisionAttachment>()
                .AsNoTracking()
                .Where(x => x.BoardDecisionId == boardDecisionId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }

        public Task<BoardDecisionAttachment?> GetByIdAsync(Guid id)
        {
            return _db.Set<BoardDecisionAttachment>().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(BoardDecisionAttachment entity)
        {
            _db.Set<BoardDecisionAttachment>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(BoardDecisionAttachment entity)
        {
            _db.Set<BoardDecisionAttachment>().Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
