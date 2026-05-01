using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Notification;

public sealed class AidCycleAlertItem
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty;
    public string Level { get; set; } = "warning";
    public string? Url { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

public sealed class AidCycleReminderScanResult
{
    public DateTime ScanDateUtc { get; set; } = DateTime.UtcNow;
    public int CreatedNotificationsCount { get; set; }
    public int MissingMonthlyCyclesCount { get; set; }
    public int PendingCyclesCount { get; set; }
    public int OverdueBeneficiariesCount { get; set; }
    public int NotIncludedBeneficiariesCount { get; set; }
    public List<AidCycleAlertItem> Alerts { get; set; } = new();
}

public interface IAidCycleReminderService
{
    Task<AidCycleReminderScanResult> RunAsync(DateTime? today = null, string? actorUserId = null, CancellationToken ct = default);
    Task<IReadOnlyList<AidCycleAlertItem>> BuildAlertsAsync(DateTime? today = null, CancellationToken ct = default);
}
