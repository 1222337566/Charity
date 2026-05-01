using Microsoft.EntityFrameworkCore;
using InfrastructureManagmentCore.Domains.Notify;
using InfrastructureManagmentCore.Persistence.Repositories.Abstractions;
using InfrastructureManagmentCore.Domains.Reactions;
using InfrastructureManagmentDataAccess.EntityFramework;
using System.Text.Json;
using InfrastrfuctureManagmentCore.Domains.Notify;

namespace InfrastructureManagmentCore.Persistence.Repositories.Ef;

public class NotificationRepository : EfRepository<Notification2>, INotificationRepository
{
    private readonly AppDbContext _db;
    private readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

    public NotificationRepository(AppDbContext db) : base(db) { _db = db; }
    public async Task SaveWithDeliveriesAsync(Notification2 notification, IEnumerable<string> toUserIds, CancellationToken ct)
    {
        if (notification is null) throw new ArgumentNullException(nameof(notification));
        var userIds = toUserIds?.Distinct().ToList() ?? new List<string>();
        if (userIds.Count == 0) return;

        // جهّز الـ Notification لو جديد
        if (notification.Id == Guid.Empty)
        {
            notification.Id = Guid.NewGuid();
            if (notification.CreatedAtUtc == default) notification.CreatedAtUtc = DateTime.UtcNow;
        }

        // Add notification if not tracked
        if (_db.Entry(notification).State == EntityState.Detached)
            _db.Notifications.Add(notification);

        // deliveries
        var deliveries = new List<NotificationDelivery>(userIds.Count);
        foreach (var uid in userIds)
        {
            deliveries.Add(new NotificationDelivery
            {
                Id = Guid.NewGuid(),
                NotificationId = notification.Id,
                UserId = uid,
                IsRead = false,
                DeliveredAtUtc = notification.CreatedAtUtc
            });
        }

        await _db.NotificationDeliveries.AddRangeAsync(deliveries, ct);
        // الحفظ يتم في الـ UnitOfWork خارج الريبو (زي ما اتفقنا)
    }

    // 2) Inbox مختصر (للخدمة عشان تجيب أحدث deliveryId بسهولة)
    public async Task<IEnumerable<(NotificationDelivery d, Notification2 n)>> GetUserInboxAsync(
       string userId, bool unreadOnly, int take, CancellationToken ct)
    {
        var q =
            from d in _db.NotificationDeliveries.AsNoTracking()
            join n in _db.Notifications.AsNoTracking() on d.NotificationId equals n.Id
            where d.UserId == userId && (!unreadOnly || !d.IsRead)
            orderby d.CreatedAtUtc descending
            select new { d, n };

        var rows = await q.Take(take).ToListAsync(ct);
        return rows.Select(r => (r.d, r.n));
    }

    // 3) تعليم دليفري واحد كمقروء (مع التحقق من المالك)
    public async Task<bool> MarkDeliveryReadAsync(Guid deliveryId, string userId, CancellationToken ct)
    {
        var d = await _db.NotificationDeliveries
            .Where(x => x.Id == deliveryId && x.UserId == userId)
            .FirstOrDefaultAsync(ct);

        if (d is null || d.IsRead) return false;

        d.IsRead = true;
        // الحفظ يتم عبر UnitOfWork خارج الريبو
        return true;
    }

