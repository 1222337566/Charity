using InfrastrfuctureManagmentCore.Domains.Notify;
using InfrastructureManagmentRealtime.DTOs;

namespace InfrastructureManagmentCore.Services.Notifications;

public interface INotificationService
{
    Task<Guid> ToUserAsync(string createdByUserId, string toUserId, string title, string message, string? url = null, string? icon = null, string level = "info", CancellationToken ct = default);
    Task ToTopicAsync(string createdByUserId, string topic, string title, string message, CancellationToken ct = default);
    Task ToAllAsync(string createdByUserId, string title, string message, CancellationToken ct = default);
    Task<List<object>> GetMyInboxAsync(string userId, bool unreadOnly, int take, CancellationToken ct = default);
    Task<bool> MarkReadAsync(Guid deliveryId, string userId, CancellationToken ct = default);
    // ====== جديد (اختياري لزر "Mark all read") ======
    Task<Guid> SendAsync(NotificationMessage msg, CancellationToken ct = default);
    // === جديد ===
    Task<int> MarkAllReadAsync(string userId, CancellationToken ct = default);
    Task<Guid> SendCalendarInviteAsync(string toUserId, Guid eventId, string title, DateTime startUtc, CancellationToken ct = default);
    Task<Guid> SendCalendarReminderAsync(string toUserId, Guid eventId, string title, int minutesBefore, CancellationToken ct = default);
    // لو عايز صفحة Inbox بفلاتر/ترقيم:
    public Task<Guid> SendCalendarRespondAsync(string ownerUserId, Guid eventId, string title, string status, CancellationToken ct = default);
    Task<NotifyQueryResult> QueryAsync(string userId, int skip, int take, string? level, string? kind, bool? read, CancellationToken ct);
}