using InfrastructureManagmentCore.Domains.Calendar;
using InfrastructureManagmentCore.Persistence.Repositories.Abstractions;
using InfrastructureManagmentCore.Services.Calendar;
using InfrastructureManagmentCore.Services.Notifications;
using InfrastructureManagmentRealtime.DTOs;
using Microsoft.EntityFrameworkCore;

public class CalendarService : ICalendarService
{
    private readonly ICalendarRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly INotificationService _notify;

    public CalendarService(ICalendarRepository repo, IUnitOfWork uow, INotificationService notify)
    { _repo = repo; _uow = uow; _notify = notify; }

    public async Task<Guid> CreateAsync(string createdByUserId, string title, string? desc, DateTime startUtc, DateTime endUtc, IEnumerable<string> attendees, IEnumerable<int> reminders, CancellationToken ct = default)
    {
        var ev = new CalendarEvent { Title = title, Description = desc, StartUtc = startUtc, EndUtc = endUtc, CreatedByUserId = createdByUserId };
        await _repo.CreateEventAsync(ev, attendees, reminders, ct);
        await _uow.SaveChangesAsync(ct);

        foreach (var uid in attendees ?? Enumerable.Empty<string>())
            await _notify.ToUserAsync(createdByUserId, uid, "Calendar", $"دعوة: {title}", level: "info", ct: ct);

        return ev.Id;
    }

    public Task<List<CalendarEvent>> GetMyAsync(string userId, CancellationToken ct = default)
        => _repo.GetUserEventsAsync(userId, ct);

    public async  Task<bool> RespondAsync(Guid eventId, string userId, string status, CancellationToken ct = default)
    {
        var ok = await _repo.RespondAsync(eventId, userId, status, ct);
        if (!ok) return false;

        // إشعار لصاحب الحدث
        var ev = await _repo.Query()
            .Include(e => e.Attendees)
            .AsNoTracking()
            .SingleOrDefaultAsync(e => e.Id == eventId, ct);

        if (ev != null)
        {
            await _notify.ToUserAsync(
                createdByUserId: userId,
                toUserId: ev.CreatedByUserId,
                title: "Calendar Update",
                message: $"قام مستخدم بالرد على دعوتك ({status.ToUpper()}) - {ev.Title}",
                level: status == "accepted" ? "success" :
                       status == "declined" ? "error" : "info",
                ct: ct
            );
        }

        return true;
    }

    public async Task<IEnumerable<CalendarEventDto>> QueryAsync(
        string userId, DateTime startUtc, DateTime endUtc, CancellationToken ct = default)
    {
        var items = await _repo.GetUserEventsAsync(userId, startUtc, endUtc, ct);

        // ماب بسيط للـ DTO
        return items.Select(e => new CalendarEventDto(
            e.Id,
            e.Title,
            e.StartUtc,
            e.EndUtc,
            e.AllDay,
            e.Color
        ));
    }

    // POST /calendar/create
    public async Task<Guid> CreateAsync(
        string ownerUserId, CreateEventDto dto, CancellationToken ct = default)
    {
        var ev = new CalendarEvent
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            StartUtc = dto.StartUtc,
            EndUtc = dto.EndUtc,
            AllDay = false,
            CreatedByUserId = ownerUserId,
            Color = null,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _repo.AddEventAsync(ev, dto.AttendeesUserIds ?? Enumerable.Empty<string>(), ct);
        await _uow.SaveChangesAsync(ct);

        // إرسال دعوات للمدعوين (أقل تغيير ممكن)
        foreach (var attendeeId in dto.AttendeesUserIds ?? Enumerable.Empty<string>())
        {
            // لو عندك SendCalendarInviteAsync استخدمها؛ وإلا استعمل ToUserAsync مباشرة:
            await _notify.ToUserAsync(
                createdByUserId: ownerUserId,
                toUserId: attendeeId,
                title: "Calendar Invite",
                message: $"دعوة إلى: {ev.Title} • {ev.StartUtc:yyyy-MM-dd HH:mm} UTC",
                url: $"/calendar/event/{ev.Id}",
                icon: null,
                level: "info",
                ct: ct
            );
        }

        return ev.Id;
    }

