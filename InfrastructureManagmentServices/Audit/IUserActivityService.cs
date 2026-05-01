using System.Collections.Generic;

public interface IUserActivityService
{
    Task LogAsync(string userId, string action, CancellationToken ct = default);
    Task LogAsync(string? userId, string action, string? description, string? entityName = null, string? entityId = null, Dictionary<string, string?>? oldValues = null, Dictionary<string, string?>? newValues = null, CancellationToken ct = default);
    Task LogBusinessAsync(string? userId, string action, string description, string? entityName = null, string? entityId = null, Dictionary<string, string?>? oldValues = null, Dictionary<string, string?>? newValues = null, CancellationToken ct = default);
}
