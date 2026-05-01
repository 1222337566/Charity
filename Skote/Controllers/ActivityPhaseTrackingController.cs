using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    [Authorize]
    public class ActivityPhaseTrackingController : Controller
    {
        private readonly AppDbContext _db;
        public ActivityPhaseTrackingController(AppDbContext db) => _db = db;

        // ── صفحة التتبع: نشاط واحد عبر مراحله ──
        public async Task<IActionResult> Index(Guid activityId, CancellationToken ct)
        {
            var activity = await _db.Set<ProjectSubGoalActivity>()
                .AsNoTracking()
                .Include(x => x.SubGoal).ThenInclude(s => s!.Goal)
                .Include(x => x.PhaseAssignments).ThenInclude(pa => pa.Phase)
                .FirstOrDefaultAsync(x => x.Id == activityId, ct);

            if (activity == null) return NotFound();

            var project = await _db.Set<CharityProject>()
                .AsNoTracking().FirstOrDefaultAsync(x => x.Id == activity.ProjectId, ct);

            ViewBag.Project  = project;
            ViewBag.Activity = activity;
            return View(activity.PhaseAssignments.OrderBy(p => p.SortOrder).ToList());
        }

        // ── تحديث تقدم مرحلة نشاط ──
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePhase(
            Guid assignmentId, Guid activityId,
            decimal progress, string status,
            decimal actualQuantity, decimal actualCost,
            int? actualDurationDays, DateTime? actualStartDate,
            DateTime? actualEndDate, string? notes, CancellationToken ct)
        {
            var pa = await _db.Set<ActivityPhaseAssignment>().FindAsync(new object[]{assignmentId}, ct);
            if (pa == null) return NotFound();

            pa.ProgressPercent    = Math.Min(progress, 100);
            pa.Status             = status;
            pa.ActualQuantity     = actualQuantity;
            pa.ActualCost         = actualCost;
            pa.ActualDurationDays = actualDurationDays;
            if (actualStartDate.HasValue) pa.ActualStartDate = actualStartDate;
            if (actualEndDate.HasValue)   pa.ActualEndDate   = actualEndDate;
            if (status == "InProgress" && pa.ActualStartDate == null)
                pa.ActualStartDate = DateTime.Today;
            if (status == "Completed") pa.ActualEndDate = DateTime.Today;
            pa.Notes = notes;
            await _db.SaveChangesAsync(ct);

            // أعد حساب تقدم النشاط الكلي
            var allPa = await _db.Set<ActivityPhaseAssignment>()
                .Where(x => x.ActivityId == activityId).ToListAsync(ct);
            var act = await _db.Set<ProjectSubGoalActivity>()
                .FindAsync(new object[]{activityId}, ct);
            if (act != null && allPa.Any())
            {
                act.ProgressPercent = Math.Min(
                    allPa.Sum(p => p.ProgressPercent * p.ContributionPercent / 100m), 100m);
                act.ActualQuantity  = (int)allPa.Sum(p => p.ActualQuantity);
                act.ActualCost      = allPa.Sum(p => p.ActualCost);
                await _db.SaveChangesAsync(ct);
            }

            TempData["Success"] = $"تم التحديث — إجمالي الإنجاز: {act?.ProgressPercent:N1}%";
            return RedirectToAction(nameof(Index), new { activityId });
        }

        // ══════════════════════════════════════════════════════════════════════
        // ── شاشة متابعة المراحل والأنشطة الشاملة ──
        // ══════════════════════════════════════════════════════════════════════
        public async Task<IActionResult> PhaseOverview(Guid projectId, CancellationToken ct)
        {
            var project = await _db.Set<CharityProject>()
                .AsNoTracking().FirstOrDefaultAsync(x => x.Id == projectId, ct);
            if (project == null) return NotFound();

            // المراحل
            var phases = await _db.Set<ProjectPhase>()
                .AsNoTracking()
                .Where(x => x.ProjectId == projectId && x.IsActive)
                .OrderBy(x => x.SortOrder).ToListAsync(ct);

            var phaseIds = phases.Select(p => p.Id).ToList();

            // الأنشطة المسكّنة في المراحل (مع بياناتها)
            var assignments = await _db.Set<ActivityPhaseAssignment>()
                .AsNoTracking()
                .Include(pa => pa.Activity)
                    .ThenInclude(a => a!.SubGoal)
                        .ThenInclude(s => s!.Goal)
                .Include(pa => pa.Phase)
                .Where(pa => phaseIds.Contains(pa.PhaseId))
                .ToListAsync(ct);

            var activityIds = assignments.Select(a => a.ActivityId).Distinct().ToList();

            // عدد المستفيدين لكل نشاط
            var beneficiaryCounts = await _db.Set<ProjectActivityBeneficiary>()
                .AsNoTracking()
                .Where(ab => activityIds.Contains(ab.ActivityId))
                .GroupBy(ab => new { ab.ActivityId, ab.ParticipationType })
                .Select(g => new { g.Key.ActivityId, g.Key.ParticipationType, Count = g.Count() })
                .ToListAsync(ct);

            // عدد الإثباتات لكل نشاط
            var verifiedCounts = await _db.Set<ProjectActivityBeneficiary>()
                .AsNoTracking()
                .Where(ab => activityIds.Contains(ab.ActivityId)
                          && ab.VerificationStatus == "Verified")
                .GroupBy(ab => ab.ActivityId)
                .Select(g => new { ActivityId = g.Key, Count = g.Count() })
                .ToListAsync(ct);

            // عدد المرفقات لكل نشاط
            var attachmentCounts = await _db.Set<ProjectActivityBeneficiaryAttachment>()
                .AsNoTracking()
                .Where(a => _db.Set<ProjectActivityBeneficiary>()
                    .Where(ab => activityIds.Contains(ab.ActivityId))
                    .Select(ab => ab.Id)
                    .Contains(a.ActivityBeneficiaryId))
                .GroupBy(a => _db.Set<ProjectActivityBeneficiary>()
                    .Where(ab => ab.Id == a.ActivityBeneficiaryId)
                    .Select(ab => ab.ActivityId).First())
                .Select(g => new { ActivityId = g.Key, Count = g.Count() })
                .ToListAsync(ct);

            // تجميع البيانات
            var byPhase = phases.Select(phase =>
            {
                var phaseAssignments = assignments
                    .Where(pa => pa.PhaseId == phase.Id)
                    .OrderBy(pa => pa.SortOrder).ToList();

                var actRows = phaseAssignments.Select(pa =>
                {
                    var actId = pa.ActivityId;
                    var participants = beneficiaryCounts
                        .FirstOrDefault(b => b.ActivityId == actId && b.ParticipationType == "Participant")?.Count ?? 0;
                    var beneficiaries = beneficiaryCounts
                        .FirstOrDefault(b => b.ActivityId == actId && b.ParticipationType == "Beneficiary")?.Count ?? 0;
                    var verified = verifiedCounts.FirstOrDefault(v => v.ActivityId == actId)?.Count ?? 0;
                    var attachments = attachmentCounts.FirstOrDefault(a => a.ActivityId == actId)?.Count ?? 0;

                    return new ActivityTrackingRow
                    {
                        Assignment    = pa,
                        Participants  = participants,
                        Beneficiaries = beneficiaries,
                        Verified      = verified,
                        Attachments   = attachments
                    };
                }).ToList();

                return new PhaseTrackingRow
                {
                    Phase       = phase,
                    ActivityRows = actRows
                };
            }).ToList();

            // إجمالي المشروع
            var allAssignments = assignments.ToList();
            decimal projectProgress = allAssignments.Any()
                ? Math.Min(allAssignments.Average(a => a.ProgressPercent), 100) : 0;

            ViewBag.Project         = project;
            ViewBag.ProjectProgress = projectProgress;
            ViewBag.TotalActivities = activityIds.Count;
            ViewBag.CompletedActivities = allAssignments
                .GroupBy(a => a.ActivityId)
                .Count(g => g.All(a => a.Status == "Completed"));
            ViewBag.TotalParticipants = beneficiaryCounts
                .Where(b => b.ParticipationType == "Participant").Sum(b => b.Count);
            ViewBag.TotalBeneficiaries = beneficiaryCounts
                .Where(b => b.ParticipationType == "Beneficiary").Sum(b => b.Count);

            return View(byPhase);
        }

        // ── AJAX: تحديث سريع لنسبة التنفيذ ──
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickUpdate(
            Guid assignmentId, decimal progress, string status, CancellationToken ct)
        {
            var pa = await _db.Set<ActivityPhaseAssignment>().FindAsync(new object[]{assignmentId}, ct);
            if (pa == null) return Json(new { ok = false });

            pa.ProgressPercent = Math.Min(progress, 100);
            pa.Status          = status;
            if (status == "InProgress" && pa.ActualStartDate == null)
                pa.ActualStartDate = DateTime.Today;
            if (status == "Completed")
            {
                pa.ActualEndDate   = DateTime.Today;
                pa.ProgressPercent = 100;
            }
            await _db.SaveChangesAsync(ct);

            // أعد حساب تقدم النشاط
            var allPa = await _db.Set<ActivityPhaseAssignment>()
                .Where(x => x.ActivityId == pa.ActivityId).ToListAsync(ct);
            var act = await _db.Set<ProjectSubGoalActivity>()
                .FindAsync(new object[]{pa.ActivityId}, ct);
            decimal actProgress = 0;
            if (act != null && allPa.Any())
            {
                actProgress = Math.Min(allPa.Sum(p => p.ProgressPercent * p.ContributionPercent / 100m), 100m);
                act.ProgressPercent = actProgress;
                await _db.SaveChangesAsync(ct);
            }

            return Json(new { ok = true, assignProgress = pa.ProgressPercent, actProgress });
        }
    }

    // ── VMs ──────────────────────────────────────────────────────────────────
    public class PhaseTrackingRow
    {
        public ProjectPhase Phase { get; set; } = null!;
        public List<ActivityTrackingRow> ActivityRows { get; set; } = new();

        public decimal OverallProgress => ActivityRows.Any()
            ? ActivityRows.Average(r => r.Assignment.ProgressPercent) : 0;
        public decimal TotalPlannedQty  => ActivityRows.Sum(r => r.Assignment.PlannedQuantity);
        public decimal TotalActualQty   => ActivityRows.Sum(r => r.Assignment.ActualQuantity);
        public decimal TotalPlannedCost => ActivityRows.Sum(r => r.Assignment.PlannedCost);
        public decimal TotalActualCost  => ActivityRows.Sum(r => r.Assignment.ActualCost);
        public int TotalParticipants    => ActivityRows.Sum(r => r.Participants);
        public int TotalBeneficiaries   => ActivityRows.Sum(r => r.Beneficiaries);
    }

    public class ActivityTrackingRow
    {
        public ActivityPhaseAssignment Assignment { get; set; } = null!;
        public int Participants  { get; set; }
        public int Beneficiaries { get; set; }
        public int Verified      { get; set; }
        public int Attachments   { get; set; }

        public int TotalBeneficiaries => Participants + Beneficiaries;
    }

    // للتوافق مع الكود القديم
    public class PhaseOverviewRow
    {
        public ProjectPhase Phase { get; set; } = null!;
        public List<ActivityPhaseAssignment> Assignments { get; set; } = new();
        public decimal OverallProgress => Assignments.Any()
            ? Assignments.Average(a => a.ProgressPercent) : 0;
        public decimal TotalPlannedQty  => Assignments.Sum(a => a.PlannedQuantity);
        public decimal TotalActualQty   => Assignments.Sum(a => a.ActualQuantity);
        public decimal TotalPlannedCost => Assignments.Sum(a => a.PlannedCost);
        public decimal TotalActualCost  => Assignments.Sum(a => a.ActualCost);
    }
}
