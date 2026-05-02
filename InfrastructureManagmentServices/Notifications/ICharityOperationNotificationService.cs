namespace InfrastructureManagmentServices.Notification;

public interface ICharityOperationNotificationService
{
    Task NotifyAidRequestCreatedAsync(
        string beneficiaryName,
        string? aidTypeName,
        decimal? requestedAmount,
        string? actorUserId,
        string? url = null,
        CancellationToken ct = default);

    Task NotifyCommitteeDecisionCreatedAsync(
        string beneficiaryName,
        string decisionType,
        string? actorUserId,
        string? url = null,
        CancellationToken ct = default);

    Task NotifyGrantInstallmentDueSoonAsync(
        string agreementTitle,
        decimal amount,
        DateTime dueDate,
        string? actorUserId,
        string? url = null,
        CancellationToken ct = default);

    Task NotifyLowStockAsync(
        string itemName,
        string storeName,
        decimal currentBalance,
        decimal minimumBalance,
        string? actorUserId,
        string? url = null,
        CancellationToken ct = default);

    Task NotifyPayrollApprovedAsync(
        int year,
        int month,
        decimal? totalNetAmount,
        string? actorUserId,
        string? url = null,
        CancellationToken ct = default);

    Task NotifyCustomAsync(
        string title,
        string message,
        string kind,
        string level,
        string? actorUserId,
        IEnumerable<string> roleNames,
        string? url = null,
        CancellationToken ct = default);

    /// <summary>
    /// نسخة سريعة للعمليات الحساسة مثل زر موافقة سير العمل.
    /// تحفظ التنبيه في قاعدة البيانات ثم ترسل realtime push بمهلة قصيرة وبشكل متوازي.
    /// </summary>
    Task NotifyCustomFastAsync(
        string title,
        string message,
        string kind,
        string level,
        string? actorUserId,
        IEnumerable<string> roleNames,
        string? url = null,
        CancellationToken ct = default);


    // ── Kafala ──
    Task NotifyKafalaPaymentDueAsync(
        string sponsorName, string caseNumber,
        decimal amount, DateTime dueDate,
        string? url = null, CancellationToken ct = default);

    Task NotifyKafalaPaymentOverdueAsync(
        string sponsorName, string caseNumber,
        decimal amount, int overdueDays,
        string? url = null, CancellationToken ct = default);

    // ── Projects ──
    Task NotifyProjectPhaseDelayedAsync(
        string projectName, string phaseName,
        int overdueDays, string? actorUserId,
        string? url = null, CancellationToken ct = default);

    Task NotifyProjectMilestoneDueAsync(
        string projectName, string milestoneTitle,
        DateTime dueDate, string? actorUserId, string? url = null, CancellationToken ct = default);

    Task NotifyProjectBudgetExceededAsync(
        string projectName, decimal planned, decimal actual,
        string? actorUserId,
        string? url = null, CancellationToken ct = default);

    // ── Aid Request workflow ──
    Task NotifyAidRequestStatusChangedAsync(
        string beneficiaryName, string newStatus,
        string? actorUserId,
        string? url = null, CancellationToken ct = default);
}