    // PUT /calendar/update
    public async Task UpdateAsync(
        string userId, UpdateEventDto dto, CancellationToken ct = default)
    {
        var current = await _repo.GetByIdAsync(dto.Id, ct) ?? throw new KeyNotFoundException("Event not found");

        // تأكد من صلاحيات التعديل (مالك الحدث غالبًا)
        if (!string.Equals(current.CreatedByUserId, userId, StringComparison.Ordinal))
            throw new UnauthorizedAccessException("You are not the owner of this event");

        current.Title = dto.Title;
        current.StartUtc = dto.StartUtc;
        current.EndUtc = dto.EndUtc;
        current.UpdatedAtUtc = DateTime.UtcNow;

        await _repo.UpdateEventAsync(current, ct);
        await _uow.SaveChangesAsync(ct);

        // إخطار المدعوين بالتحديث (اختياري)
        var attendees = await _repo.GetEventAttendeesAsync(current.Id, ct);
        foreach (var uid in attendees)
        {
            await _notify.ToUserAsync(
                createdByUserId: userId,
                toUserId: uid,
                title: "Calendar Update",
                message: $"تم تحديث: {current.Title} • {current.StartUtc:yyyy-MM-dd HH:mm} UTC",
                url: $"/calendar/event/{current.Id}",
                icon: null,
                level: "info",
                ct: ct
            );
        }
    }

    // DELETE /calendar/{id}
    public async Task DeleteAsync(
        string userId, Guid eventId, CancellationToken ct = default)
    {
        // داخل الريبو: تحقّق من الملكية واحذف
        await _repo.DeleteEventAsync(userId, eventId, ct);
        await _uow.SaveChangesAsync(ct);

        // (اختياري) إخطار المدعوين بالحذف
        var attendees = await _repo.GetEventAttendeesAsync(eventId, ct);
        foreach (var uid in attendees)
        {
            await _notify.ToUserAsync(
                createdByUserId: userId,
                toUserId: uid,
                title: "Calendar Update",
                message: $"تم إلغاء الحدث",
                url: $"/calendar",
                icon: null,
                level: "warning",
                ct: ct
            );
        }
    }

    // POST /calendar/respond
    public async Task RespondAsync(
        string userId, RespondDto dto, CancellationToken ct = default)
    {
        // حفظ حالة ردّ المدعو
        await _repo.SetAttendeeStatusAsync(dto.EventId, userId, dto.Status, ct);
        await _uow.SaveChangesAsync(ct);

        // إخطار مالك الحدث بردّ المدعو
        var ev = await _repo.GetByIdAsync(dto.EventId, ct) ?? throw new KeyNotFoundException("Event not found");
        var ownerId = ev.CreatedByUserId;

        await _notify.ToUserAsync(
            createdByUserId: userId,
            toUserId: ownerId,
            title: "Calendar Update",
            message: $"تم الرد ({dto.Status.ToUpper()}): {ev.Title}",
            url: $"/calendar/event/{ev.Id}",
            icon: null,
            level: dto.Status.Equals("accepted", StringComparison.OrdinalIgnoreCase) ? "success"
                 : dto.Status.Equals("declined", StringComparison.OrdinalIgnoreCase) ? "danger" : "info",
            ct: ct
        );
    }

    public async Task RespondAsync1(Guid eventId, string userId, string status, CancellationToken ct)
    {
        if (!await _repo.EventExistsAsync(eventId, ct)) throw new InvalidOperationException("Event not found");
        if (!await _repo.IsAttendeeAsync(eventId, userId, ct)) throw new UnauthorizedAccessException("Not attendee");

        await _repo.SetAttendeeStatusAsync(eventId, userId, status, DateTime.UtcNow, ct);
        await _uow.SaveChangesAsync(ct);

        // إشعار لمنشئ الحدث (مثال)
        await _notify.ToUserAsync(
            toUserId: /* ownerId */ userId,                  // عدّلها لمالك الحدث
            createdByUserId: userId,
            title: "Calendar response",
            message: $"User responded: {status}",
            url: $"/calendar/event/{eventId}",
            icon: "calendar-respond",
            level: status == "accepted" ? "success" : (status == "declined" ? "warning" : "info"),
            ct: ct
        );
    }
}