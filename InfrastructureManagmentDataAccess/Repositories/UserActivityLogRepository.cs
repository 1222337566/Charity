//using InfrastructureManagmentCore.Domains.Auditing;
using InfrastrfuctureManagmentCore.Domains.Identity;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentDataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

public class UserActivityLogRepository : BaseRepository<UserActivityLog>, IUserActivityLogRepository
{
    public UserActivityLogRepository(AppDbContext db) : base(db) { }
    public async Task AddLogAsync(UserActivityLog log, CancellationToken ct = default)
    {
       if (log.OccurredAtUtc == default)
       {
            log.OccurredAtUtc = DateTime.UtcNow;
       }

await _appDBContext.Set<UserActivityLog>().AddAsync(log, ct);
await _appDBContext.SaveChangesAsync(ct);
    }
    public async Task<IReadOnlyList<UserActivityLog>> SearchAsync(string? query, string? action, string? userId, string? ipAddress, DateTime? fromUtc, DateTime? toUtc, int take, CancellationToken ct = default)
    {
        take = Math.Clamp(take <= 0 ? 200 : take, 20, 5000);
        var logs = _appDBContext.Set<UserActivityLog>().AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            query = query.Trim();
            logs = logs.Where(x =>
                (x.UserId != null && x.UserId.Contains(query)) ||
                (x.Action != null && x.Action.Contains(query)) ||
                (x.IpAddress != null && x.IpAddress.Contains(query)) ||
                (x.UserAgent != null && x.UserAgent.Contains(query)) ||
                (x.Name != null && x.Name.Contains(query)));
        }

        if (!string.IsNullOrWhiteSpace(action))
        {
            action = action.Trim();
            logs = logs.Where(x => x.Action != null && x.Action.Trim() == action);
        }

        if (!string.IsNullOrWhiteSpace(userId))

                    {
            userId = userId.Trim();
            logs = logs.Where(x => x.UserId != null && x.UserId.Contains(userId));
                    }

        if (!string.IsNullOrWhiteSpace(ipAddress))
            logs = logs.Where(x => x.IpAddress != null && x.IpAddress.Contains(ipAddress));

        if (fromUtc.HasValue)
            logs = logs.Where(x => x.OccurredAtUtc >= fromUtc.Value);

        if (toUtc.HasValue)
        {
            var endExclusive = toUtc.Value.Date.AddDays(1);
            logs = logs.Where(x => x.OccurredAtUtc < endExclusive);
        }

        return await logs
            .OrderByDescending(x => x.OccurredAtUtc)
            .ThenByDescending(x => x.Id)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task<UserActivityLog?> GetByIdAsync(long id, CancellationToken ct = default)
        => await _appDBContext.Set<UserActivityLog>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<string>> GetActionsAsync(CancellationToken ct = default)
        => await _appDBContext.Set<UserActivityLog>()
            .AsNoTracking()
            .Where(x => x.Action != null && x.Action != "")
            .Select(x => x.Action!)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(ct);

    public async Task<UserActivityLogSummary> GetSummaryAsync(DateTime? fromUtc, DateTime? toUtc, CancellationToken ct = default)
    {
        var logs = _appDBContext.Set<UserActivityLog>().AsNoTracking().AsQueryable();

        if (fromUtc.HasValue)
            logs = logs.Where(x => x.OccurredAtUtc >= fromUtc.Value);

        if (toUtc.HasValue)
        {
            var endExclusive = toUtc.Value.Date.AddDays(1);
            logs = logs.Where(x => x.OccurredAtUtc < endExclusive);
        }

        var today = DateTime.UtcNow.Date;
        var last7 = today.AddDays(-6);

        return new UserActivityLogSummary
        {
            TotalCount = await logs.CountAsync(ct),
            TodayCount = await logs.CountAsync(x => x.OccurredAtUtc >= today, ct),
            Last7DaysCount = await logs.CountAsync(x => x.OccurredAtUtc >= last7, ct),
            DistinctUsers = await logs.Select(x => x.UserId).Where(x => x != null && x != "").Distinct().CountAsync(ct)
        };
    }
}
