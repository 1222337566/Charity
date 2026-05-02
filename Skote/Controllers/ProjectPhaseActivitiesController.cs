using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Activities;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    public class ProjectPhaseActivitiesController : Controller
    {
        private readonly IProjectPhaseActivityRepository _repository;
        private readonly AppDbContext _db;
        public ProjectPhaseActivitiesController(IProjectPhaseActivityRepository repository, AppDbContext db)
        {
            _repository = repository;
            _db = db;
        }

        public async Task<IActionResult> Index(Guid? phaseId)
        {
            ViewBag.PhaseId = phaseId;
            ViewBag.CurrentPhaseId = phaseId;

            if (phaseId.HasValue && phaseId.Value != Guid.Empty)
            {
                var phase = await _db.Set<ProjectPhase>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == phaseId.Value);
                if (phase == null) return NotFound();
                await LoadProjectHeaderAsync(phase.ProjectId);
                ViewBag.ProjectId = phase.ProjectId;
                ViewBag.PhaseName = phase.Name;
                return View(await _repository.GetByPhaseIdAsync(phaseId.Value));
            }

            ViewBag.PhaseName = "جميع أنشطة مراحل المشروعات";
            ViewBag.ProjectId = null;

            var items = await _db.Set<ProjectPhaseActivity>()
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAtUtc)
                .ThenBy(x => x.Title)
                .Select(x => new ProjectPhaseActivityListItemVm
                {
                    Id = x.Id,
                    ProjectId = x.ProjectId,
                    PhaseId = x.PhaseId,
                    Code = x.Code,
                    Title = x.Title,
                    Status = x.Status,
                    Priority = x.Priority,
                    ProgressPercent = x.ProgressPercent,
                    PlannedHours = x.PlannedHours,
                    ActualHours = x.ActualHours,
                    ResponsiblePersonName = x.ResponsiblePersonName,
                    PlannedStartDate = x.PlannedStartDate,
                    PlannedEndDate = x.PlannedEndDate,
                    OpenTasksCount = _db.Set<ProjectPhaseTask>().Count(t => t.ActivityId == x.Id && t.IsActive && t.Status != "Done" && t.Status != "Completed")
                })
                .ToListAsync();

            return View(items);
        }

        public IActionResult Create(Guid projectId, Guid phaseId)
            => View(new CreateProjectPhaseActivityVm { ProjectId = projectId, PhaseId = phaseId });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProjectPhaseActivityVm vm)
        {
            if (!ModelState.IsValid) return View(vm);
            await _repository.AddAsync(new ProjectPhaseActivity
            {
                Id = Guid.NewGuid(),
                ProjectId = vm.ProjectId,
                PhaseId = vm.PhaseId,
                Code = vm.Code ?? string.Empty,
                Title = vm.Title,
                Description = vm.Description,
                SortOrder = vm.SortOrder,
                PlannedStartDate = vm.PlannedStartDate,
                PlannedEndDate = vm.PlannedEndDate,
                ActualStartDate = vm.ActualStartDate,
                ActualEndDate = vm.ActualEndDate,
                Status = vm.Status,
                Priority = vm.Priority,
                ProgressPercent = vm.ProgressPercent,
                PlannedHours = vm.PlannedHours,
                ActualHours = vm.ActualHours,
                ResponsiblePersonName = vm.ResponsiblePersonName,
                Notes = vm.Notes,
                IsActive = vm.IsActive
            });
            return RedirectToAction(nameof(Index), new { phaseId = vm.PhaseId });
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return View(new EditProjectPhaseActivityVm
            {
                Id = entity.Id,
                ProjectId = entity.ProjectId,
                PhaseId = entity.PhaseId,
                Code = entity.Code,
                Title = entity.Title,
                Description = entity.Description,
                SortOrder = entity.SortOrder,
                PlannedStartDate = entity.PlannedStartDate,
                PlannedEndDate = entity.PlannedEndDate,
                ActualStartDate = entity.ActualStartDate,
                ActualEndDate = entity.ActualEndDate,
                Status = entity.Status,
                Priority = entity.Priority,
                ProgressPercent = entity.ProgressPercent,
                PlannedHours = entity.PlannedHours,
                ActualHours = entity.ActualHours,
                ResponsiblePersonName = entity.ResponsiblePersonName,
                Notes = entity.Notes,
                IsActive = entity.IsActive
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProjectPhaseActivityVm vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var entity = await _repository.GetByIdAsync(vm.Id);
            if (entity == null) return NotFound();
            entity.Code = vm.Code ?? string.Empty;
            entity.Title = vm.Title;
            entity.Description = vm.Description;
            entity.SortOrder = vm.SortOrder;
            entity.PlannedStartDate = vm.PlannedStartDate;
            entity.PlannedEndDate = vm.PlannedEndDate;
            entity.ActualStartDate = vm.ActualStartDate;
            entity.ActualEndDate = vm.ActualEndDate;
            entity.Status = vm.Status;
            entity.Priority = vm.Priority;
            entity.ProgressPercent = vm.ProgressPercent;
            entity.PlannedHours = vm.PlannedHours;
            entity.ActualHours = vm.ActualHours;
            entity.ResponsiblePersonName = vm.ResponsiblePersonName;
            entity.Notes = vm.Notes;
            entity.IsActive = vm.IsActive;
            await _repository.UpdateAsync(entity);
            return RedirectToAction(nameof(Index), new { phaseId = entity.PhaseId });
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
