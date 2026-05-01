using InfrastrfuctureManagmentCore.Domains.Notify;
using InfrastructureManagementRealtime.DTOs;
using InfrastructureManagementRealtime.Helpers;
using InfrastructureManagementRealtime.Hubs;
using InfrastructureManagmentCore.Domains.Notify;
using InfrastructureManagmentCore.Domains.Reactions;
using InfrastructureManagmentCore.Persistence.Repositories.Abstractions;
using InfrastructureManagmentRealtime.DTOs;
using Microsoft.AspNetCore.SignalR;
// INotificationBus (اللي عندك)
namespace InfrastructureManagmentCore.Services.Notifications;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly INotificationBus _bus;
    private readonly IHubContext<NotifyHub> _hub;
    public NotificationService(INotificationRepository repo, IUnitOfWork uow, INotificationBus bus, IHubContext<NotifyHub> hub)
    {
        _repo = repo; _uow = uow; _bus = bus;
        _hub = hub;
    }

    public async Task<Guid> ToUserAsync(string createdByUserId, string toUserId, string title, string message, string? url = null, string? icon = null, string level = "info", CancellationToken ct = default)
    {
        var n = new Notification2 { Title = title, Message = message, Url = url, Icon = icon, Level = level, Scope = "user", CreatedByUserId = createdByUserId, CreatedAtUtc = DateTime.UtcNow };
        await _repo.SaveWithDeliveriesAsync(n, new[] { toUserId }, ct);
        await _uow.SaveChangesAsync(ct);
        var deliveryId = await GetLatestDeliveryIdForUserAsync(toUserId, ct);

        // 3) ابعت الريل تايم ومعاه deliveryId
        await _bus.ToUserAsync(toUserId, new NotificationPayload(
            title: n.Title,
            message: n.Message,
            url: n.Url,
            icon: n.Icon,
            level: n.Level,
            ticks: DateTime.UtcNow.Ticks
        )
        {
            deliveryId = deliveryId
        });
       // await _bus.ToUserAsync(toUserId, new NotificationPayload(n.Title, n.Message, n.Url, n.Icon, n.Level, DateTime.UtcNow.Ticks));
        return n.Id;
    }
    private async Task<Guid> GetLatestDeliveryIdForUserAsync(string userId, CancellationToken ct)
    {
        // خيار 1: لو عندك DbContext مباشر (بدّل AppDbContext باسمك)
        // return await _db.NotificationDeliveries
        //     .Where(d => d.ToUserId == userId)
        //     .OrderByDescending(d => d.CreatedAtUtc)
        //     .Select(d => d.Id)
        //     .FirstOrDefaultAsync(ct);

        // خيار 2: لو ماعندكش DbContext هنا — استخدم الريبو الموجود عندك
        // TODO: بدّل اسم _repo واسم الدالة حسب مشروعك
        var latest = (await _repo.GetUserInboxAsync(userId, unreadOnly: false, take: 1, ct))
                     .FirstOrDefault();
        // لاحظ: الكود عندك كان بيرجع item فيه d.Id
        var deliveryId = latest.d.Id;
        return deliveryId;
    }
    public async Task ToTopicAsync(string createdByUserId, string topic, string title, string message, CancellationToken ct = default)
    {
        var n = new Notification2 { Title = title, Message = message, Level = "warning", Scope = "topic", CreatedByUserId = createdByUserId };
        await _repo.AddAsync(n, ct);
        await _uow.SaveChangesAsync(ct);
        await _bus.ToTopicAsync(topic, new NotificationPayload(n.Title, n.Message, null, null, n.Level, DateTime.UtcNow.Ticks));
    }

    public async Task ToAllAsync(string createdByUserId, string title, string message, CancellationToken ct = default)
    {
        var n = new Notification2 { Title = title, Message = message, Level = "info", Scope = "broadcast", CreatedByUserId = createdByUserId };
        await _repo.AddAsync(n, ct);
        await _uow.SaveChangesAsync(ct);
        await _bus.BroadcastAsync(new NotificationPayload(n.Title, n.Message, null, null, n.Level, DateTime.UtcNow.Ticks));
    }

    public async Task<List<object>> GetMyInboxAsync(string userId, bool unreadOnly, int take, CancellationToken ct = default)
    {
        var list = await _repo.GetUserInboxAsync(userId, unreadOnly, take, ct);
        return list.Select(x => new { notificationId = x.n.Id, deliveryId = x.d.Id, x.n.Title, x.n.Message, x.n.Level, x.n.Url, x.n.CreatedAtUtc, x.d.IsRead, x.d.ReadAtUtc } as object).ToList();
    }

    public Task<bool> MarkReadAsync(Guid deliveryId, string userId, CancellationToken ct = default)
        => _repo.MarkDeliveryReadAsync(deliveryId, userId, ct);

    public async Task<int> MarkAllReadAsync(string userId, CancellationToken ct = default)
    {
        var inbox = await _repo.GetUserInboxAsync(userId, unreadOnly: true, take: int.MaxValue, ct);
        var ids = inbox.Select(x => x.d.Id).ToList();
        var count = 0;

        foreach (var id in ids)
        {
            if (await _repo.MarkDeliveryReadAsync(id, userId, ct))
                count++;
        }

        await _uow.SaveChangesAsync(ct);
        return count;
    }
    public async Task<NotifyQueryResult> QueryAsync(string userId, int skip, int take, string? level, string? kind, bool? read, CancellationToken ct)
    {
        var result = await _repo.QueryUserNotificationsAsync(userId, skip, take, level, kind, read, ct);
        // نفترض الريبو يرجّع (items, hasMore) أو (items, total)
        return new NotifyQueryResult
        {
            Items = result.Items.Select(r => new NotificationDeliveryDto(
                r.DeliveryId, r.Title, r.Message, r.Level, r.Url, r.CreatedAtUtc, r.IsRead, r.Kind, r.Meta
            )).ToList(),
            HasMore = result.HasMore
        };
    }
    public Task<Guid> SendCalendarInviteAsync(string toUserId, Guid eventId, string title, DateTime startUtc, CancellationToken ct = default)
    => ToUserAsync(
        createdByUserId: toUserId, // أو OwnerId لو متاح
        toUserId: toUserId,
        title: "Calendar Invite",
        message: $"دعوة إلى: {title} • {startUtc:yyyy-MM-dd HH:mm} UTC",
        url: $"/calendar/event/{eventId}",
        icon: null,
        level: "info",
        ct: ct
    );

    public Task<Guid> SendCalendarReminderAsync(string toUserId, Guid eventId, string title, int minutesBefore, CancellationToken ct = default)
        => ToUserAsync(
            createdByUserId: toUserId,
            toUserId: toUserId,
            title: "Reminder",
            message: $"بعد {minutesBefore} دقيقة: {title}",
            url: $"/calendar/event/{eventId}",
            icon: null,
            level: "warning",
            ct: ct
        );

    public Task<Guid> SendCalendarRespondAsync(string ownerUserId, Guid eventId, string title, string status, CancellationToken ct = default)
        => ToUserAsync(
            createdByUserId: ownerUserId,
            toUserId: ownerUserId,
            title: "Calendar Update",
            message: $"تم الرد ({status.ToUpper()}): {title}",
            url: $"/calendar/event/{eventId}",
            icon: null,
            level: status.Equals("accepted", StringComparison.OrdinalIgnoreCase) ? "success"
                 : status.Equals("declined", StringComparison.OrdinalIgnoreCase) ? "danger" : "info",
            ct: ct
        );

    public async Task<Guid> SendAsync(NotificationMessage msg, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(msg.ToUserId))
            throw new ArgumentNullException(nameof(msg.ToUserId));

        // 1) خزّن
        var saved = await _repo.AddAsync(new Notification2
        {
            CreatedByUserId = msg.ToUserId,
            Title = msg.Title ?? "",
            Message = msg.Message ?? "",
            Level = string.IsNullOrWhiteSpace(msg.Level) ? "info" : msg.Level!,
            Url = msg.Url,
            CreatedAtUtc = DateTime.UtcNow
        }, ct);

        // 2) ابثّ عبر SignalR (event اسمها متوافق مع JS عندك)
        await _hub.Clients.User(msg.ToUserId).SendAsync("ReceiveNotification", new
        {
            deliveryId = saved.Id,
            title = saved.Title,
            message = saved.Message,
            level = saved.Level,
            url = saved.Url,
            createdAtUtc = saved.CreatedAtUtc,
            isRead = saved.IsRead
        }, ct);

        return saved.Id;
    }

   



    public async Task<List<object>> GetInboxAsync(string userId, bool unreadOnly = false, int take = 50, CancellationToken ct = default)
    {
        var list = await _repo.GetInboxAsync(userId, unreadOnly, take, ct);
        // DTO خفيف للـAPI
        return list.Select(x => new {
            deliveryId = x.Id,
            title = x.Title,
            message = x.Message,
            level = x.Level,
            url = x.Url,
            createdAtUtc = x.CreatedAtUtc,
            isRead = x.IsRead
        }).Cast<object>().ToList();
    }
}
