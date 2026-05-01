using InfrastructureManagmentCore.Domains.Calendar;
using InfrastructureManagmentCore.Persistence.Repositories.Abstractions;
using InfrastructureManagmentCore.Services.Notifications;
using InfrastructureManagmentRealtime.DTOs;

namespace InfrastructureManagmentCore.Services.Calendar;

public interface ICalendarService
{
    Task<Guid> CreateAsync(string createdByUserId, string title, string? desc, DateTime startUtc, DateTime endUtc, IEnumerable<string> attendees, IEnumerable<int> reminders, CancellationToken ct = default);
    Task<List<CalendarEvent>> GetMyAsync(string userId, CancellationToken ct = default);
    ////////////////////calendar respond
    ///
    Task<bool> RespondAsync(Guid eventId, string userId, string status, CancellationToken ct = default);
    Task<IEnumerable<CalendarEventDto>> QueryAsync(
            string userId, DateTime startUtc, DateTime endUtc, CancellationToken ct = default);

    Task<Guid> CreateAsync(
        string ownerUserId, CreateEventDto dto, CancellationToken ct = default);

    Task UpdateAsync(
        string userId, UpdateEventDto dto, CancellationToken ct = default);

    Task DeleteAsync(
        string userId, Guid eventId, CancellationToken ct = default);

    Task RespondAsync(
        string userId, RespondDto dto, CancellationToken ct = default);

    Task RespondAsync1(Guid eventId, string userId, string status, CancellationToken ct);

}