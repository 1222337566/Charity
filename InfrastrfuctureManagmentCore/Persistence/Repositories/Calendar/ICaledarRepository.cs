using InfrastructureManagmentCore.Domains.Calendar;

namespace InfrastructureManagmentCore.Persistence.Repositories.Abstractions;

public interface ICalendarRepository : IRepository<CalendarEvent>
{
    Task CreateEventAsync(CalendarEvent ev, IEnumerable<string> attendeesUserIds, IEnumerable<int> reminders, CancellationToken ct = default);
    Task<List<CalendarEvent>> GetUserEventsAsync(string userId, CancellationToken ct = default);
    ///calendar respond
    ///
    Task<bool> RespondAsync(Guid eventId, string userId, string status, CancellationToken ct = default);
    Task<IEnumerable<CalendarEvent>> GetUserEventsAsync(string userId, DateTime startUtc, DateTime endUtc, CancellationToken ct);

    Task AddEventAsync(CalendarEvent ev, IEnumerable<string> attendeeUserIds, CancellationToken ct);

    Task UpdateEventAsync(CalendarEvent ev, CancellationToken ct);

    Task DeleteEventAsync(string userId, Guid eventId, CancellationToken ct); // مع تحقق الملكية/الصلاحيات داخل الريبو

    Task SetAttendeeStatusAsync(Guid eventId, string userId, string status, CancellationToken ct);

    Task<CalendarEvent?> GetByIdAsync(Guid eventId, CancellationToken ct);

    Task<IEnumerable<string>> GetEventAttendeesAsync(Guid eventId, CancellationToken ct);

    Task<bool> EventExistsAsync(Guid eventId, CancellationToken ct);
    Task<bool> IsAttendeeAsync(Guid eventId, string userId, CancellationToken ct);
    Task SetAttendeeStatusAsync(Guid eventId, string userId, string status, DateTime utcNow, CancellationToken ct);
}