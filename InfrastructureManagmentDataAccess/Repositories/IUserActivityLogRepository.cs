using InfrastrfuctureManagmentCore.Domains.Identity;
//using InfrastructureManagmentCore.Domains.Identity;
using InfrastructureManagmentDataAccess.Repositories;
using System.Threading;
using System.Threading.Tasks;

public interface IUserActivityLogRepository : IBaseRepository<UserActivityLog>
{
    Task AddLogAsync(UserActivityLog log, CancellationToken ct = default);
    Task<IReadOnlyList<UserActivityLog>> SearchAsync(string? query, string? action, string? userId, string? ipAddress, DateTime? fromUtc, DateTime? toUtc, int take, CancellationToken ct = default);
    Task<UserActivityLog?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<IReadOnlyList<string>> GetActionsAsync(CancellationToken ct = default);
    Task<UserActivityLogSummary> GetSummaryAsync(DateTime? fromUtc, DateTime? toUtc, CancellationToken ct = default);
}

public sealed class UserActivityLogSummary
{
    public int TotalCount { get; set; }
    public int TodayCount { get; set; }
    public int Last7DaysCount { get; set; }
    public int DistinctUsers { get; set; }
}
