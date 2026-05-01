namespace Skote.ViewModels.Audit;

public class UserActivityLogDetailsVm
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string ControllerName { get; set; } = string.Empty;
    public string ActionName { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public Dictionary<string, string?> OldValues { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string?> NewValues { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public DateTime OccurredAtUtc { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
}
