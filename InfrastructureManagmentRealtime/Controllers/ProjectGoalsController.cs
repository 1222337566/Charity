using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    [Authorize]
    public class ProjectGoalsController : Controller
    {
        private readonly AppDbContext _db;
        public ProjectGoalsController(AppDbContext db) => _db = db;

        // ── صفحة الأهداف والهيكل الشجري ──
        public async Task<IActionResult> Index(Guid projectId, CancellationToken ct)
        {
            var project = await _db.Set<CharityProject>()
                .AsNoTracking().FirstOrDefaultAsync(x => x.Id == projectId, ct);
            if (project == null) return NotFound();

            var goals = await _db.Set<ProjectGoal>()
                .AsNoTracking()
                .Include(g => g.SubGoals)
                    .ThenInclude(s => s.Activities)
                        
                .Where(g => g.ProjectId == projectId && g.IsActive)
                .OrderBy(g => g.SortOrder)
                .ToListAsync(ct);

            var phases = await _db.Set<ProjectPhase>()
                .AsNoTracking()
                .Where(p => p.ProjectId == projectId && p.IsActive)
                .OrderBy(p => p.SortOrder)
                .ToListAsync(ct);

            ViewBag.Project = project;
            ViewBag.Phases  = phases;
            return View(goals);
        }

        // ── إضافة هدف رئيسي ──
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGoal(Guid projectId, string title,
            string? description, string? successIndicator, string? targetValue, int sortOrder)
        {
            _db.Set<ProjectGoal>().Add(new ProjectGoal
            {
                Id                = Guid.NewGuid(),
                ProjectId         = projectId,
                Title             = title.Trim(),
                Description       = description?.Trim(),
                SuccessIndicator  = successIndicator?.Trim(),
                TargetValue       = targetValue?.Trim(),
                SortOrder         = sortOrder > 0 ? sortOrder : 99,
                Status            = "Active",
                IsActive          = true,
                CreatedAtUtc      = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
            TempData["Success"] = $"تم إضافة الهدف الرئيسي «{title}»";
            return RedirectToAction(nameof(Index), new { projectId });
        }

        // ── إضافة هدف فرعي ──
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSubGoal(Guid projectId, Guid goalId,
            string title, string? description, string? successIndicator,
            string? targetValue, int sortOrder)
        {
            var goal = await _db.Set<ProjectGoal>().FindAsync(goalId);
            if (goal == null) return NotFound();

            _db.Set<ProjectSubGoal>().Add(new ProjectSubGoal
            {
                Id               = Guid.NewGuid(),
                ProjectId        = projectId,
                GoalId           = goalId,
                Title            = title.Trim(),
                Description      = description?.Trim(),
                SuccessIndicator = successIndicator?.Trim(),
                TargetValue      = targetValue?.Trim(),
                SortOrder        = sortOrder > 0 ? sortOrder : 99,
                Status           = "Active",
                IsActive         = true,
                CreatedAtUtc     = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
            TempData["Success"] = $"تم إضافة الهدف الفرعي «{title}»";
            return RedirectToAction(nameof(Index), new { projectId });
        }

        // ── إضافة نشاط لهدف فرعي ──
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddActivity(Guid projectId, Guid subGoalId,
            string title, string? description, Guid? phaseId,
            DateTime? plannedStartDate, DateTime? plannedEndDate,
            string? responsiblePersonName, string priority,
            decimal plannedCost, decimal plannedHours, int sortOrder)
        {
            _db.Set<ProjectSubGoalActivity>().Add(new ProjectSubGoalActivity
            {
                Id                   = Guid.NewGuid(),
                ProjectId            = projectId,
                SubGoalId            = subGoalId,
           
                Title                = title.Trim(),
                Description          = description?.Trim(),
                ResponsiblePersonName = responsiblePersonName?.Trim(),
                Priority             = priority,
                PlannedStartDate     = plannedStartDate,
                PlannedEndDate       = plannedEndDate,
                PlannedCost          = plannedCost,
                PlannedHours         = plannedHours,
                SortOrder            = sortOrder > 0 ? sortOrder : 99,
                Status               = "Planned",
                IsActive             = true,
                CreatedAtUtc         = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
            TempData["Success"] = $"تم إضافة النشاط «{title}»";
            return RedirectToAction(nameof(Index), new { projectId });
        }

        // ── تحديث تقدم نشاط ──
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateActivityProgress(
            Guid id, decimal progress, string status,
            decimal actualHours, decimal actualCost, string? notes)
        {
            var act = await _db.Set<ProjectSubGoalActivity>().FindAsync(id);
            if (act == null) return NotFound();

            act.ProgressPercent = Math.Min(progress, 100);
            act.Status          = status;
            act.ActualHours     = actualHours;
            act.ActualCost      = actualCost;
            act.Notes           = notes;
            act.UpdatedAtUtc    = DateTime.UtcNow;
            if (status == "Completed") act.ActualEndDate = DateTime.Today;
            if (status == "InProgress" && act.ActualStartDate == null)
                act.ActualStartDate = DateTime.Today;

            // حدّث تقدم الهدف الفرعي
            await RecalcSubGoalProgressAsync(act.SubGoalId);
            await _db.SaveChangesAsync();

            TempData["Success"] = "تم تحديث التقدم";
            return RedirectToAction(nameof(Index), new { projectId = act.ProjectId });
        }

        // ── حذف / تعطيل ──
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGoal(Guid id, Guid projectId)
        {
            var e = await _db.Set<ProjectGoal>().FindAsync(id);
            if (e != null) { e.IsActive = false; await _db.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index), new { projectId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSubGoal(Guid id, Guid projectId)
        {
            var e = await _db.Set<ProjectSubGoal>().FindAsync(id);
            if (e != null) { e.IsActive = false; await _db.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index), new { projectId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteActivity(Guid id, Guid projectId)
        {
            var e = await _db.Set<ProjectSubGoalActivity>().FindAsync(id);
            if (e != null) { e.IsActive = false; await _db.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index), new { projectId });
        }

        private async Task RecalcSubGoalProgressAsync(Guid subGoalId)
        {
            var acts = await _db.Set<ProjectSubGoalActivity>()
                .Where(a => a.SubGoalId == subGoalId && a.IsActive).ToListAsync();
            var sg = await _db.Set<ProjectSubGoal>().FindAsync(subGoalId);
            if (sg != null && acts.Any())
                sg.ProgressPercent = acts.Average(a => a.ProgressPercent);
        }
    }
}
