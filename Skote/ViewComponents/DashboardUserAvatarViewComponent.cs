using InfrastructureManagmentCore.Domains.Profiling;
using InfrastructureManagmentServices.Profile;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class DashboardUserAvatarViewComponent : ViewComponent
{
    private readonly IProfileService _profile; // بتتعامل داخليًا مع الريبو/UoW

    public DashboardUserAvatarViewComponent(IProfileService profile)
        => _profile = profile;

    // تقدر تغيّر الحجم والـ welcome text من الاستدعاء
    public async Task<IViewComponentResult> InvokeAsync(string size = "xl", bool showWelcome = true)
    {
        var vm = new Vm2 { Size = size, ShowWelcome = showWelcome };

        if (User?.Identity?.IsAuthenticated == true)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = HttpContext.User.Identity?.Name;
            

            var p = await _profile.GetAsync(userId); // لا DbContext هنا
            vm.Name = string.IsNullOrWhiteSpace(p?.FullName) ? userName : p.FullName;
            vm.Photo = string.IsNullOrWhiteSpace(p?.ProfileImagePath)
                       ? Url.Content("~/assets/images/users/avatar-1.jpg")
                       : p.ProfileImagePath; // مثال: /uploads/profiles/xxx.jpg
            vm.job = string.IsNullOrWhiteSpace(p?.JobTitle) ? "Employee" : p.JobTitle;
        }

        return View(vm);
    }

    public class Vm2
    {
        public string Name { get; set; }
        public string Photo { get; set; }
        public string Size { get; set; }      // sm | md | lg | xl
        public bool ShowWelcome { get; set; }
        public string job { get; set; }
    }
}
