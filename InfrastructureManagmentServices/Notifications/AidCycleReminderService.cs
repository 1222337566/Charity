using InfrastrfuctureManagmentCore.Domains.Charity.AidCycles;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentServices.Notification;

public class AidCycleReminderService : IAidCycleReminderService
{
    private readonly AppDbContext _db;
    private readonly ICharityOperationNotificationService _notificationService;

    public AidCycleReminderService(
        AppDbContext db,
        ICharityOperationNotificationService notificationService)
    {
        _db = db;
        _notificationService = notificationService;
    }

    public async Task<AidCycleReminderScanResult> RunAsync(DateTime? today = null, string? actorUserId = null, CancellationToken ct = default)
    {
        var result = new AidCycleReminderScanResult();
        var alerts = await BuildAlertsAsync(today, ct);
        result.Alerts = alerts.ToList();

        foreach (var alert in alerts)
        {
            await _notificationService.NotifyCustomAsync(
                title: alert.Title,
                message: alert.Message,
                kind: alert.Kind,
                level: alert.Level,
                actorUserId: actorUserId,
                roleNames: new[]
                {
                    CharityNotificationAudience.CharityManager,
                    CharityNotificationAudience.BeneficiariesOfficer,
                    CharityNotificationAudience.FinancialOfficer
                },
                url: alert.Url,
                ct: ct);

            result.CreatedNotificationsCount++;
        }

        result.MissingMonthlyCyclesCount = alerts.Count(x => x.Kind == "charity.aid-cycle.monthly.missing");
        result.PendingCyclesCount = alerts.Count(x => x.Kind == "charity.aid-cycle.monthly.pending");
        result.OverdueBeneficiariesCount = alerts.Count(x => x.Kind == "charity.aid-cycle.monthly.overdue" || x.Kind == "charity.aid-cycle.monthly.undisbursed");
        result.NotIncludedBeneficiariesCount = alerts.Count(x => x.Kind == "charity.aid-cycle.monthly.not-included");
        return result;
    }

    public async Task<IReadOnlyList<AidCycleAlertItem>> BuildAlertsAsync(DateTime? today = null, CancellationToken ct = default)
    {
        var now = (today ?? DateTime.Today).Date;
        var alerts = new List<AidCycleAlertItem>();

        var monthlyCycles = await _db.Set<AidCycle>()
            .AsNoTracking()
            .Where(x => x.CycleType == "Monthly"
                        && x.PeriodYear == now.Year
                        && x.PeriodMonth == now.Month
                        && x.Status != "Cancelled")
            .ToListAsync(ct);

        if (!monthlyCycles.Any())
        {
            alerts.Add(new AidCycleAlertItem
            {
                Title = "لم يتم إنشاء دورة الصرف الشهرية",
                Message = $"لم يتم إنشاء دورة صرف شهرية للشهر {now:MM/yyyy} حتى الآن.",
                Kind = "charity.aid-cycle.monthly.missing",
                Level = "warning",
                Url = "/AidCycles"
            });
        }

        foreach (var cycle in monthlyCycles)
        {
            if (cycle.Status != "Disbursed" && cycle.Status != "Closed")
            {
                alerts.Add(new AidCycleAlertItem
                {
                    Title = "دورة صرف شهرية لم تُنفذ بالكامل",
                    Message = $"الدورة {cycle.Title} بتاريخ صرف مخطط {cycle.PlannedDisbursementDate:yyyy-MM-dd} ما زالت حالتها {cycle.Status}.",
                    Kind = "charity.aid-cycle.monthly.pending",
                    Level = now > cycle.PlannedDisbursementDate.Date ? "error" : "warning",
                    Url = $"/AidCycles/Details/{cycle.Id}"
                });
            }

            var undisbursedCount = await _db.Set<AidCycleBeneficiary>()
                .AsNoTracking()
                .CountAsync(x => x.AidCycleId == cycle.Id && x.Status != "Disbursed", ct);

            if (undisbursedCount > 0)
            {
                alerts.Add(new AidCycleAlertItem
                {
                    Title = "مستحقون لم يتم صرفهم بعد",
                    Message = $"يوجد {undisbursedCount} مستفيد/ة داخل الدورة {cycle.Title} لم يتم صرفهم بعد.",
                    Kind = "charity.aid-cycle.monthly.undisbursed",
                    Level = now > cycle.PlannedDisbursementDate.Date ? "error" : "warning",
                    Url = $"/AidCycleBeneficiaries?aidCycleId={cycle.Id}"
                });
            }

            var overdueCount = await _db.Set<AidCycleBeneficiary>()
                .AsNoTracking()
                .CountAsync(x => x.AidCycleId == cycle.Id
                                 && x.Status != "Disbursed"
                                 && x.NextDueDate != null
                                 && x.NextDueDate.Value.Date < now, ct);

            if (overdueCount > 0)
            {
                alerts.Add(new AidCycleAlertItem
                {
                    Title = "تأخر صرف مستحقين",
                    Message = $"يوجد {overdueCount} مستفيد/ة داخل الدورة {cycle.Title} تجاوزوا تاريخ الاستحقاق ولم يتم صرفهم.",
                    Kind = "charity.aid-cycle.monthly.overdue",
                    Level = "error",
                    Url = $"/AidCycleBeneficiaries?aidCycleId={cycle.Id}"
                });
            }
        }

        var currentCycleIds = monthlyCycles.Select(x => x.Id).ToList();
        var currentCycleBeneficiaryIds = await _db.Set<AidCycleBeneficiary>()
            .AsNoTracking()
            .Where(x => currentCycleIds.Contains(x.AidCycleId))
            .Select(x => x.BeneficiaryId)
            .Distinct()
            .ToListAsync(ct);

        var approvedDecisionBeneficiaryIds = await _db.Set<BeneficiaryCommitteeDecision>()
            .AsNoTracking()
            .Where(x => x.Beneficiary != null
                        && x.Beneficiary.IsActive
                        && x.ApprovedStatus == true
                        && x.ApprovedAmount != null
                        && x.ApprovedAmount > 0)
            .OrderByDescending(x => x.DecisionDate)
            .Select(x => x.BeneficiaryId)
            .Distinct()
            .ToListAsync(ct);

        var notIncludedCount = approvedDecisionBeneficiaryIds.Count(x => !currentCycleBeneficiaryIds.Contains(x));
        if (notIncludedCount > 0)
        {
            alerts.Add(new AidCycleAlertItem
            {
                Title = "حالات معتمدة لم تدخل دورة الصرف",
                Message = $"يوجد {notIncludedCount} مستفيد/ة بقرارات لجنة معتمدة لم يتم إدراجهم في دورة الصرف الشهرية الحالية.",
                Kind = "charity.aid-cycle.monthly.not-included",
                Level = "warning",
                Url = "/AidCycles"
            });
        }

        return alerts;
    }
}
