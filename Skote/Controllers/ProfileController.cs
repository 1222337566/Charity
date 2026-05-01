// Skote/Controllers/ProfileController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InfrastructureManagmentCore.Domains.Identity;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using InfrastructureManagmentServices.Profile;
using InfrastructureManagmentWebFramework.DTOs.Profile;

[Authorize]
public class ProfileController : Controller
{
    private readonly IProfileService _profile;
    private readonly UserManager<ApplicationUser> _um;

    public ProfileController(IProfileService profile, UserManager<ApplicationUser> um)
    { _profile = profile; _um = um; }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _um.GetUserAsync(User);
        var vm = await _profile.GetAsync(user.Id);
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var user = await _um.GetUserAsync(User);
        var vm = await _profile.GetAsync(user.Id);
        return View(new ProfileEditDto
        {
            FullName = vm?.FullName,
            Phone = vm?.Phone,
            Department = vm?.Department,
            JobTitle = vm?.JobTitle,
            Address = vm?.Address,
            Gender = vm?.Gender,
            BirthDate = vm?.BirthDate,
            NationalId = vm?.NationalId,
            ProfileImagePath = vm?.ProfileImagePath
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromForm] ProfileEditDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        var user = await _um.GetUserAsync(User);
        var r = await _profile.UpdateAsync(user.Id, dto);
        if (!r.Succeeded)
        {
            ModelState.AddModelError(string.Empty, r.Message ?? "فشل التحديث.");
            return View(dto);
        }
        return RedirectToAction(nameof(Index));
    }
}
