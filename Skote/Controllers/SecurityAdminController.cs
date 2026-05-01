using InfrastructureManagmentCore.Domains.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Skote.Helpers;
using Skote.Seeding;
using Skote.ViewModels.Security;
using System.Security.Claims;

[Authorize(Policy = CharityPolicies.SettingsManage)]
public class SecurityAdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public SecurityAdminController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SeedDefaults()
    {
        var changed = await CharityIdentitySeeder.ReseedRoleClaimsAsync(_roleManager);
        TempData["Success"] = $"تم تحديث الأدوار وصلاحياتها الافتراضية. عدد التغييرات: {changed}";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Users(string? search)
    {
        var usersQuery = _userManager.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var q = search.Trim().ToLower();
            usersQuery = usersQuery.Where(x =>
                (x.UserName ?? string.Empty).ToLower().Contains(q) ||
                (x.Email ?? string.Empty).ToLower().Contains(q) ||
                (x.DisplayName ?? string.Empty).ToLower().Contains(q));
        }

        var users = usersQuery.OrderBy(x => x.UserName).ToList();
        var vm = new SecurityUsersVm { Search = search };

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var directPermissions = (await _userManager.GetClaimsAsync(user))
                .Where(c => c.Type == CharityPermissions.Type)
                .Select(c => c.Value)
                .OrderBy(x => x)
                .ToList();

            vm.Users.Add(new SecurityUserListItemVm
            {
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
                DisplayName = user.DisplayName,
                Email = user.Email,
                Roles = roles.OrderBy(x => x).ToList(),
                DirectPermissions = directPermissions
            });
        }

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> EditUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        var userPermissionClaims = (await _userManager.GetClaimsAsync(user))
            .Where(c => c.Type == CharityPermissions.Type)
            .Select(c => c.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var vm = new EditUserPermissionsVm
        {
            UserId = user.Id,
            UserName = user.UserName ?? string.Empty,
            DisplayName = user.DisplayName,
            Email = user.Email,
            Roles = CharityRoles.All
                .Select(r => new RoleSelectionVm
                {
                    RoleName = r,
                    Selected = userRoles.Contains(r, StringComparer.OrdinalIgnoreCase)
                })
                .ToList(),
            Permissions = CharityPermissions.All
                .Select(p => new PermissionSelectionVm
                {
                    Permission = p,
                    DisplayName = CharityPermissionDisplayNames.Get(p),
                    Selected = userPermissionClaims.Contains(p)
                })
                .ToList()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(EditUserPermissionsVm vm)
    {
        var user = await _userManager.FindByIdAsync(vm.UserId);
        if (user is null)
        {
            return NotFound();
        }

        vm.Roles ??= new();
        vm.Permissions ??= new();

        var currentRoles = await _userManager.GetRolesAsync(user);
        var selectedRoles = vm.Roles.Where(x => x.Selected).Select(x => x.RoleName).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var rolesToRemove = currentRoles.Where(r => !selectedRoles.Contains(r)).ToArray();
        if (rolesToRemove.Length > 0)
        {
            var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!removeRolesResult.Succeeded)
            {
                AddErrors(removeRolesResult);
            }
        }

        var rolesToAdd = selectedRoles.Where(r => !currentRoles.Contains(r, StringComparer.OrdinalIgnoreCase)).ToArray();
        foreach (var role in rolesToAdd)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }
        if (rolesToAdd.Length > 0)
        {
            var addRolesResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
            if (!addRolesResult.Succeeded)
            {
                AddErrors(addRolesResult);
            }
        }

        var currentClaims = await _userManager.GetClaimsAsync(user);
        var currentPermissions = currentClaims.Where(c => c.Type == CharityPermissions.Type).ToList();
        var selectedPermissions = vm.Permissions.Where(x => x.Selected).Select(x => x.Permission).ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var claim in currentPermissions.Where(c => !selectedPermissions.Contains(c.Value)).ToList())
        {
            var removeClaimResult = await _userManager.RemoveClaimAsync(user, claim);
            if (!removeClaimResult.Succeeded)
            {
                AddErrors(removeClaimResult);
            }
        }

        var existingPermissionValues = currentPermissions.Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var permission in selectedPermissions.Where(p => !existingPermissionValues.Contains(p)))
        {
            var addClaimResult = await _userManager.AddClaimAsync(user, new Claim(CharityPermissions.Type, permission));
            if (!addClaimResult.Succeeded)
            {
                AddErrors(addClaimResult);
            }
        }

        if (!ModelState.IsValid)
        {
            vm.UserName = user.UserName ?? string.Empty;
            vm.DisplayName = user.DisplayName;
            vm.Email = user.Email;
            return View(vm);
        }

        TempData["Success"] = "تم تحديث أدوار وصلاحيات المستخدم بنجاح.";
        return RedirectToAction(nameof(Users));
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }
}
