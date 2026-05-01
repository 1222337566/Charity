using Microsoft.EntityFrameworkCore;
using InfrastructureManagmentCore.Domains.Calendar;
using InfrastructureManagmentCore.Persistence.Repositories.Abstractions;
using InfrastructureManagmentDataAccess.EntityFramework;

namespace InfrastructureManagmentCore.Persistence.Repositories.Ef;

public class CalendarRepository : EfRepository<CalendarEvent>, ICalendarRepository
{
    public CalendarRepository(AppDbContext db) : base(db) { }

    public async Task CreateEventAsync(CalendarEvent ev, IEnumerable<string> attendeesUserIds, IEnumerable<int> reminders, CancellationToken ct = default)
    {
        await _set.AddAsync(ev, ct);

        foreach (var uid in attendeesUserIds ?? Enumerable.Empty<string>())
            _db.CalendarAttendees.Add(new CalendarAttendee { Event = ev, UserId = uid, Status = "invited" });

        foreach (var m in reminders ?? Enumerable.Empty<int>())
            _db.EventReminders.Add(new EventReminder { Event = ev, MinutesBefore = m, Channel = "signalr" });
    }

    public Task<List<CalendarEvent>> GetUserEventsAsync(string userId, CancellationToken ct = default)
    {
        var owns = _db.CalendarEvents.AsNoTracking().Where(e => e.CreatedByUserId == userId);
        var invited = _db.CalendarAttendees.AsNoTracking().Where(a => a.UserId == userId).Select(a => a.Event);
        return owns.Union(invited).OrderBy(e => e.StartUtc).ToListAsync(ct);
    }

    public Task<bool> EventExistsAsync(Guid eventId, CancellationToken ct)
       => _db.CalendarEvents.AnyAsync(e => e.Id == eventId, ct);

    public Task<bool> IsAttendeeAsync(Guid eventId, string userId, CancellationToken ct)
        => _db.CalendarAttendees.AnyAsync(a => a.EventId == eventId && a.UserId == userId, ct);

    public async Task SetAttendeeStatusAsync(Guid eventId, string userId, string status, DateTime utcNow, CancellationToken ct)
    {
        var att = await _db.CalendarAttendees
            .FirstAsync(a => a.EventId == eventId && a.UserId == userId, ct);
        att.Status = status;           // accepted/declined/tentative
        att.Event.UpdatedAtUtc = utcNow;
        // لا تنسَ تتأكد إن الأعمدة موجودة في الـMigration
    }

    public async Task<bool> RespondAsync(Guid eventId, string userId, string status, CancellationToken ct = default)
    {
        var att = await _db.CalendarAttendees
        .SingleOrDefaultAsync(a => a.EventId == eventId && a.UserId == userId, ct);

        if (att is null) return false;

        att.Status = status;
        await _db.SaveChangesAsync(ct);
        return true;
    }
    public async Task<IEnumerable<CalendarEvent>> GetUserEventsAsync(string userId, DateTime startUtc, DateTime endUtc, CancellationToken ct = default)
    {
        return await _db.CalendarEvents
            .Include(e => e.Attendees)
            .Where(e =>
                e.CreatedByUserId == userId ||
                e.Attendees.Any(a => a.UserId == userId))
            .Where(e => e.StartUtc < endUtc && (e.EndUtc == null || e.EndUtc > startUtc))
            .ToListAsync(ct);
    }

    public async Task AddEventAsync(CalendarEvent ev, IEnumerable<string> attendeeUserIds, CancellationToken ct = default)
    {
        await _db.CalendarEvents.AddAsync(ev, ct);
        if (attendeeUserIds != null)
        {
            foreach (var id in attendeeUserIds)
            {
                await _db.CalendarAttendees.AddAsync(new CalendarAttendee
                {
                    Id = Guid.NewGuid(),
                    EventId = ev.Id,
                    UserId = id,
                    Status = "invited"
                }, ct);
            }
        }
    }

    public async Task UpdateEventAsync(CalendarEvent ev, CancellationToken ct = default)
    {
        _db.CalendarEvents.Update(ev);
        await Task.CompletedTask;
    }

    public async Task DeleteEventAsync(string userId, Guid eventId, CancellationToken ct = default)
    {
        var ev = await _db.CalendarEvents
            .FirstOrDefaultAsync(e => e.Id == eventId && e.CreatedByUserId == userId, ct);
        if (ev != null)
        {
            var attendees = _db.CalendarAttendees.Where(a => a.EventId == eventId);
            _db.CalendarAttendees.RemoveRange(attendees);
            _db.CalendarEvents.Remove(ev);
        }
    }

    public async Task<CalendarEvent?> GetByIdAsync(Guid eventId, CancellationToken ct = default)
    {
        return await _db.CalendarEvents
            .Include(e => e.Attendees)
            .FirstOrDefaultAsync(e => e.Id == eventId, ct);
    }

    public async Task<IEnumerable<string>> GetEventAttendeesAsync(Guid eventId, CancellationToken ct = default)
    {
        return await _db.CalendarAttendees
            .Where(a => a.EventId == eventId)
            .Select(a => a.UserId)
            .ToListAsync(ct);
    }

    public async Task SetAttendeeStatusAsync(Guid eventId, string userId, string status, CancellationToken ct = default)
    {
        var attendee = await _db.CalendarAttendees
            .FirstOrDefaultAsync(a => a.EventId == eventId && a.UserId == userId, ct);
        if (attendee != null)
        {
            attendee.Status = status;
            _db.CalendarAttendees.Update(attendee);
        }
    }
}
