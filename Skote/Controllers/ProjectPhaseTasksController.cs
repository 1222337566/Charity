using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Tasks;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectPlanning.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    public class ProjectPhaseTasksController : Controller
    {
        private readonly IProjectPhaseTaskRepository _repository;
        private readonly AppDbContext _db;
        public ProjectPhaseTasksController(IProjectPhaseTaskRepository repository, AppDbContext db)
        {
            _repository = repository;
            _db = db;
        }

        public async Task<IActionResult> Index(Guid? phaseId, Guid? activityId)
        {
            ViewBag.PhaseId = phaseId;
            ViewBag.ActivityId = activityId;
            ViewBag.CurrentPhaseId = phaseId;

            if (activityId.HasValue && activityId.Value != Guid.Empty)
            {
                var activity = await _db.Set<ProjectPhaseActivity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == activityId.Value);
                if (activity == null) return NotFound();

                var phase = await _db.Set<ProjectPhase>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == activity.PhaseId);
                if (phase != null)
                {
                    phaseId = phase.Id;
                    ViewBag.PhaseId = phase.Id;
                    ViewBag.CurrentPhaseId = phase.Id;
                    ViewBag.ProjectId = phase.ProjectId;
                    await LoadProjectHeaderAsync(phase.ProjectId);
                }

                return View(await _repository.GetByActivityIdAsync(activityId.Value));
            }

            if (phaseId.HasValue && phaseId.Value != Guid.Empty)
            {
                var phase = await _db.Set<ProjectPhase>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == phaseId.Value);
                if (phase == null) return NotFound();
                await LoadProjectHeaderAsync(phase.ProjectId);
                ViewBag.ProjectId = phase.ProjectId;
                return View(await _repository.GetByPhaseIdAsync(phaseId.Value));
            }

            ViewBag.ProjectId = null;
            var items = await _db.Set<ProjectPhaseTask>()
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAtUtc)
                .ThenBy(x => x.Title)
                .Select(x => new ProjectPhaseTaskListItemVm
                {
                    Id = x.Id,
                    ProjectId = x.ProjectId,
                    PhaseId = x.PhaseId,
                    ActivityId = x.ActivityId,
                    Code = x.Code,
                    Title = x.Title,
                    Status = x.Status,
                    Priority = x.Priority,
                    PercentComplete = x.PercentComplete,
                    EstimatedHours = x.EstimatedHours,
                    SpentHours = x.SpentHours,
                    AssignedToName = x.AssignedToName,
                    DueDate = x.DueDate,
                    LastDailyUpdateAtUtc = x.LastDailyUpdateAtUtc
                })
                .ToListAsync();

            return View(items);
        }

        public IActionResult Create(Guid projectId, Guid phaseId, Guid activityId)
            => View(new CreateProjectPhaseTaskVm { ProjectId = projectId, PhaseId = phaseId, ActivityId = activityId });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProjectPhaseTaskVm vm)
        {
            if (!ModelState.IsValid) return View(vm);
            await _repository.AddAsync(new ProjectPhaseTask
            {
                Id = Guid.NewGuid(),
                ProjectId = vm.ProjectId,
                PhaseId = vm.PhaseId,
                ActivityId = vm.ActivityId,
                Code = vm.Code ?? string.Empty,
                Title = vm.Title,
                Description = vm.Description,
                SortOrder = vm.SortOrder,
                Status = vm.Status,
                Priority = vm.Priority,
                PlannedStartDate = vm.PlannedStartDate,
                DueDate = vm.DueDate,
                PercentComplete = vm.PercentComplete,
                EstimatedHours = vm.EstimatedHours,
                SpentHours = vm.SpentHours,
                AssignedToName = vm.AssignedToName,
                AssignedToUserId = vm.AssignedToUserId,
                RequiresDailyFollowUp = vm.RequiresDailyFollowUp,
                Notes = vm.Notes,
                IsActive = vm.IsActive
            });
            return RedirectToAction(nameof(Index), new { phaseId = vm.PhaseId, activityId = vm.ActivityId });
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return View(new EditProjectPhaseTaskVm
            {
                Id = entity.Id,
                ProjectId = entity.ProjectId,
                PhaseId = entity.PhaseId,
                ActivityId = entity.ActivityId,
                Code = entity.Code,
                Title = entity.Title,
                Description = entity.Description,
                SortOrder = entity.SortOrder,
                Status = entity.Status,
                Priority = entity.Priority,
                PlannedStartDate = entity.PlannedStartDate,
                DueDate = entity.DueDate,
                PercentComplete = entity.PercentComplete,
                EstimatedHours = entity.EstimatedHours,
                SpentHours = entity.SpentHours,
                AssignedToName = entity.AssignedToName,
                AssignedToUserId = entity.AssignedToUserId,
                RequiresDailyFollowUp = entity.RequiresDailyFollowUp,
                Notes = entity.Notes,
                IsActive = entity.IsActive
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProjectPhaseTaskVm vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var entity = await _repository.GetByIdAsync(vm.Id);
            if (entity == null) return NotFound();
            entity.Code = vm.Code ?? string.Empty;
            entity.Title = vm.Title;
            entity.Description = vm.Description;
            entity.SortOrder = vm.SortOrder;
            entity.Status = vm.Status;
            entity.Priority = vm.Priority;
            entity.PlannedStartDate = vm.PlannedStartDate;
            entity.DueDate = vm.DueDate;
            entity.PercentComplete = vm.PercentComplete;
            entity.EstimatedHours = vm.EstimatedHours;
            entity.SpentHours = vm.SpentHours;
            entity.AssignedToName = vm.AssignedToName;
            entity.AssignedToUserId = vm.AssignedToUserId;
            entity.RequiresDailyFollowUp = vm.RequiresDailyFollowUp;
            entity.Notes = vm.Notes;
            entity.IsActive = vm.IsActive;
            if (vm.Status == "InProgress" && entity.StartedAtUtc == null) entity.StartedAtUtc = DateTime.UtcNow;
            if (vm.Status == "Done" && entity.CompletedAtUtc == null) entity.CompletedAtUtc = DateTime.UtcNow;
            if (vm.Status != "Done") entity.CompletedAtUtc = null;
            await _repository.UpdateAsync(entity);
            return RedirectToAction(nameof(Index), new { phaseId = entity.PhaseId, activityId = entity.ActivityId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(Guid id, string status)
        {
            var task = await _db.Set<ProjectPhaseTask>().FindAsync(id);
            if (task == null) return NotFound();
            task.Status = status;
            task.UpdatedAtUtc = DateTime.UtcNow;
            if (status == "InProgress" && task.StartedAtUtc == null)
                task.StartedAtUtc = DateTime.UtcNow;
            if (status == "Done")
            {
                task.CompletedAtUtc = DateTime.UtcNow;
                task.PercentComplete = 100;
            }
            await _db.SaveChangesAsync();
            return Ok();
        }

        // ── Kanban per ActivityPhaseAssignment ──
        public async Task<IActionResult> ActivityKanban(
            Guid assignmentId, CancellationToken ct)
        {
            var pa = await _db.Set<ActivityPhaseAssignment>()
                .AsNoTracking()
                .Include(x => x.Phase)
                .Include(x => x.Activity)
                    .ThenInclude(a => a!.SubGoal)
                        .ThenInclude(s => s!.Goal)
                .FirstOrDefaultAsync(x => x.Id == assignmentId, ct);
            if (pa == null) return NotFound();

            var project = await _db.Set<CharityProject>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == pa.Activity!.ProjectId, ct);

            // جيب المهام المرتبطة بهذه المرحلة — نستخدم PhaseId + نفلتر بـ Notes = assignmentId
            var tasks = await _db.Set<ProjectPhaseTask>()
                .AsNoTracking()
                .Where(t => t.PhaseId == pa.PhaseId && t.IsActive
                         && t.Notes == assignmentId.ToString()) // ربط بـ assignmentId
                .OrderBy(t => t.SortOrder).ThenBy(t => t.CreatedAtUtc)
                .ToListAsync(ct);

            var columns = new[] { "Todo","InProgress","Blocked","Done" };
            var kanban = columns.Select(col => new {
                Status = col,
                StatusAr = col switch {
                    "Todo"=>"قائمة الانتظار","InProgress"=>"جارٍ التنفيذ",
                    "Blocked"=>"موقوف","Done"=>"مكتمل",_=>col
                },
                Color = col switch {
                    "Todo"=>"#adb5bd","InProgress"=>"#556ee6",
                    "Blocked"=>"#f46a6a","Done"=>"#34c38f",_=>"#adb5bd"
                },
                Tasks = tasks.Where(t => t.Status == col).ToList()
            }).ToList();

            ViewBag.Assignment = pa;
            ViewBag.Project    = project;
            ViewBag.Kanban     = kanban;
            ViewBag.AllTasks   = tasks;
            return View(tasks);
        }

        // ── إضافة مهمة من الكانبان ──
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickCreateTask(
            Guid assignmentId, string title, string priority,
            string? assignedToName, DateTime? dueDate, CancellationToken ct)
        {
            var pa = await _db.Set<ActivityPhaseAssignment>()
                .Include(x => x.Activity)
                .FirstOrDefaultAsync(x => x.Id == assignmentId, ct);
            if (pa == null) return NotFound();

            var projectId = pa.Activity!.ProjectId;

            // جيب أو أنشئ ProjectPhaseActivity صالحة للمرحلة
            // (ProjectPhaseTask يتطلب FK صالح لـ CharityProjectPhaseActivities)
            var phaseAct = await _db.Set<ProjectPhaseActivity>()
                .Where(x => x.PhaseId == pa.PhaseId && x.IsActive)
                .OrderBy(x => x.CreatedAtUtc)
                .FirstOrDefaultAsync(ct);

            if (phaseAct == null)
            {
                // أنشئ نشاط placeholder للمرحلة
                phaseAct = new ProjectPhaseActivity
                {
                    Id            = Guid.NewGuid(),
                    ProjectId     = projectId,
                    PhaseId       = pa.PhaseId,
                    Code          = $"ACT-{pa.PhaseId:N}"[..16],
                    Title         = pa.Phase?.Name ?? "مهام المرحلة",
                    SortOrder     = 1,
                    PlannedStartDate = pa.PlannedStartDate ?? DateTime.Today,
                    PlannedEndDate   = pa.PlannedEndDate   ?? DateTime.Today.AddMonths(1),
                    Status        = "Planned",
                    Priority      = "Medium",
                    IsActive      = true
                };
                _db.Set<ProjectPhaseActivity>().Add(phaseAct);
                await _db.SaveChangesAsync(ct);
            }

            var code = $"TSK-{DateTime.UtcNow:yyMMddHHmmss}";
            var task = new ProjectPhaseTask
            {
                Id             = Guid.NewGuid(),
                ProjectId      = projectId,
                PhaseId        = pa.PhaseId,
                ActivityId     = phaseAct.Id,   // ← FK صالح لـ CharityProjectPhaseActivities
                Code           = code,
                Title          = title.Trim(),
                Priority       = priority,
                AssignedToName = assignedToName?.Trim(),
                DueDate        = dueDate,
                Status         = "Todo",
                RequiresDailyFollowUp = true,
                Notes          = assignmentId.ToString(), // ← ربط بـ ActivityPhaseAssignment
                IsActive       = true
            };
            _db.Set<ProjectPhaseTask>().Add(task);
            await _db.SaveChangesAsync(ct);

            return RedirectToAction(nameof(ActivityKanban), new { assignmentId });
        }

        // ── تحريك مهمة (AJAX drag-drop) ──
        [HttpPost]
        public async Task<IActionResult> MoveTask(
            Guid taskId, string newStatus, CancellationToken ct)
        {
            var task = await _db.Set<ProjectPhaseTask>().FindAsync(new object[]{taskId}, ct);
            if (task == null) return NotFound();

            task.Status = newStatus;
            task.UpdatedAtUtc = DateTime.UtcNow;
            if (newStatus == "InProgress" && task.StartedAtUtc == null)
                task.StartedAtUtc = DateTime.UtcNow;
            if (newStatus == "Done" && task.CompletedAtUtc == null)
                task.CompletedAtUtc = DateTime.UtcNow;
            if (newStatus != "Done") task.CompletedAtUtc = null;

            await _db.SaveChangesAsync(ct);
            return Json(new { success = true, status = newStatus });
        }

        private async Task LoadProjectHeaderAsync(Guid projectId)
        {
            var project = await _db.Set<CharityProject>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == projectId);
            if (project != null)
            {
                ViewBag.ProjectHeader = new ProjectHeaderVm { Id = project.Id, Code = project.Code, Name = project.Name, Status = project.Status, StartDate = project.StartDate, EndDate = project.EndDate, Budget = project.Budget };
            }
        }
    }
}
