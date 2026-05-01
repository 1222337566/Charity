using InfrastructureManagmentCore.Domains.Identity;
using InfrastructureManagmentCore.Domains.Notify;
using InfrastructureManagmentCore.Persistence.Repositories.Abstractions;
using InfrastructureManagmentDataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Skote.Helpers;

namespace InfrastructureManagmentServices.Notification;

public class CharityOperationNotificationService : ICharityOperationNotificationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitofWork _unitofWork;
    private readonly IRealtimeClient _realtimeClient;
    private readonly ILogger<CharityOperationNotificationService> _logger;

    public CharityOperationNotificationService(
        UserManager<ApplicationUser> userManager,
        INotificationRepository notificationRepository,
        IUnitofWork unitofWork,
        IRealtimeClient realtimeClient,
        ILogger<CharityOperationNotificationService> logger)
    {
        _userManager = userManager;
        _notificationRepository = notificationRepository;
        _unitofWork = unitofWork;
        _realtimeClient = realtimeClient;
        _logger = logger;
    }

    public Task NotifyAidRequestCreatedAsync(
        string beneficiaryName,
        string? aidTypeName,
        decimal? requestedAmount,
        string? actorUserId,
        string? url = null,
        CancellationToken ct = default)
    {
        var title = "طلب مساعدة جديد";
        var amountText = requestedAmount.HasValue ? $" بقيمة {requestedAmount.Value:n2}" : string.Empty;
        var aidText = string.IsNullOrWhiteSpace(aidTypeName) ? string.Empty : $" ({aidTypeName})";
        var message = $"تم تسجيل طلب مساعدة للمستفيد {beneficiaryName}{aidText}{amountText}.";

        return NotifyCustomAsync(
            title,
            message,
            kind: "charity.aid-request.created",
            level: "info",
            actorUserId: actorUserId,
            roleNames: new[]
            {
                CharityNotificationAudience.CharityManager,
                CharityNotificationAudience.BeneficiariesOfficer,
                CharityNotificationAudience.SocialResearcher,
                CharityNotificationAudience.FinancialOfficer
            },
            url,
            ct);
    }

    public Task NotifyCommitteeDecisionCreatedAsync(
        string beneficiaryName,
        string decisionType,
        string? actorUserId,
        string? url = null,
        CancellationToken ct = default)
    {
        var title = "قرار لجنة جديد";
        var message = $"تم تسجيل قرار لجنة للمستفيد {beneficiaryName} بنوع قرار: {decisionType}.";

        return NotifyCustomAsync(
            title,
            message,
            kind: "charity.committee-decision.created",
            level: "success",
            actorUserId: actorUserId,
            roleNames: new[]
            {
                CharityNotificationAudience.CharityManager,
                CharityNotificationAudience.BeneficiariesOfficer,
                CharityNotificationAudience.SocialResearcher
            },
            url,
            ct);
    }

    public Task NotifyGrantInstallmentDueSoonAsync(
        string agreementTitle,
        decimal amount,
        DateTime dueDate,
        string? actorUserId,
        string? url = null,
        CancellationToken ct = default)
    {
        var title = "دفعة تمويل قريبة الاستحقاق";
        var message = $"دفعة تمويل لاتفاقية {agreementTitle} تستحق في {dueDate:yyyy-MM-dd} بقيمة {amount:n2}.";

        return NotifyCustomAsync(
            title,
            message,
            kind: "charity.grant-installment.due-soon",
            level: "warning",
            actorUserId: actorUserId,
            roleNames: new[]
            {
                CharityNotificationAudience.CharityManager,
                CharityNotificationAudience.FunderRelations,
                CharityNotificationAudience.ProjectManager,
                CharityNotificationAudience.FinancialOfficer
            },
            url,
            ct);
    }

    public Task NotifyLowStockAsync(
        string itemName,
        string storeName,
        decimal currentBalance,
        decimal minimumBalance,
        string? actorUserId,
        string? url = null,
        CancellationToken ct = default)
    {
        var title = "تنبيه رصيد منخفض بالمخزن";
        var message = $"الصنف {itemName} في مخزن {storeName} وصل إلى {currentBalance:n2} بينما الحد الأدنى {minimumBalance:n2}.";

        return NotifyCustomAsync(
            title,
            message,
            kind: "charity.stock.low-balance",
            level: "warning",
            actorUserId: actorUserId,
            roleNames: new[]
            {
                CharityNotificationAudience.CharityManager,
                CharityNotificationAudience.StoreKeeper,
                CharityNotificationAudience.ProjectManager
            },
            url,
            ct);
    }

    public Task NotifyPayrollApprovedAsync(
        int year,
        int month,
        decimal? totalNetAmount,
        string? actorUserId,
        string? url = null,
        CancellationToken ct = default)
    {
        var title = "اعتماد شهر مرتبات";
        var amountText = totalNetAmount.HasValue ? $" بإجمالي صافي {totalNetAmount.Value:n2}" : string.Empty;
        var message = $"تم اعتماد مرتبات شهر {month:D2}/{year}{amountText}.";

        return NotifyCustomAsync(
            title,
            message,
            kind: "charity.payroll.approved",
            level: "success",
            actorUserId: actorUserId,
            roleNames: new[]
            {
                CharityNotificationAudience.CharityManager,
                CharityNotificationAudience.PayrollOfficer,
                CharityNotificationAudience.FinancialOfficer,
                CharityNotificationAudience.HrOfficer
            },
            url,
            ct);
    }


    public Task NotifyCustomAsync(
        string title,
        string message,
        string kind,
        string level,
        string? actorUserId,
        IEnumerable<string> roleNames,
        string? url = null,
        CancellationToken ct = default)
        => NotifyCustomInternalAsync(
            title,
            message,
            kind,
            level,
            actorUserId,
            roleNames,
            url,
            realtimeTimeout: TimeSpan.FromSeconds(10),
            ct);

    public Task NotifyCustomFastAsync(
        string title,
        string message,
        string kind,
        string level,
        string? actorUserId,
        IEnumerable<string> roleNames,
        string? url = null,
        CancellationToken ct = default)
        => NotifyCustomInternalAsync(
            title,
            message,
            kind,
            level,
            actorUserId,
            roleNames,
            url,
            realtimeTimeout: TimeSpan.FromMilliseconds(700),
            ct);

    private async Task NotifyCustomInternalAsync(
        string title,
        string message,
        string kind,
        string level,
        string? actorUserId,
        IEnumerable<string> roleNames,
        string? url,
        TimeSpan realtimeTimeout,
        CancellationToken ct)
    {
        var userIds = await ResolveUserIdsAsync(roleNames, actorUserId, ct);
        if (userIds.Count == 0)
        {
            _logger.LogInformation("No recipients found for notification {Kind}", kind);
            return;
        }

        var notification = new Notification2
        {
            Id = Guid.NewGuid(),
            Title = title,
            Message = message,
            Url = url,
            Kind = kind,
            Level = string.IsNullOrWhiteSpace(level) ? "info" : level,
            Scope = "user",
            CreatedByUserId = actorUserId,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _notificationRepository.SaveWithDeliveriesAsync(notification, userIds, ct);
        await _unitofWork.SaveChangesAsync(ct);

        await PushRealtimeAsync(notification, userIds, title, message, url, realtimeTimeout, ct);
    }

    private async Task PushRealtimeAsync(
        Notification2 notification,
        IReadOnlyCollection<string> userIds,
        string title,
        string message,
        string? url,
        TimeSpan timeout,
        CancellationToken ct)
    {
        if (userIds.Count == 0)
            return;

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeoutCts.CancelAfter(timeout);

        var tasks = userIds.Select(async userId =>
        {
            try
            {
                await _realtimeClient.ToUserAsync(
                    userId,
                    type: "notify:new",
                    message: title,
                    payload: new
                    {
                        notificationId = notification.Id,
                        title,
                        body = message,
                        url,
                        level = notification.Level,
                        kind = notification.Kind,
                        createdAtUtc = notification.CreatedAtUtc
                    },
                    timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                _logger.LogDebug("Realtime push timed out for user {UserId} and notification {NotificationId}", userId, notification.Id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Realtime push failed for user {UserId} and notification {NotificationId}", userId, notification.Id);
            }
        }).ToArray();

        try
        {
            await Task.WhenAll(tasks);
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug("Realtime push batch timed out for notification {NotificationId}", notification.Id);
        }
    }
    private async Task<List<string>> ResolveUserIdsAsync(IEnumerable<string> roleNames, string? actorUserId, CancellationToken ct)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var roleName in roleNames.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var users = await _userManager.GetUsersInRoleAsync(roleName);
            foreach (var user in users)
            {
                if (!string.IsNullOrWhiteSpace(actorUserId) && string.Equals(user.Id, actorUserId, StringComparison.OrdinalIgnoreCase))
                    continue;

                result.Add(user.Id);
            }
        }

        return result.ToList();
    }

    public Task NotifyKafalaPaymentDueAsync(string sponsorName, string caseNumber, decimal amount, DateTime dueDate, string? url = null, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task NotifyKafalaPaymentOverdueAsync(string sponsorName, string caseNumber, decimal amount, int overdueDays, string? url = null, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task NotifyProjectPhaseDelayedAsync(string projectName, string phaseName, int overdueDays, string? actorUserId, string? url = null, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task NotifyProjectMilestoneDueAsync(string projectName, string milestoneTitle, DateTime dueDate, string? actorUserId, string? url = null, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task NotifyProjectBudgetExceededAsync(string projectName, decimal planned, decimal actual, string? actorUserId, string? url = null, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task NotifyAidRequestStatusChangedAsync(string beneficiaryName, string newStatus, string? actorUserId, string? url = null, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
