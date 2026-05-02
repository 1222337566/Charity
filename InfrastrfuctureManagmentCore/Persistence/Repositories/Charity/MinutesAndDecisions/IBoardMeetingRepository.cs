using InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.MinutesAndDecisions
{
    public interface IBoardMeetingRepository
    {
        Task<List<BoardMeeting>> GetAllAsync();
        Task<BoardMeeting?> GetByIdAsync(Guid id);
        Task<BoardMeeting?> GetByIdWithDetailsAsync(Guid id);
        Task AddAsync(BoardMeeting entity);
        Task UpdateAsync(BoardMeeting entity);
        Task<bool> MeetingNumberExistsAsync(string meetingNumber, Guid? excludeId = null);
    }
}
