using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Skote.Controllers
{
    [Authorize]
    public class ActivityBeneficiaryAssignmentController : Controller
    {
        private readonly AppDbContext _db;
        public ActivityBeneficiaryAssignmentController(AppDbContext db) => _db = db;

        // ══ الشاشة الرئيسية: مراحل → أنشطة → فئات → أعداد ══════════════════
        public async Task<IActionResult> Index(Guid projectId, CancellationToken ct)
        {
            var project = await _db.Set<CharityProject>()
                .AsNoTracking().FirstOrDefaultAsync(x => x.Id == projectId, ct);
            if (project == null) return NotFound();

            // المراحل + أنشطتها
            var phases = await _db.Set<ProjectPhase>()
                .AsNoTracking()
                .Where(p => p.ProjectId == projectId && p.IsActive)
                .OrderBy(p => p.SortOrder).ToListAsync(ct);

            var phaseIds   = phases.Select(p => p.Id).ToList();
            var assignments = await _db.Set<ActivityPhaseAssignment>()
                .AsNoTracking()
                .Include(a => a.Activity)
                .Where(a => phaseIds.Contains(a.PhaseId))
                .OrderBy(a => a.SortOrder).ToListAsync(ct);

            var actIds = assignments.Select(a => a.ActivityId).Distinct().ToList();

            // الفئات المستهدفة من المقترح
            var targetGroups = await GetTargetGroupsAsync(project, ct);

            // الأعداد الحالية: نشاط × فئة → عدد
            var counts = await _db.Set<ProjectActivityBeneficiary>()
                .AsNoTracking()
                .Where(ab => actIds.Contains(ab.ActivityId))
                .GroupBy(ab => new { ab.ActivityId, ab.TargetGroupName, ab.ParticipationType })
                .Select(g => new {
                    g.Key.ActivityId,
                    g.Key.TargetGroupName,
                    g.Key.ParticipationType,
                    Count = g.Count()
                }).ToListAsync(ct);

            // الكمية المخططة لكل نشاط (من ActivityPhaseAssignment)
            var planned = assignments.GroupBy(a => a.ActivityId)
                .ToDictionary(g => g.Key, g => g.Sum(a => a.PlannedQuantity));

            // بناء الـ VM
            var rows = phases.Select(ph => new PhaseAssignmentRow
            {
                Phase = ph,
                Activities = assignments
                    .Where(a => a.PhaseId == ph.Id)
                    .Select(a => new ActivityAssignmentRow
                    {
                        Assignment    = a,
                        TargetGroups  = targetGroups,
                        PlannedTotal  = planned.TryGetValue(a.ActivityId, out var p) ? (int)p : 0,
                        GroupCounts   = targetGroups.Select(tg => new TargetGroupCount
                        {
                            GroupName     = tg,
                            Participants  = counts.FirstOrDefault(c =>
                                c.ActivityId == a.ActivityId &&
                                c.TargetGroupName == tg &&
                                c.ParticipationType == "Participant")?.Count ?? 0,
                            Beneficiaries = counts.FirstOrDefault(c =>
                                c.ActivityId == a.ActivityId &&
                                c.TargetGroupName == tg &&
                                c.ParticipationType == "Beneficiary")?.Count ?? 0
                        }).ToList()
                    }).ToList()
            }).ToList();

            ViewBag.Project      = project;
            ViewBag.TargetGroups = targetGroups;
            return View(rows);
        }

        // ══ شاشة التسكين: تسكين مستفيد في نشاط × فئة ══════════════════════
        [HttpGet]
        public async Task<IActionResult> Assign(
            Guid projectId, Guid activityId, Guid phaseId,
            string? targetGroup, CancellationToken ct)
        {
            var project = await _db.Set<CharityProject>()
                .AsNoTracking().FirstOrDefaultAsync(x => x.Id == projectId, ct);
            if (project == null) return NotFound();

            var activity = await _db.Set<ProjectSubGoalActivity>()
                .AsNoTracking().FirstOrDefaultAsync(x => x.Id == activityId, ct);
            var phase = await _db.Set<ProjectPhase>()
                .AsNoTracking().FirstOrDefaultAsync(x => x.Id == phaseId, ct);

            // المستفيدون المسكَّنون بالفعل في هذا النشاط
            var assigned = await _db.Set<ProjectActivityBeneficiary>()
                .AsNoTracking()
                .Include(ab => ab.Beneficiary).ThenInclude(pb => pb!.Beneficiary)
                .Where(ab => ab.ActivityId == activityId)
                .ToListAsync(ct);

            // الفئات المستهدفة
            var targetGroups = await GetTargetGroupsAsync(project, ct);

            // المستفيدون المسجّلون في المشروع (للإضافة)
            var projectBeneficiaries = await _db.Set<ProjectBeneficiary>()
                .AsNoTracking()
                .Include(pb => pb.Beneficiary)
                .Where(pb => pb.ProjectId == projectId)
                .OrderBy(pb => pb.Beneficiary!.FullName)
                .ToListAsync(ct);

            ViewBag.Project         = project;
            ViewBag.Activity        = activity;
            ViewBag.Phase           = phase;
            ViewBag.TargetGroups    = targetGroups;
            ViewBag.SelectedGroup   = targetGroup;
            ViewBag.Assigned        = assigned;

            var vm = new AssignBeneficiaryVm
            {
                ProjectId    = projectId,
                ActivityId   = activityId,
                PhaseId      = phaseId,
                TargetGroupName = targetGroup ?? string.Empty,
                ParticipationType = "Beneficiary",
                Beneficiaries = projectBeneficiaries
                    .Where(pb => !assigned.Any(a => a.BeneficiaryId == pb.Id))
                    .Select(pb => new SelectListItem(
                        $"{pb.Beneficiary?.Code} - {pb.Beneficiary?.FullName}",
                        pb.Id.ToString()
                    )).ToList(),
                TargetGroupList = targetGroups
                    .Select(tg => new SelectListItem(tg, tg)).ToList()
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(AssignBeneficiaryVm vm, CancellationToken ct)
        {
            // منع التكرار
            var exists = await _db.Set<ProjectActivityBeneficiary>()
                .AnyAsync(ab => ab.ActivityId == vm.ActivityId
                             && ab.BeneficiaryId == vm.BeneficiaryId, ct);
            if (exists)
            {
                TempData["Warning"] = "هذا المستفيد مسكَّن بالفعل في هذا النشاط.";
                return RedirectToAction(nameof(Assign), new
                {
                    projectId   = vm.ProjectId,
                    activityId  = vm.ActivityId,
                    phaseId     = vm.PhaseId,
                    targetGroup = vm.TargetGroupName
                });
            }

            _db.Set<ProjectActivityBeneficiary>().Add(new()
            {
                Id                = Guid.NewGuid(),
                ProjectId         = vm.ProjectId,
                PhaseId           = vm.PhaseId,
                ActivityId        = vm.ActivityId,
                BeneficiaryId     = vm.BeneficiaryId,
                TargetGroupName   = vm.TargetGroupName,
                ParticipationType = vm.ParticipationType,
                VerificationStatus= "Unverified",
                Notes             = vm.Notes,
                CreatedAtUtc      = DateTime.UtcNow
            });
            await _db.SaveChangesAsync(ct);

            TempData["Success"] = "تم تسكين المستفيد في النشاط.";
            return RedirectToAction(nameof(Assign), new
            {
                projectId   = vm.ProjectId,
                activityId  = vm.ActivityId,
                phaseId     = vm.PhaseId,
                targetGroup = vm.TargetGroupName
            });
        }

        // ══ حذف تسكين ══════════════════════════════════════════════════════
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(
            Guid id, Guid projectId, Guid activityId,
            Guid phaseId, string? targetGroup, CancellationToken ct)
        {
            var ab = await _db.Set<ProjectActivityBeneficiary>().FindAsync(new object[]{id}, ct);
            if (ab != null) { _db.Set<ProjectActivityBeneficiary>().Remove(ab); await _db.SaveChangesAsync(ct); }
            TempData["Success"] = "تم إلغاء التسكين.";
            return RedirectToAction(nameof(Assign), new { projectId, activityId, phaseId, targetGroup });
        }

        // ══ AJAX: مستفيدو نشاط معين مع فلتر فئة ══════════════════════════
        [HttpGet]
        public async Task<IActionResult> GetActivityBeneficiaries(
            Guid activityId, string? targetGroup, CancellationToken ct)
        {
            var q = _db.Set<ProjectActivityBeneficiary>()
                .AsNoTracking()
                .Include(ab => ab.Beneficiary).ThenInclude(pb => pb!.Beneficiary)
                .Where(ab => ab.ActivityId == activityId);

            if (!string.IsNullOrEmpty(targetGroup))
                q = q.Where(ab => ab.TargetGroupName == targetGroup);

            var items = await q.Select(ab => new {
                id               = ab.Id,
                name             = ab.Beneficiary!.Beneficiary!.FullName,
                code             = ab.Beneficiary.Beneficiary.Code,
                targetGroup      = ab.TargetGroupName,
                participationType= ab.ParticipationType,
                verificationStatus= ab.VerificationStatus
            }).ToListAsync(ct);

            return Json(items);
        }

        // ══ Helper ════════════════════════════════════════════════════════
        private async Task<List<string>> GetTargetGroupsAsync(
            CharityProject project, CancellationToken ct)
        {
            // من المقترح المرتبط بالمشروع
            var proposal = await _db.Set<ProjectProposal>()
                .AsNoTracking()
                .Include(p => p.TargetGroups)
                .FirstOrDefaultAsync(p => p.ProposalNumber == project.Code, ct);

            if (proposal?.TargetGroups.Any() == true)
                return proposal.TargetGroups
                    .Where(t => !string.IsNullOrWhiteSpace(t.CategoryName))
                    .Select(t => t.CategoryName!).Distinct().ToList();

            // fallback: من الأنشطة مباشرة
            return await _db.Set<ProjectSubGoalActivity>()
                .AsNoTracking()
                .Where(a => a.ProjectId == project.Id && !string.IsNullOrEmpty(a.TargetGroup))
                .Select(a => a.TargetGroup!).Distinct().ToListAsync(ct);
        }
    }

    // ══ VMs ════════════════════════════════════════════════════════════════
    public class PhaseAssignmentRow
    {
        public ProjectPhase Phase { get; set; } = null!;
        public List<ActivityAssignmentRow> Activities { get; set; } = new();
        public int TotalAssigned => Activities.Sum(a => a.TotalAssigned);
    }

    public class ActivityAssignmentRow
    {
        public ActivityPhaseAssignment Assignment { get; set; } = null!;
        public int PlannedTotal   { get; set; }
        public List<string> TargetGroups { get; set; } = new();
        public List<TargetGroupCount> GroupCounts { get; set; } = new();
        public int TotalAssigned => GroupCounts.Sum(g => g.Total);
        public decimal FillPercent => PlannedTotal > 0
            ? Math.Min((decimal)TotalAssigned / PlannedTotal * 100, 100) : 0;
    }

    public class TargetGroupCount
    {
        public string GroupName     { get; set; } = string.Empty;
        public int    Participants  { get; set; }
        public int    Beneficiaries { get; set; }
        public int    Total         => Participants + Beneficiaries;
    }

    public class AssignBeneficiaryVm
    {
        public Guid    ProjectId         { get; set; }
        public Guid    ActivityId        { get; set; }
        public Guid    PhaseId           { get; set; }
        public Guid    BeneficiaryId     { get; set; }
        public string  TargetGroupName   { get; set; } = string.Empty;
        public string  ParticipationType { get; set; } = "Beneficiary";
        public string? Notes             { get; set; }
        public List<SelectListItem> Beneficiaries   { get; set; } = new();
        public List<SelectListItem> TargetGroupList { get; set; } = new();
    }
}
