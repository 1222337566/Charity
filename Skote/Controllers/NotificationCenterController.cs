using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skote.Helpers;

[Authorize]
public class NotificationCenterController : Controller
{
    private readonly IRemoteNotifyClient _remoteNotifyClient;

    public NotificationCenterController(IRemoteNotifyClient remoteNotifyClient)
    {
        _remoteNotifyClient = remoteNotifyClient;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendTestToMe(CancellationToken ct)
    {
        await _remoteNotifyClient.SendTestAsync(ct);
        TempData["NotificationCenterMessage"] = "تم إرسال إشعار تجريبي إلى حسابك الحالي.";
        return RedirectToAction(nameof(Index));
    }
}
