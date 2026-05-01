using System.Security.Claims;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.TaskUpdates;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    public class ProjectTaskDailyUpdatesController : Controller
    {
        private readonly IProjectTaskDailyUpdateRepository _repository;
        private readonly AppDbContext _db;
        public ProjectTaskDailyUpdatesController(IProjectTaskDailyUpdateRepository repository, AppDbContext db)
        {
            _repository = repository;
            _db = db;
        }

        public async Task<IActionResult> Index(Guid taskId)
        {
            var task = await _db.Set<ProjectPhaseTask>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == taskId);
            if (task == null) return NotFound();
            await LoadProjectHeaderAsync(task.ProjectId);
            ViewBag.TaskId = taskId;
            ViewBag.PhaseId = task.PhaseId;
            ViewBag.ActivityId = task.ActivityId;
            ViewBag.TaskTitle = task.Title;
            return View(await _repository.GetByTaskIdAsync(taskId));
        }

        public IActionResult Create(Guid taskId)
            => View(new CreateProjectTaskDailyUpdateVm { TaskId = taskId, UpdateDate = DateTime.Today });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProjectTaskDailyUpdateVm vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserName = User.Identity?.Name;
            await _repository.AddAsync(new ProjectTaskDailyUpdate
            {
                Id = Guid.NewGuid(),
                TaskId = vm.TaskId,
                UpdateDate = vm.UpdateDate.Date,
                Status = vm.Status,
                ProgressPercent = vm.ProgressPercent,
                HoursSpent = vm.HoursSpent,
                Note = vm.Note,
                BlockerNote = vm.BlockerNote,
                NextAction = vm.NextAction,
                CreatedByUserId = currentUserId,
                CreatedByName = currentUserName
            });
            return RedirectToAction(nameof(Index), new { taskId = vm.TaskId });
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return View(new EditProjectTaskDailyUpdateVm
            {
                Id = entity.Id,
                TaskId = entity.TaskId,
                UpdateDate = entity.UpdateDate,
                Status = entity.Status,
                ProgressPercent = entity.ProgressPercent,
                HoursSpent = entity.HoursSpent,
                Note = entity.Note,
                BlockerNote = entity.BlockerNote,
                NextAction = entity.NextAction
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProjectTaskDailyUpdateVm vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var entity = await _repository.GetByIdAsync(vm.Id);
            if (entity == null) return NotFound();
            entity.UpdateDate = vm.UpdateDate.Date;
            entity.Status = vm.Status;
            entity.ProgressPercent = vm.ProgressPercent;
            entity.HoursSpent = vm.HoursSpent;
            entity.Note = vm.Note;
            entity.BlockerNote = vm.BlockerNote;
            entity.NextAction = vm.NextAction;
            await _repository.UpdateAsync(entity);
            return RedirectToAction(nameof(Index), new { taskId = entity.TaskId });
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
