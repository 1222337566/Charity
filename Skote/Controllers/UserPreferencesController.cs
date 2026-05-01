using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Controllers
{
    [Authorize]
    public class UserPreferencesController : Controller
    {
        public IActionResult Index()
        {
            var prefs = LoadPrefs();
            return View(prefs);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Save(UserPreferencesVm vm)
        {
            var opts = new CookieOptions
            {
                Expires  = DateTimeOffset.UtcNow.AddYears(1),
                HttpOnly = false,   // JS needs to read some of these
                SameSite = SameSiteMode.Lax,
                IsEssential = true
            };
            Response.Cookies.Append("pref_darkmode",     vm.DarkMode.ToString(),           opts);
            Response.Cookies.Append("pref_soundnotify",  vm.SoundNotifications.ToString(), opts);
            Response.Cookies.Append("pref_dailysummary", vm.DailySummary.ToString(),       opts);
            Response.Cookies.Append("pref_defaultview",  vm.DefaultView ?? "Kanban",       opts);
            Response.Cookies.Append("pref_duealert",     vm.DueAlertDays.ToString(),       opts);

            TempData["PrefsMessage"] = "تم حفظ التفضيلات بنجاح ✓";
            return RedirectToAction(nameof(Index));
        }

        private UserPreferencesVm LoadPrefs() => new()
        {
            DarkMode           = bool.TryParse(Request.Cookies["pref_darkmode"],     out var d)  && d,
            SoundNotifications = !bool.TryParse(Request.Cookies["pref_soundnotify"], out var s)  || s,
            DailySummary       = !bool.TryParse(Request.Cookies["pref_dailysummary"],out var ds) || ds,
            DefaultView        = Request.Cookies["pref_defaultview"] ?? "Kanban",
            DueAlertDays       = int.TryParse(Request.Cookies["pref_duealert"], out var da) ? da : 3,
        };
    }

    public class UserPreferencesVm
    {
        public bool   DarkMode           { get; set; }
        public bool   SoundNotifications { get; set; } = true;
        public bool   DailySummary       { get; set; } = true;
        public string DefaultView        { get; set; } = "Kanban";
        public int    DueAlertDays       { get; set; } = 3;
    }
}
