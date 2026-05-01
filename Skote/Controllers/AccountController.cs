using InfrastructureManagmentServices.Authentication;
using InfrastructureManagmentServices.Register;
using InfrastructureManagmentWebFramework.DTOs.Register;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
[AllowAnonymous]
public class AccountController : Controller
{
    private readonly IRegisterationService _registration;
    private readonly IAuthservice _auth;
    public AccountController(IRegisterationService registration, IAuthservice authservice)
    {
        _registration = registration;
        _auth = authservice;
    }
    [HttpGet, AllowAnonymous]
    public IActionResult Login(string returnUrl = null) => View(new LoginDto { ReturnUrl = returnUrl });

    [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto model)
    {
        if (!ModelState.IsValid) return View(model);
        var r = await _auth.LoginAsync(model);
        if (!r.Succeeded)
        {
            ModelState.AddModelError(string.Empty, r.Message ?? "فشل تسجيل الدخول.");
            return View(model);
        }
        if (!string.IsNullOrWhiteSpace(model.ReturnUrl)) return Redirect(model.ReturnUrl);
        return RedirectToAction("Index", "Dashboard3");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _auth.LogoutAsync();
        return RedirectToAction(nameof(Login));
    }

    [HttpGet , AllowAnonymous]
    public IActionResult Register() => View(new RegisterWithInfoDto());

    [ValidateAntiForgeryToken]
    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> Register([FromForm] RegisterWithInfoDto model, CancellationToken ct)
    {
        var result = await _registration.RegisterWithPersonalInfoAsync(model, ct);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet, AllowAnonymous]
    public IActionResult Denied() => View();
}
