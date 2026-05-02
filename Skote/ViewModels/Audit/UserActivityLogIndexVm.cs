namespace Skote.ViewModels.Audit;

public class UserActivityLogIndexVm
{
    public string? Query { get; set; }
    public string? Action { get; set; }
    public string? UserId { get; set; }
    public string? IpAddress { get; set; }
    public DateTime? FromUtc { get; set; }
    public DateTime? ToUtc { get; set; }
    public int Take { get; set; } = 200;

    public List<string> AvailableActions { get; set; } = new();
    public List<UserActivityLogRowVm> Items { get; set; } = new();
    public AuditSummaryVm Summary { get; set; } = new();
}

public class UserActivityLogRowVm
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string ControllerName { get; set; } = string.Empty;
    public string ActionName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public DateTime OccurredAtUtc { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
}

public class AuditSummaryVm
{
    public int TotalCount { get; set; }
    public int TodayCount { get; set; }
    public int Last7DaysCount { get; set; }
    public int DistinctUsers { get; set; }
}
