using InfrastrfuctureManagmentCore.Domains.HR;
using InfrastructureManagmentCore.Domains.Identity;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Skote.Helpers;

namespace Skote.Controllers
{
    [Authorize(Roles = "Admin,HrOfficer")]
    public class EmployeeUserLinkController : Controller
    {
        private readonly AppDbContext                     _db;
        private readonly UserManager<ApplicationUser>     _userMgr;
        private readonly RoleManager<IdentityRole>        _roleMgr;

        public EmployeeUserLinkController(
            AppDbContext db,
            UserManager<ApplicationUser> userMgr,
            RoleManager<IdentityRole> roleMgr)
        {
            _db      = db;
            _userMgr = userMgr;
            _roleMgr = roleMgr;
        }

        // ══════════════════════════════════════════════════
        // لوحة ربط الموظفين بالمستخدمين
        // ══════════════════════════════════════════════════
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var employees = await _db.Set<HrEmployee>()
                .AsNoTracking()
                .Include(x => x.Department)
                .Include(x => x.JobTitle)
                .Where(x => x.IsActive)
                .OrderBy(x => x.FullName)
                .ToListAsync(ct);

            // جيب المستخدمين المرتبطين بموظفين
            var linkedIds = employees
                .Where(e => e.UserId != null).Select(e => e.UserId!).ToList();

            var users = await _userMgr.Users
                .Where(u => linkedIds.Contains(u.Id))
                .ToListAsync();

            var usersByEmployeeId = employees
                .Where(e => e.UserId != null)
                .ToDictionary(
                    e => e.Id,
                    e => users.FirstOrDefault(u => u.Id == e.UserId)
                );

            ViewBag.UsersByEmployeeId = usersByEmployeeId;
            ViewBag.AllRoles          = CharityRoles.All.ToList();
            return View(employees);
        }

        // ══════════════════════════════════════════════════
        // إنشاء حساب مستخدم جديد للموظف
        // ══════════════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> CreateAccount(Guid employeeId, CancellationToken ct)
        {
            var emp = await _db.Set<HrEmployee>()
                .Include(x => x.JobTitle)
                .FirstOrDefaultAsync(x => x.Id == employeeId, ct);
            if (emp == null) return NotFound();

            // اقترح username من الاسم
            var suggested = GenerateUsername(emp.FullName);
            var suggestedRole = emp.JobTitle?.SystemRole
                ?? MapJobTitleToRole(emp.JobTitle?.Name);

            ViewBag.Employee      = emp;
            ViewBag.AllRoles      = CharityRoles.All
                .Select(r => new SelectListItem { Value = r, Text = RoleAr(r) })
                .ToList();
            ViewBag.SuggestedUsername = suggested;
            ViewBag.SuggestedRole     = suggestedRole;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAccount(
            Guid employeeId, string username, string email,
            string password, string role, CancellationToken ct)
        {
            var emp = await _db.Set<HrEmployee>()
                .Include(x => x.JobTitle)
                .FirstOrDefaultAsync(x => x.Id == employeeId, ct);
            if (emp == null) return NotFound();

            ViewBag.Employee  = emp;
            ViewBag.AllRoles  = CharityRoles.All
                .Select(r => new SelectListItem { Value = r, Text = RoleAr(r) })
                .ToList();

            // تحقق من عدم وجود مستخدم بنفس الاسم
            if (await _userMgr.FindByNameAsync(username) != null)
            {
                ModelState.AddModelError("", $"اسم المستخدم «{username}» مستخدم مسبقاً");
                return View();
            }

            var user = new ApplicationUser
            {
                UserName     = username,
                Email        = email,
                DisplayName  = emp.FullName,
                Department   = emp.Department?.Name,
                HrEmployeeId = emp.Id,
                IsActive     = true,
                EmailConfirmed = true
            };

            var result = await _userMgr.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError("", err.Description);
                return View();
            }

            // أسند الدور
            if (!string.IsNullOrWhiteSpace(role))
                await _userMgr.AddToRoleAsync(user, role);

            // ربط الموظف بالمستخدم
            emp.UserId = user.Id;
            await _db.SaveChangesAsync(ct);

            TempData["Success"] = $"تم إنشاء حساب «{username}» وربطه بالموظف {emp.FullName}";
            return RedirectToAction(nameof(Index));
        }

