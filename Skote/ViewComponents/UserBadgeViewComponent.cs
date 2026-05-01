using System.Security.Claims;
using InfrastructureManagmentServices.Profile;
using Microsoft.AspNetCore.Mvc;

public class UserBadgeViewComponent : ViewComponent
{
    private readonly IProfileService _profile; // الخدمة بتقرأ من الريبو

    public UserBadgeViewComponent(IProfileService profile) => _profile = profile;

    // تقدر تستدعيه كده:
    // @await Component.InvokeAsync("UserBadge")                 // الوضع الافتراضي (li)
    // @await Component.InvokeAsync("UserBadge", new { mode="flex" }) // لو التوب بار بدون <ul>
    public async Task<IViewComponentResult> InvokeAsync(string mode = "li")
    {
        var vm = new Vm();
        if (User?.Identity?.IsAuthenticated == true)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = HttpContext.User.Identity?.Name;
            var p = await _profile.GetAsync(userId);

            vm.UserName = userName;
            vm.DisplayName = p?.FullName;
            vm.Photo = p?.ProfileImagePath; // مثال: /uploads/profiles/xxx.jpg
        }

        return View(mode?.Equals("flex", StringComparison.OrdinalIgnoreCase) == true ? "Flex" : "Default", vm);
    }

    public class Vm
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Photo { get; set; }
    }
}