    // 4) استعلام بفلاتر + ترقيم صفحات للـ Inbox Page
    public async Task<(IEnumerable<NotificationDeliveryRow> Items, bool HasMore)> QueryUserNotificationsAsync(
        string userId, int skip, int take, string? level, string? kind, bool? read, CancellationToken ct)
    {
        // الإستعلام الأساسي
        var q =
            from d in _db.NotificationDeliveries.AsNoTracking()
            join n in _db.Notifications.AsNoTracking() on d.NotificationId equals n.Id
            where d.UserId == userId
            select new { d, n };

        if (!string.IsNullOrWhiteSpace(level))
            q = q.Where(x => x.n.Level == level);

        if (!string.IsNullOrWhiteSpace(kind))
            q = q.Where(x => x.n.Kind == kind);

        if (read.HasValue)
            q = q.Where(x => x.d.IsRead == read.Value);

        var ordered = q.OrderByDescending(x => x.d.DeliveredAtUtc);

        // البيانات المطلوبة
        var page = await ordered.Skip(skip).Take(take).ToListAsync(ct);
        var items = page.Select(x => new NotificationDeliveryRow
        {
            DeliveryId = x.d.Id,
            Title = x.n.Title ?? "Notification",
            Message = x.n.Message ?? "",
            Level = x.n.Level ?? "info",
            Url = x.n.Url,
            CreatedAtUtc = x.d.DeliveredAtUtc,
            IsRead = x.d.IsRead,
            Kind = x.n.Kind,
            Meta = SafeDeserialize(x.n.Scope)
        }).ToList();

        // هل يوجد المزيد؟
        var hasMore = await ordered.Skip(skip + take).AnyAsync(ct);

        return (items, hasMore);
    }

    private object? SafeDeserialize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try { return JsonSerializer.Deserialize<object>(json, _json); } catch { return null; }
    }
    public async Task<Notification2> AddAsync(Notification2 n, CancellationToken ct = default)
    {
        _db.Notifications.Add(n);
        await _db.SaveChangesAsync(ct);
        return n;
    }

    public async Task MarkReadAsync(Guid id, string userId, CancellationToken ct = default)
    {
        var n = await _db.Notifications.FirstOrDefaultAsync(x => x.Id == id && x.CreatedByUserId == userId, ct);
        if (n == null) return;
        if (!n.IsRead)
        {
            n.IsRead = true;
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task<int> MarkAllReadAsync(string userId, CancellationToken ct = default)
    {
        var q = _db.Notifications.Where(x => x.CreatedByUserId == userId && !x.IsRead);
        var list = await q.ToListAsync(ct);
        if (list.Count == 0) return 0;
        foreach (var n in list) n.IsRead = true;
        return await _db.SaveChangesAsync(ct);
    }

    public async Task<List<Notification2>> GetInboxAsync(string userId, bool unreadOnly = false, int take = 50, CancellationToken ct = default)
    {
        var q = _db.Notifications.Where(x => x.CreatedByUserId == userId);
        if (unreadOnly) q = q.Where(x => !x.IsRead);
        return await q.OrderByDescending(x => x.CreatedAtUtc)
                      .Take(Math.Clamp(take, 1, 200))
                      .ToListAsync(ct);
    }
    //public async Task<Notification2> SaveWithDeliveriesAsync(Notification2 n, IEnumerable<string> userIds, CancellationToken ct = default)
    //{
    //    foreach (var uid in userIds)
    //        n.Deliveries.Add(new NotificationDelivery { UserId = uid });
    //    await _set.AddAsync(n, ct);
    //    return n;
    //}

    //public async Task<List<(Notification2 n, NotificationDelivery d)>> GetUserInboxAsync(string userId, bool unreadOnly, int take, CancellationToken ct = default)
    //{
    //    var q = _db.NotificationDeliveries
    //        .AsNoTracking()
    //        .Where(d => d.UserId == userId)
    //        .Include(d => d.Notification)
    //        .OrderByDescending(d => d.Notification.CreatedAtUtc);

    //    if (unreadOnly) 
    //        q = (IOrderedQueryable<NotificationDelivery>)q.Where(d => !d.IsRead);
    //    var list = await q.Take(Math.Clamp(take, 10, 200)).ToListAsync(ct);
    //    return list.Select(d => (d.Notification, d)).ToList();
    //}

    //public async Task<bool> MarkDeliveryReadAsync(Guid deliveryId, string userId, CancellationToken ct = default)
    //{
    //    var d = await _db.NotificationDeliveries.SingleOrDefaultAsync(x => x.Id == deliveryId && x.UserId == userId, ct);
    //    if (d is null) return false;
    //    if (!d.IsRead)
    //    {
    //        d.IsRead = true;
    //        d.ReadAtUtc = DateTime.UtcNow;
    //        await _db.SaveChangesAsync(ct);
    //    }
    //    return true;
    //}
}