        // ══════════════════════════════════════════════════
        // ربط موظف بمستخدم موجود
        // ══════════════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> LinkExistingUser(Guid employeeId, CancellationToken ct)
        {
            var emp = await _db.Set<HrEmployee>()
                .Include(x => x.JobTitle)
                .FirstOrDefaultAsync(x => x.Id == employeeId, ct);
            if (emp == null) return NotFound();

            // قائمة المستخدمين غير المرتبطين بموظف
            var linkedUserIds = await _db.Set<HrEmployee>()
                .Where(x => x.UserId != null && x.Id != employeeId)
                .Select(x => x.UserId!).ToListAsync(ct);

            var freeUsers = await _userMgr.Users
                .Where(u => !linkedUserIds.Contains(u.Id))
                .OrderBy(u => u.UserName)
                .ToListAsync();

            ViewBag.Employee = emp;
            ViewBag.Users    = freeUsers.Select(u => new SelectListItem
            {
                Value = u.Id,
                Text  = $"{u.UserName} — {u.DisplayName ?? u.Email}"
            }).ToList();
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkExistingUser(
            Guid employeeId, string userId, string? role, CancellationToken ct)
        {
            var emp  = await _db.Set<HrEmployee>().FindAsync(new object[] { employeeId }, ct);
            var user = await _userMgr.FindByIdAsync(userId);
            if (emp == null || user == null) return NotFound();

            emp.UserId       = userId;
            user.HrEmployeeId = employeeId;
            user.DisplayName  = emp.FullName;
            await _db.SaveChangesAsync(ct);

            if (!string.IsNullOrWhiteSpace(role))
                await _userMgr.AddToRoleAsync(user, role);

            TempData["Success"] = $"تم ربط الموظف {emp.FullName} بالمستخدم {user.UserName}";
            return RedirectToAction(nameof(Index));
        }

        // ══════════════════════════════════════════════════
        // إدارة أدوار مستخدم
        // ══════════════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> ManageRoles(Guid employeeId, CancellationToken ct)
        {
            var emp = await _db.Set<HrEmployee>()
                .Include(x => x.JobTitle)
                .FirstOrDefaultAsync(x => x.Id == employeeId, ct);
            if (emp == null || emp.UserId == null)
            {
                TempData["Error"] = "لا يوجد حساب مرتبط بهذا الموظف";
                return RedirectToAction(nameof(Index));
            }

            var user         = await _userMgr.FindByIdAsync(emp.UserId);
            if (user == null) return NotFound();

            var currentRoles = (await _userMgr.GetRolesAsync(user)).ToHashSet();
            var suggestedRole = emp.JobTitle?.SystemRole ?? MapJobTitleToRole(emp.JobTitle?.Name);

            ViewBag.Employee       = emp;
            ViewBag.User           = user;
            ViewBag.SuggestedRole  = suggestedRole;
            ViewBag.AllRoles       = CharityRoles.All
                .Select(r => new { Role = r, NameAr = RoleAr(r), IsAssigned = currentRoles.Contains(r) })
                .ToList();
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(
            Guid employeeId, List<string> selectedRoles, CancellationToken ct)
        {
            var emp = await _db.Set<HrEmployee>()
                .FirstOrDefaultAsync(x => x.Id == employeeId, ct);
            if (emp?.UserId == null) return NotFound();

            var user         = await _userMgr.FindByIdAsync(emp.UserId);
            if (user == null) return NotFound();

            var currentRoles = await _userMgr.GetRolesAsync(user);
            await _userMgr.RemoveFromRolesAsync(user, currentRoles);
            if (selectedRoles.Any())
                await _userMgr.AddToRolesAsync(user, selectedRoles);

            TempData["Success"] = $"تم تحديث صلاحيات {emp.FullName}";
            return RedirectToAction(nameof(Index));
        }

        // ══════════════════════════════════════════════════
        // تعطيل / تفعيل الحساب
        // ══════════════════════════════════════════════════
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAccount(Guid employeeId)
        {
            var emp = await _db.Set<HrEmployee>()
                .FirstOrDefaultAsync(x => x.Id == employeeId);
            if (emp?.UserId == null) return NotFound();

            var user = await _userMgr.FindByIdAsync(emp.UserId);
            if (user == null) return NotFound();

            user.IsActive      = !user.IsActive;
            user.LockoutEnd    = user.IsActive ? null : DateTimeOffset.MaxValue;
            user.LockoutEnabled = true;
            await _userMgr.UpdateAsync(user);

            TempData["Success"] = user.IsActive
                ? $"تم تفعيل حساب {emp.FullName}"
                : $"تم تعطيل حساب {emp.FullName}";
            return RedirectToAction(nameof(Index));
        }

        // ══════════════════════════════════════════════════
        // إعادة تعيين كلمة المرور
        // ══════════════════════════════════════════════════
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(Guid employeeId, string newPassword)
        {
            var emp = await _db.Set<HrEmployee>()
                .FirstOrDefaultAsync(x => x.Id == employeeId);
            if (emp?.UserId == null) return NotFound();

            var user  = await _userMgr.FindByIdAsync(emp.UserId);
            if (user == null) return NotFound();

            var token = await _userMgr.GeneratePasswordResetTokenAsync(user);
            var result = await _userMgr.ResetPasswordAsync(user, token, newPassword);

            TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
                ? $"تم إعادة تعيين كلمة المرور لـ {emp.FullName}"
                : string.Join(" / ", result.Errors.Select(e => e.Description));
            return RedirectToAction(nameof(Index));
        }

        // ══════════════════════════════════════════════════
        // Helpers
        // ══════════════════════════════════════════════════
        private static string GenerateUsername(string fullName)
        {
            var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
                return (parts[0] + "." + parts[1]).ToLowerInvariant()
                    .Replace("أ","a").Replace("إ","a").Replace("ا","a")
                    .Replace("ب","b").Replace("ت","t").Replace("ث","th")
                    .Replace("ج","g").Replace("ح","h").Replace("خ","kh")
                    .Replace("د","d").Replace("ذ","z").Replace("ر","r")
                    .Replace("ز","z").Replace("س","s").Replace("ش","sh")
                    .Replace("ص","s").Replace("ض","d").Replace("ط","t")
                    .Replace("ظ","z").Replace("ع","a").Replace("غ","gh")
                    .Replace("ف","f").Replace("ق","q").Replace("ك","k")
                    .Replace("ل","l").Replace("م","m").Replace("ن","n")
                    .Replace("ه","h").Replace("و","w").Replace("ي","y")
                    .Replace("ى","a").Replace("ة","a");
            return fullName.Length > 0 ? fullName[0].ToString().ToLower() + "user" : "user";
        }

        private static string MapJobTitleToRole(string? jobTitle) => jobTitle switch
        {
            "مدير تنفيذي" or "مدير مشروعات" => CharityRoles.CharityManager,
            "باحث اجتماعي" or "أخصائي اجتماعي" => CharityRoles.SocialResearcher,
            "محاسب" or "مدير مالي" => CharityRoles.FinancialOfficer,
            "أمين مخزن" or "مخزن" => CharityRoles.StoreKeeper,
            "مسؤول موارد بشرية" or "HR" => CharityRoles.HrOfficer,
            "مسؤول رواتب" or "مسؤول مرتبات" => CharityRoles.PayrollOfficer,
            "مسؤول علاقات المتبرعين" => CharityRoles.DonorRelations,
            "مشرف مشروع" or "مدير مشروع" => CharityRoles.ProjectManager,
            "مسؤول مستفيدين" => CharityRoles.BeneficiariesOfficer,
            _ => CharityRoles.Reviewer
        };

        private static string RoleAr(string role) => role switch
        {
            "Admin"                 => "مدير النظام",
            "CharityManager"        => "مدير الجمعية",
            "BeneficiariesOfficer"  => "مسؤول المستفيدين",
            "SocialResearcher"      => "باحث اجتماعي",
            "DonorRelations"        => "مسؤول المتبرعين",
            "FunderRelations"       => "مسؤول الممولين",
            "ProjectManager"        => "مدير مشروعات",
            "StoreKeeper"           => "أمين مخزن",
            "HrOfficer"             => "مسؤول الموارد البشرية",
            "PayrollOfficer"        => "مسؤول الرواتب",
            "FinancialOfficer"      => "مسؤول المالية",
            "Accountant"            => "محاسب",
            "Reviewer"              => "مراجع",
            "ReportsViewer"         => "مستعرض تقارير",
            _ => role
        };
    }
}
