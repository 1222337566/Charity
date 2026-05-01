using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;
using InfrastrfuctureManagmentCore.Domains.HR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Skote.Controllers
{
    [Authorize]
    public class ProjectTeamMembersController : Controller
    {
        private readonly AppDbContext _db;
        public ProjectTeamMembersController(AppDbContext db) => _db = db;

        // ══ قائمة أعضاء الفريق ══════════════════════════════════════════════
        public async Task<IActionResult> Index(Guid projectId, string? type, CancellationToken ct)
        {
            if (!await SetProjectHeaderAsync(projectId, ct)) return NotFound();

            var query = _db.Set<ProjectTeamMember>()
                .AsNoTracking()
                .Include(x => x.Employee)
                .Include(x => x.Volunteer)
                .Include(x => x.Phase)
                .Include(x => x.Activity)
                .Where(x => x.ProjectId == projectId);

            if (!string.IsNullOrEmpty(type))
                query = query.Where(x => x.MemberType == type);

            var members = await query
                .OrderBy(x => x.MemberType)
                .ThenBy(x => x.StartDate)
                .ToListAsync(ct);

            // إحصائيات
            var all       = await _db.Set<ProjectTeamMember>().AsNoTracking()
                                .Where(x => x.ProjectId == projectId).ToListAsync(ct);
            ViewBag.TotalEmployees   = all.Count(x => x.MemberType == "Employee");
            ViewBag.TotalVolunteers  = all.Count(x => x.MemberType == "Volunteer");
            ViewBag.TotalHours       = all.Sum(x => x.ActualHours);
            ViewBag.TotalDays        = all.Sum(x => x.ActualDays ?? 0);
            ViewBag.ActiveCount      = all.Count(x => x.ParticipationStatus == "Active");
            ViewBag.CompletedCount   = all.Count(x => x.ParticipationStatus == "Completed");
            ViewBag.FilterType       = type;

            return View(members);
        }

        // ══ إضافة عضو ═══════════════════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> Create(Guid projectId, CancellationToken ct)
        {
            if (!await SetProjectHeaderAsync(projectId, ct)) return NotFound();
            var vm = new ProjectTeamMemberFormVm { ProjectId = projectId, StartDate = DateTime.Today };
            await FillLookupsAsync(vm, ct);
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectTeamMemberFormVm vm, CancellationToken ct)
        {
            if (!await SetProjectHeaderAsync(vm.ProjectId, ct)) return NotFound();
            await FillLookupsAsync(vm, ct);

            if (string.IsNullOrWhiteSpace(vm.RoleTitle))
                ModelState.AddModelError(nameof(vm.RoleTitle), "الدور مطلوب.");
            if (vm.MemberType == "Employee" && !vm.EmployeeId.HasValue)
                ModelState.AddModelError(nameof(vm.EmployeeId), "اختر الموظف.");
            if (vm.MemberType == "Volunteer" && !vm.VolunteerId.HasValue)
                ModelState.AddModelError(nameof(vm.VolunteerId), "اختر المتطوع.");
            if (!ModelState.IsValid) return View(vm);

            var entity = BuildEntity(vm);
            entity.CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _db.Set<ProjectTeamMember>().Add(entity);
            await SaveWithAttachmentsAsync(entity, vm, ct);

            TempData["Success"] = "تم إضافة عضو الفريق.";
            return RedirectToAction(nameof(Index), new { projectId = vm.ProjectId });
        }

        // ══ تعديل ══════════════════════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
        {
            var entity = await LoadWithAttachmentsAsync(id, ct);
            if (entity == null) return NotFound();
            if (!await SetProjectHeaderAsync(entity.ProjectId, ct)) return NotFound();

            var vm = MapToVm(entity);
            await FillLookupsAsync(vm, ct);
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProjectTeamMemberFormVm vm, CancellationToken ct)
        {
            var entity = await LoadWithAttachmentsAsync(vm.Id!.Value, ct);
            if (entity == null) return NotFound();
            if (!await SetProjectHeaderAsync(entity.ProjectId, ct)) return NotFound();
            await FillLookupsAsync(vm, ct);

            if (!ModelState.IsValid) return View(vm);

            ApplyToEntity(vm, entity);
            entity.UpdatedAtUtc = DateTime.UtcNow;
            await SaveWithAttachmentsAsync(entity, vm, ct);

            TempData["Success"] = "تم تحديث بيانات عضو الفريق.";
            return RedirectToAction(nameof(Index), new { projectId = entity.ProjectId });
        }

        // ══ إثبات المشاركة ═══════════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> Verify(Guid id, CancellationToken ct)
        {
            var entity = await LoadWithAttachmentsAsync(id, ct);
            if (entity == null) return NotFound();
            if (!await SetProjectHeaderAsync(entity.ProjectId, ct)) return NotFound();
            ViewBag.TeamMember = entity;
            return View(new VerifyTeamMemberVm
            {
                TeamMemberId      = entity.Id,
                VerificationStatus = entity.VerificationStatus,
                VerificationNotes  = entity.VerificationNotes,
                VerificationDate   = entity.VerificationDate ?? DateTime.Today,
                ActualHours        = entity.ActualHours,
                ActualDays         = entity.ActualDays ?? 0
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify(VerifyTeamMemberVm vm, CancellationToken ct)
        {
            var entity = await LoadWithAttachmentsAsync(vm.TeamMemberId, ct);
            if (entity == null) return NotFound();

            entity.VerificationStatus = vm.VerificationStatus;
            entity.VerificationNotes  = vm.VerificationNotes;
            entity.VerificationDate   = vm.VerificationDate;
            entity.VerifiedByUserId   = User.FindFirstValue(ClaimTypes.NameIdentifier);
            entity.ActualHours        = vm.ActualHours;
            entity.ActualDays         = vm.ActualDays;
            entity.UpdatedAtUtc       = DateTime.UtcNow;

            if (vm.NewAttachments?.Count > 0)
                foreach (var f in vm.NewAttachments)
                {
                    using var ms = new MemoryStream();
                    await f.CopyToAsync(ms, ct);
                    _db.Set<ProjectTeamMemberAttachment>().Add(new()
                    {
                        Id               = Guid.NewGuid(),
                        TeamMemberId     = entity.Id,
                        AttachmentType   = vm.AttachmentType ?? "Other",
                        OriginalFileName = f.FileName,
                        ContentType      = f.ContentType ?? "application/octet-stream",
                        FileSizeBytes    = f.Length,
                        FileContent      = ms.ToArray(),
                        UploadedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        CreatedAtUtc     = DateTime.UtcNow
                    });
                }

            await _db.SaveChangesAsync(ct);
            TempData["Success"] = "تم حفظ حالة الإثبات.";
            return RedirectToAction(nameof(Index), new { projectId = entity.ProjectId });
        }

        // ══ تقرير الفريق (JSON للطباعة/التصدير) ════════════════════════
        [HttpGet]
        public async Task<IActionResult> Report(Guid projectId, CancellationToken ct)
        {
            if (!await SetProjectHeaderAsync(projectId, ct)) return NotFound();

            var members = await _db.Set<ProjectTeamMember>()
                .AsNoTracking()
                .Include(x => x.Employee).ThenInclude(e => e!.JobTitle)
                .Include(x => x.Volunteer)
                .Include(x => x.Phase)
                .Include(x => x.Activity)
                .Where(x => x.ProjectId == projectId)
                .OrderBy(x => x.MemberType).ThenBy(x => x.StartDate)
                .ToListAsync(ct);

            // تجميع: المرحلة → الأعضاء
            var byPhase = members
                .GroupBy(m => m.Phase?.Name ?? "غير مسكَّن")
                .Select(g => new
                {
                    Phase       = g.Key,
                    Employees   = g.Where(m => m.MemberType == "Employee").ToList(),
                    Volunteers  = g.Where(m => m.MemberType == "Volunteer").ToList(),
                    TotalHours  = g.Sum(m => m.ActualHours),
                    TotalDays   = g.Sum(m => m.ActualDays ?? 0)
                }).ToList();

            ViewBag.ByPhase        = byPhase;
            ViewBag.TotalEmployees = members.Count(m => m.MemberType == "Employee");
            ViewBag.TotalVolunteers= members.Count(m => m.MemberType == "Volunteer");
            ViewBag.TotalHours     = members.Sum(m => m.ActualHours);
            ViewBag.TotalDays      = members.Sum(m => m.ActualDays ?? 0);
            return View(members);
        }

        // ══ AJAX: أنشطة المرحلة ════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> GetPhaseActivities(Guid phaseId, CancellationToken ct)
        {
            var acts = await _db.Set<ActivityPhaseAssignment>()
                .AsNoTracking()
                .Where(a => a.PhaseId == phaseId)
                .Include(a => a.Activity)
                .Select(a => new { value = a.Activity!.Id, text = a.Activity.Title })
                .ToListAsync(ct);
            return Json(acts);
        }

        // ══ Helpers ═════════════════════════════════════════════════════
        private async Task<bool> SetProjectHeaderAsync(Guid projectId, CancellationToken ct)
        {
            var project = await _db.Set<CharityProject>()
                .AsNoTracking().FirstOrDefaultAsync(x => x.Id == projectId, ct);
            if (project == null) return false;
            ViewBag.Project = project;
            return true;
        }

        private async Task<ProjectTeamMember?> LoadWithAttachmentsAsync(Guid id, CancellationToken ct)
            => await _db.Set<ProjectTeamMember>()
                .Include(x => x.Attachments)
                .Include(x => x.Employee)
                .Include(x => x.Volunteer)
                .Include(x => x.Phase)
                .Include(x => x.Activity)
                .FirstOrDefaultAsync(x => x.Id == id, ct);

        private async Task FillLookupsAsync(ProjectTeamMemberFormVm vm, CancellationToken ct)
        {
            vm.Employees = await _db.Set<HrEmployee>().AsNoTracking()
                .Where(e => e.IsActive)
                .OrderBy(e => e.FullName)
                .Select(e => new SelectListItem(e.Code + " - " + e.FullName, e.Id.ToString()))
                .ToListAsync(ct);

            vm.Volunteers = await _db.Set<Volunteer>().AsNoTracking()
                .Where(v => v.IsActive)
                .OrderBy(v => v.FullName)
                .Select(v => new SelectListItem(v.VolunteerCode + " - " + v.FullName, v.Id.ToString()))
                .ToListAsync(ct);

            vm.Phases = await _db.Set<ProjectPhase>().AsNoTracking()
                .Where(p => p.ProjectId == vm.ProjectId && p.IsActive)
                .OrderBy(p => p.SortOrder)
                .Select(p => new SelectListItem(p.Name, p.Id.ToString()))
                .ToListAsync(ct);
        }

        private static ProjectTeamMember BuildEntity(ProjectTeamMemberFormVm vm)
        {
            var e = new ProjectTeamMember { Id = Guid.NewGuid() };
            ApplyToEntity(vm, e);
            e.ProjectId = vm.ProjectId;
            return e;
        }

        private static void ApplyToEntity(ProjectTeamMemberFormVm vm, ProjectTeamMember e)
        {
            e.MemberType          = vm.MemberType;
            e.EmployeeId          = vm.MemberType == "Employee"  ? vm.EmployeeId  : null;
            e.VolunteerId         = vm.MemberType == "Volunteer" ? vm.VolunteerId : null;
            e.PhaseId             = vm.PhaseId;
            e.ActivityId          = vm.ActivityId;
            e.RoleTitle           = vm.RoleTitle;
            e.ParticipationStatus = vm.ParticipationStatus;
            e.StartDate           = vm.StartDate;
            e.EndDate             = vm.EndDate;
            e.PlannedHours        = vm.PlannedHours;
            e.ActualHours         = vm.ActualHours;
            e.PlannedDays         = vm.PlannedDays;
            e.ActualDays          = vm.ActualDays;
            e.Notes               = vm.Notes;
        }

        private async Task SaveWithAttachmentsAsync(
            ProjectTeamMember entity, ProjectTeamMemberFormVm vm, CancellationToken ct)
        {
            if (vm.NewAttachments?.Count > 0)
                foreach (var f in vm.NewAttachments)
                {
                    using var ms = new MemoryStream();
                    await f.CopyToAsync(ms, ct);
                    _db.Set<ProjectTeamMemberAttachment>().Add(new()
                    {
                        Id               = Guid.NewGuid(),
                        TeamMemberId     = entity.Id,
                        AttachmentType   = "Other",
                        OriginalFileName = f.FileName,
                        ContentType      = f.ContentType ?? "application/octet-stream",
                        FileSizeBytes    = f.Length,
                        FileContent      = ms.ToArray(),
                        CreatedAtUtc     = DateTime.UtcNow
                    });
                }
            await _db.SaveChangesAsync(ct);
        }

        private static ProjectTeamMemberFormVm MapToVm(ProjectTeamMember e) => new()
        {
            Id                  = e.Id,
            ProjectId           = e.ProjectId,
            MemberType          = e.MemberType,
            EmployeeId          = e.EmployeeId,
            VolunteerId         = e.VolunteerId,
            PhaseId             = e.PhaseId,
            ActivityId          = e.ActivityId,
            RoleTitle           = e.RoleTitle,
            ParticipationStatus = e.ParticipationStatus,
            StartDate           = e.StartDate,
            EndDate             = e.EndDate,
            PlannedHours        = e.PlannedHours,
            ActualHours         = e.ActualHours,
            PlannedDays         = e.PlannedDays,
            ActualDays          = e.ActualDays,
            Notes               = e.Notes
        };
    }

    // ══ VMs ════════════════════════════════════════════════════════════════
    public class ProjectTeamMemberFormVm
    {
        public Guid?   Id         { get; set; }
        public Guid    ProjectId  { get; set; }
        public string  MemberType { get; set; } = "Volunteer";
        public Guid?   EmployeeId  { get; set; }
        public Guid?   VolunteerId { get; set; }
        public Guid?   PhaseId    { get; set; }
        public Guid?   ActivityId { get; set; }
        public string? RoleTitle  { get; set; }
        public string  ParticipationStatus { get; set; } = "Assigned";
        public DateTime  StartDate    { get; set; } = DateTime.Today;
        public DateTime? EndDate      { get; set; }
        public decimal PlannedHours   { get; set; }
        public decimal ActualHours    { get; set; }
        public int?    PlannedDays    { get; set; }
        public int?    ActualDays     { get; set; }
        public string? Notes          { get; set; }
        public List<Microsoft.AspNetCore.Http.IFormFile>? NewAttachments { get; set; }
        public List<SelectListItem> Employees  { get; set; } = new();
        public List<SelectListItem> Volunteers { get; set; } = new();
        public List<SelectListItem> Phases     { get; set; } = new();
        public List<SelectListItem> Activities { get; set; } = new();
    }

    public class VerifyTeamMemberVm
    {
        public Guid   TeamMemberId      { get; set; }
        public string VerificationStatus { get; set; } = "Unverified";
        public string? VerificationNotes { get; set; }
        public DateTime VerificationDate { get; set; } = DateTime.Today;
        public decimal  ActualHours      { get; set; }
        public int      ActualDays       { get; set; }
        public string?  AttachmentType   { get; set; }
        public List<Microsoft.AspNetCore.Http.IFormFile>? NewAttachments { get; set; }
    }
}
