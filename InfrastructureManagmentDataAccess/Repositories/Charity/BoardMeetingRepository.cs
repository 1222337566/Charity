using InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.MinutesAndDecisions;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class BoardMeetingRepository : IBoardMeetingRepository
    {
        private readonly AppDbContext _db;

        public BoardMeetingRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<BoardMeeting>> GetAllAsync()
        {
            return await _db.Set<BoardMeeting>()
                .AsNoTracking()
                .OrderByDescending(x => x.MeetingDate)
                .ThenByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<BoardMeeting?> GetByIdAsync(Guid id)
        {
            return await _db.Set<BoardMeeting>()
                .Include(x => x.Minute)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<BoardMeeting?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _db.Set<BoardMeeting>()
                .Include(x => x.Minute)
                .Include(x => x.Attendees)
                .Include(x => x.Attachments)
                .Include(x => x.Decisions)
                    .ThenInclude(x => x.FollowUps)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(BoardMeeting entity)
        {
            _db.Set<BoardMeeting>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(BoardMeeting entity)
        {
            _db.Set<BoardMeeting>().Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> MeetingNumberExistsAsync(string meetingNumber, Guid? excludeId = null)
        {
            return await _db.Set<BoardMeeting>()
                .AnyAsync(x => x.MeetingNumber == meetingNumber &&
                    (!excludeId.HasValue || x.Id != excludeId.Value));
        }
    }
}
