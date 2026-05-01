using InfrastrfuctureManagmentCore.Domains.HR;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Skote.Controllers
{
    [Authorize]
    public class HrLeaveRequestsController : Controller
    {
        private readonly AppDbContext _db;
        public HrLeaveRequestsController(AppDbContext db) => _db = db;

        // ── لوحة الإجازات الرئيسية ──
        public async Task<IActionResult> Index(Guid? employeeId, string? status, int? year, CancellationToken ct)
        {
            var q = _db.Set<HrLeaveRequest>()
                .AsNoTracking()
                .Include(x => x.Employee).ThenInclude(e => e!.Department)
                .Include(x => x.LeaveType)
                .AsQueryable();

            if (employeeId.HasValue) q = q.Where(x => x.EmployeeId == employeeId.Value);
            if (!string.IsNullOrWhiteSpace(status)) q = q.Where(x => x.Status == status);
            if (year.HasValue) q = q.Where(x => x.StartDate.Year == year.Value);

            var requests = await q.OrderByDescending(x => x.CreatedAtUtc).ToListAsync(ct);

            // إحصائيات
            var curYear = year ?? DateTime.Today.Year;
            ViewBag.PendingCount  = requests.Count(x => x.Status == "Pending");
            ViewBag.ApprovedCount = requests.Count(x => x.Status == "Approved");
            ViewBag.Year          = curYear;
            ViewBag.EmployeeId    = employeeId;
            ViewBag.Status        = status;

            ViewBag.Employees = await _db.Set<HrEmployee>()
                .Where(x => x.IsActive).OrderBy(x => x.FullName)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.FullName })
                .ToListAsync(ct);
            ViewBag.LeaveTypes = await _db.Set<HrLeaveType>().Where(x => x.IsActive).ToListAsync(ct);

            return View(requests);
        }

        // ── تقديم طلب إجازة ──
        [HttpGet]
        public async Task<IActionResult> Create(Guid? employeeId, CancellationToken ct)
        {
            await FillLookupsAsync(employeeId, ct);
            return View(new HrLeaveRequest
            {
                EmployeeId = employeeId ?? Guid.Empty,
                StartDate  = DateTime.Today,
                EndDate    = DateTime.Today
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HrLeaveRequest model, CancellationToken ct)
        {
            if (model.EndDate < model.StartDate)
                ModelState.AddModelError(nameof(model.EndDate), "تاريخ الانتهاء يجب أن يكون بعد تاريخ البداية");

            // احسب عدد الأيام (استبعاد الجمعة والسبت)
            var days = CalcWorkingDays(model.StartDate, model.EndDate);
            if (model.IsHalfDay) days = 0.5m;
            if (days <= 0 && !model.IsHalfDay)
                ModelState.AddModelError("", "لا توجد أيام عمل ضمن الفترة المحددة");

            // تحقق من تقاطع مع إجازة موجودة
            var overlap = await _db.Set<HrLeaveRequest>()
                .AnyAsync(x => x.EmployeeId == model.EmployeeId
                    && x.Status != "Rejected" && x.Status != "Cancelled"
                    && x.StartDate <= model.EndDate && x.EndDate >= model.StartDate, ct);
            if (overlap)
                ModelState.AddModelError("", "يوجد طلب إجازة آخر في نفس الفترة أو يتقاطع معها");

            // تحقق من رصيد الإجازة
            var balance = await GetOrCreateBalanceAsync(model.EmployeeId, model.LeaveTypeId,
                DateTime.Today.Year, ct);
            if (balance.Remaining < days)
                ModelState.AddModelError("", $"الرصيد المتبقي ({balance.Remaining:N1} يوم) لا يكفي لهذا الطلب ({days:N1} يوم)");

            await FillLookupsAsync(model.EmployeeId, ct);
            if (!ModelState.IsValid) return View(model);

            model.Id            = Guid.NewGuid();
            model.RequestNumber = $"LVR-{DateTime.UtcNow:yyyyMMddHHmmss}";
            model.TotalDays     = (int)Math.Ceiling(days);
            model.Status        = "Pending";
            model.CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // حجز الرصيد المعلق
            balance.TotalPending += days;
            _db.Set<HrLeaveRequest>().Add(model);
            await _db.SaveChangesAsync(ct);

            TempData["Success"] = $"تم تقديم طلب الإجازة رقم {model.RequestNumber}";
            return RedirectToAction(nameof(Index), new { employeeId = model.EmployeeId });
        }

        // ── موافقة ──
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HrOfficer,CharityManager")]
        public async Task<IActionResult> Approve(Guid id, string? notes, CancellationToken ct)
        {
            var req = await _db.Set<HrLeaveRequest>().FindAsync(new object[] { id }, ct);
            if (req == null) return NotFound();

            req.Status            = "Approved";
            req.ApprovedByUserId  = User.FindFirstValue(ClaimTypes.NameIdentifier);
            req.ApprovedAtUtc     = DateTime.UtcNow;
            req.Notes             = notes;
            req.UpdatedAtUtc      = DateTime.UtcNow;

            // خصم من الرصيد
            var balance = await GetOrCreateBalanceAsync(req.EmployeeId, req.LeaveTypeId,
                req.StartDate.Year, ct);
            balance.TotalUsed    += req.TotalDays;
            balance.TotalPending -= req.TotalDays;
            balance.UpdatedAtUtc  = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);
            TempData["Success"] = "تمت الموافقة على طلب الإجازة";
            return RedirectToAction(nameof(Index));
        }

        // ── رفض ──
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HrOfficer,CharityManager")]
        public async Task<IActionResult> Reject(Guid id, string reason, CancellationToken ct)
        {
            var req = await _db.Set<HrLeaveRequest>().FindAsync(new object[] { id }, ct);
            if (req == null) return NotFound();

            req.Status           = "Rejected";
            req.RejectionReason  = reason;
            req.UpdatedAtUtc     = DateTime.UtcNow;

            var balance = await GetOrCreateBalanceAsync(req.EmployeeId, req.LeaveTypeId,
                req.StartDate.Year, ct);
            balance.TotalPending -= req.TotalDays;
            balance.UpdatedAtUtc  = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);
            TempData["Warning"] = "تم رفض طلب الإجازة";
            return RedirectToAction(nameof(Index));
        }

        // ── إلغاء ──
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
        {
            var req = await _db.Set<HrLeaveRequest>().FindAsync(new object[] { id }, ct);
            if (req == null) return NotFound();

            if (req.Status == "Approved" && req.StartDate <= DateTime.Today)
            {
                TempData["Error"] = "لا يمكن إلغاء إجازة بدأت بالفعل";
                return RedirectToAction(nameof(Index));
            }

            var balance = await GetOrCreateBalanceAsync(req.EmployeeId, req.LeaveTypeId,
                req.StartDate.Year, ct);
            if (req.Status == "Pending")   balance.TotalPending -= req.TotalDays;
            if (req.Status == "Approved")  balance.TotalUsed    -= req.TotalDays;
            balance.UpdatedAtUtc = DateTime.UtcNow;

            req.Status       = "Cancelled";
            req.UpdatedAtUtc = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);

            TempData["Success"] = "تم إلغاء طلب الإجازة";
            return RedirectToAction(nameof(Index));
        }

        // ── رصيد الإجازات للموظف ──
        public async Task<IActionResult> Balance(Guid employeeId, int? year, CancellationToken ct)
        {
            var curYear = year ?? DateTime.Today.Year;
            var emp = await _db.Set<HrEmployee>()
                .Include(x => x.Department).Include(x => x.JobTitle)
                .FirstOrDefaultAsync(x => x.Id == employeeId, ct);
            if (emp == null) return NotFound();

            var leaveTypes = await _db.Set<HrLeaveType>().Where(x => x.IsActive).ToListAsync(ct);

            // جيب أو أنشئ أرصدة لكل نوع
            var balances = new List<HrLeaveBalance>();
            foreach (var lt in leaveTypes)
            {
                var b = await GetOrCreateBalanceAsync(employeeId, lt.Id, curYear, ct);
                balances.Add(b);
            }

            // آخر 10 طلبات
            var history = await _db.Set<HrLeaveRequest>()
                .AsNoTracking()
                .Include(x => x.LeaveType)
                .Where(x => x.EmployeeId == employeeId && x.StartDate.Year == curYear)
                .OrderByDescending(x => x.StartDate)
                .Take(20).ToListAsync(ct);

            ViewBag.Employee  = emp;
            ViewBag.Year      = curYear;
            ViewBag.LeaveTypes = leaveTypes;
            ViewBag.History   = history;
            return View(balances);
        }

        // ── تعديل رصيد يدوياً (Admin فقط) ──
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HrOfficer")]
        public async Task<IActionResult> AdjustBalance(
            Guid employeeId, Guid leaveTypeId, int year,
            decimal newEntitled, decimal adjustment, string? note, CancellationToken ct)
        {
            var balance = await GetOrCreateBalanceAsync(employeeId, leaveTypeId, year, ct);
            if (newEntitled >= 0)
                balance.TotalEntitled = newEntitled;
            if (adjustment != 0)
                balance.TotalEntitled += adjustment;
            balance.UpdatedAtUtc = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            TempData["Success"] = "تم تعديل الرصيد";
            return RedirectToAction(nameof(Balance), new { employeeId, year });
        }

        // ── إدارة أنواع الإجازات ──
        [Authorize(Roles = "Admin,HrOfficer")]
        public async Task<IActionResult> LeaveTypes(CancellationToken ct)
        {
            var types = await _db.Set<HrLeaveType>().OrderBy(x => x.NameAr).ToListAsync(ct);
            return View(types);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HrOfficer")]
        public async Task<IActionResult> SaveLeaveType(HrLeaveType model, CancellationToken ct)
        {
            var existing = await _db.Set<HrLeaveType>().FindAsync(new object[] { model.Id }, ct);
            if (existing == null)
            {
                model.Id = Guid.NewGuid();
                _db.Set<HrLeaveType>().Add(model);
            }
            else
            {
                existing.NameAr = model.NameAr; existing.NameEn = model.NameEn;
                existing.Category = model.Category; existing.MaxDaysPerYear = model.MaxDaysPerYear;
                existing.MaxConsecutiveDays = model.MaxConsecutiveDays;
                existing.RequiresAttachment = model.RequiresAttachment;
                existing.PaidLeave = model.PaidLeave; existing.CarryOverAllowed = model.CarryOverAllowed;
                existing.MaxCarryOverDays = model.MaxCarryOverDays; existing.Color = model.Color;
                existing.IsActive = model.IsActive;
            }
            await _db.SaveChangesAsync(ct);
            TempData["Success"] = "تم حفظ نوع الإجازة";
            return RedirectToAction(nameof(LeaveTypes));
        }

        // ══════════════════════════════════════════════════
        //  Helpers
        // ══════════════════════════════════════════════════
        private async Task<HrLeaveBalance> GetOrCreateBalanceAsync(
            Guid empId, Guid typeId, int year, CancellationToken ct)
        {
            var balance = await _db.Set<HrLeaveBalance>()
                .FirstOrDefaultAsync(x => x.EmployeeId == empId
                    && x.LeaveTypeId == typeId && x.Year == year, ct);

            if (balance != null) return balance;

            var lt = await _db.Set<HrLeaveType>().FindAsync(new object[] { typeId }, ct);
            balance = new HrLeaveBalance
            {
                Id            = Guid.NewGuid(),
                EmployeeId    = empId,
                LeaveTypeId   = typeId,
                Year          = year,
                TotalEntitled = lt?.MaxDaysPerYear ?? 21,
                UpdatedAtUtc  = DateTime.UtcNow
            };
            _db.Set<HrLeaveBalance>().Add(balance);
            await _db.SaveChangesAsync(ct);
            return balance;
        }

        private static decimal CalcWorkingDays(DateTime start, DateTime end)
        {
            var days = 0m;
            for (var d = start.Date; d <= end.Date; d = d.AddDays(1))
                if (d.DayOfWeek != DayOfWeek.Friday && d.DayOfWeek != DayOfWeek.Saturday)
                    days++;
            return days;
        }

        private async Task FillLookupsAsync(Guid? empId, CancellationToken ct)
        {
            ViewBag.Employees = await _db.Set<HrEmployee>()
                .Where(x => x.IsActive).OrderBy(x => x.FullName)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.FullName })
                .ToListAsync(ct);
            ViewBag.LeaveTypes = await _db.Set<HrLeaveType>()
                .Where(x => x.IsActive).OrderBy(x => x.NameAr)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.NameAr })
                .ToListAsync(ct);

            if (empId.HasValue && empId != Guid.Empty)
            {
                var curYear = DateTime.Today.Year;
                var types   = await _db.Set<HrLeaveType>().Where(x => x.IsActive).ToListAsync(ct);
                var bals    = await _db.Set<HrLeaveBalance>()
                    .Where(x => x.EmployeeId == empId && x.Year == curYear)
                    .ToListAsync(ct);
                ViewBag.BalanceMap = types.ToDictionary(
                    t => t.Id,
                    t => bals.FirstOrDefault(b => b.LeaveTypeId == t.Id)?.Remaining ?? (decimal)t.MaxDaysPerYear
                );
            }
        }
    }
}
