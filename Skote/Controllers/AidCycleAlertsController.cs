using InfrastructureManagmentServices.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Skote.Controllers;

[Authorize]
public class AidCycleAlertsController : Controller
{
    private readonly IAidCycleReminderService _aidCycleReminderService;

    public AidCycleAlertsController(IAidCycleReminderService aidCycleReminderService)
    {
        _aidCycleReminderService = aidCycleReminderService;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var items = await _aidCycleReminderService.BuildAlertsAsync(DateTime.Today, ct);
        return View(items);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RunNow(CancellationToken ct)
    {
        var actorUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _aidCycleReminderService.RunAsync(DateTime.Today, actorUserId, ct);
        TempData["Success"] = $"تم تشغيل الفحص وإنشاء/تحديث {result.CreatedNotificationsCount} تنبيه/ات.";
        return RedirectToAction(nameof(Index));
    }
}
