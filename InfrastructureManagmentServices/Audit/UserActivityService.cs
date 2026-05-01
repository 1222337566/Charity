using InfrastrfuctureManagmentCore.Domains.Identity;
using InfrastructureManagmentDataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Text.Json;

public class UserActivityService : IUserActivityService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHttpContextAccessor _http;

    public UserActivityService(IServiceScopeFactory scopeFactory, IHttpContextAccessor http)
    {
        _scopeFactory = scopeFactory;
        _http = http;
    }

    public Task LogAsync(string userId, string action, CancellationToken ct = default)
        => LogAsync(userId, action, null, null, null, null, null, ct);

    public Task LogBusinessAsync(
        string? userId,
        string action,
        string description,
        string? entityName = null,
        string? entityId = null,
        Dictionary<string, string?>? oldValues = null,
        Dictionary<string, string?>? newValues = null,
        CancellationToken ct = default)
        => LogAsync(userId, action, description, entityName, entityId, oldValues, newValues, ct);

    public async Task LogAsync(
        string? userId,
        string action,
        string? description,
        string? entityName = null,
        string? entityId = null,
        Dictionary<string, string?>? oldValues = null,
        Dictionary<string, string?>? newValues = null,
        CancellationToken ct = default)
    {
        using var scope = _scopeFactory.CreateScope();

        var repo = scope.ServiceProvider.GetRequiredService<IUserActivityLogRepository>();

        var ctx = _http.HttpContext;

        var payload = new AuditPayload
        {
            UserName = ctx?.User?.Identity?.Name,
            Description = description,
            ControllerName = ctx?.GetRouteValue("controller")?.ToString(),
            ActionName = ctx?.GetRouteValue("action")?.ToString(),
            HttpMethod = ctx?.Request?.Method,
            Path = ctx?.Request?.Path.Value,
            EntityName = entityName,
            EntityId = entityId,
            OldValues = oldValues ?? new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase),
            NewValues = newValues ?? new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
        };

        var log = new UserActivityLog
        {
            UserId = string.IsNullOrWhiteSpace(userId)
                ? (ctx?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? ctx?.User?.FindFirstValue("sub") ?? "anonymous")
                : userId,
            Action = action,
            Name = JsonSerializer.Serialize(payload),
            OccurredAtUtc = DateTime.UtcNow,
            IpAddress = ctx?.Connection?.RemoteIpAddress?.ToString() ?? string.Empty,
            UserAgent = ctx?.Request?.Headers["User-Agent"].ToString() ?? string.Empty
        };

        await repo.AddLogAsync(log, ct);
    }

    private sealed class AuditPayload
    {
        public string? UserName { get; set; }
        public string? Description { get; set; }
        public string? ControllerName { get; set; }
        public string? ActionName { get; set; }
        public string? HttpMethod { get; set; }
        public string? Path { get; set; }
        public string? EntityName { get; set; }
        public string? EntityId { get; set; }
        public Dictionary<string, string?> OldValues { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string?> NewValues { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    }
}