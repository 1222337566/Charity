using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using InfrastructureManagmentCore.Persistence; // ApplicationDbContext
using InfrastructureManagmentCore.Domains.Calendar;
using InfrastructureManagmentCore.Services.Notifications;
using InfrastructureManagmentDataAccess.EntityFramework;

namespace InfrastructureManagementRealtime.Services;

public class CalendarReminderService : BackgroundService
{
    private readonly ILogger<CalendarReminderService> _log;
    private readonly IServiceProvider _sp;
    // كل كام دقيقة نتحقق؟
    private readonly TimeSpan _pollInterval = TimeSpan.FromMinutes(1);
    // تسامح (دقيقة) لتفادي الفجوات لو السيرفس اتأخر ثانية أو اثنين
    private readonly TimeSpan _tolerance = TimeSpan.FromSeconds(30);

    public CalendarReminderService(ILogger<CalendarReminderService> log, IServiceProvider sp)
    { _log = log; _sp = sp; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.LogInformation("CalendarReminderService started");
        while (!stoppingToken.IsCancellationRequested)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                using var scope = _sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var notify = scope.ServiceProvider.GetRequiredService<INotificationService>();

                var now = DateTime.UtcNow;

                // هنجلب كل الأحداث اللي ليها Reminder يساوي الوقت الحالي تقريبًا (ضمن تسامح)
                // StartUtc - MinutesBefore ≈ now
                var reminders = await db.EventReminders
                    .AsNoTracking()
                    .Include(r => r.Event)
                    .Where(r =>
                        // نجيب نافذة ±tolerance حول "الوقت المستهدف"
                        Math.Abs(
                            (r.Event.StartUtc - TimeSpan.FromMinutes(r.MinutesBefore) - now).TotalSeconds
                        ) <= _tolerance.TotalSeconds
                    )
                    .ToListAsync(stoppingToken);

                foreach (var r in reminders)
                {
                    var ev = r.Event;
                    if (ev is null) continue;

                    // هات الحضور
                    var attendees = await db.CalendarAttendees
                        .AsNoTracking()
                        .Where(a => a.EventId == ev.Id)
                        .Select(a => a.UserId)
                        .ToListAsync(stoppingToken);

                    // ابعت إشعار لكل مدعو
                    foreach (var uid in attendees)
                    {
                        await notify.ToUserAsync(
                            ev.CreatedByUserId,
                            uid,
                            "Reminder",
                            $"الموعد {ev.Title} سيبدأ {r.MinutesBefore} دقيقة",
                            level: "warning",
                            ct: stoppingToken
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error while processing calendar reminders");
            }

            // انتظر باقي المدة من الـ interval
            var wait = _pollInterval - sw.Elapsed;
            if (wait < TimeSpan.Zero) wait = TimeSpan.FromSeconds(5);
            try { await Task.Delay(wait, stoppingToken); } catch { }
        }
        _log.LogInformation("CalendarReminderService stopped");
    }
}
