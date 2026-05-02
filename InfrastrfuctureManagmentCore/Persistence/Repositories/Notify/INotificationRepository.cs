using InfrastrfuctureManagmentCore.Domains.Notify;
using InfrastructureManagmentCore.Domains.Notify;
using InfrastructureManagmentCore.Domains.Reactions;

namespace InfrastructureManagmentCore.Persistence.Repositories.Abstractions;

public interface INotificationRepository : IRepository<Notification2>
{
    // Task<Notification2> SaveWithDeliveriesAsync(Notification2 n, IEnumerable<string> userIds, CancellationToken ct = default);
    Task<IEnumerable<(NotificationDelivery d, Notification2 n)>> GetUserInboxAsync(string userId, bool unreadOnly, int take, CancellationToken ct);
    Task<bool> MarkDeliveryReadAsync(Guid deliveryId, string userId, CancellationToken ct = default);
    //Task<(IEnumerable<NotificationDeliveryRow> Items, bool HasMore)> QueryUserNotificationsAsync(
    //    string userId, int skip, int take, string? level, string? kind, bool? read, CancellationToken ct);
    Task SaveWithDeliveriesAsync(Notification2 notification, IEnumerable<string> toUserIds, CancellationToken ct);
    //Task<IEnumerable<(NotificationDelivery d, Notification n)>> GetUserInboxAsync(string userId, bool unreadOnly, int take, CancellationToken ct);
   // Task<bool> MarkDeliveryReadAsync(Guid deliveryId, string userId, CancellationToken ct);

    Task<(IEnumerable<NotificationDeliveryRow> Items, bool HasMore)> QueryUserNotificationsAsync(
        string userId, int skip, int take, string? level, string? kind, bool? read, CancellationToken ct);
    Task<Notification2> AddAsync(Notification2 n, CancellationToken ct = default);
    Task MarkReadAsync(Guid id, string userId, CancellationToken ct = default);
    Task<int> MarkAllReadAsync(string userId, CancellationToken ct = default);
    Task<List<Notification2>> GetInboxAsync(string userId, bool unreadOnly = false, int take = 50, CancellationToken ct = default);
}