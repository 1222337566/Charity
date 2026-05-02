using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Milestones;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Controllers
{
    public class ProjectPhaseMilestonesController : Controller
    {
        private readonly IProjectPhaseMilestoneRepository _repository;
        private readonly IProjectPhaseRepository _phaseRepository;
        private readonly ICharityProjectRepository _projectRepository;

        public ProjectPhaseMilestonesController(IProjectPhaseMilestoneRepository repository, IProjectPhaseRepository phaseRepository, ICharityProjectRepository projectRepository)
        {
            _repository = repository;
            _phaseRepository = phaseRepository;
            _projectRepository = projectRepository;
        }

        public async Task<IActionResult> Index(Guid phaseId)
        {
            var phase = await _phaseRepository.GetByIdAsync(phaseId);
            if (phase == null) return NotFound();
            if (!await PopulateProjectAsync(phase.ProjectId, phase)) return NotFound();
            var items = await _repository.GetByPhaseIdAsync(phaseId);
            return View(items.Select(x => new ProjectPhaseMilestoneListItemVm
            {
                Id = x.Id,
                Title = x.Title,
                DueDate = x.DueDate,
                CompletedDate = x.CompletedDate,
                Status = x.Status,
                ProgressPercent = x.ProgressPercent,
                Notes = x.Notes
            }).ToList());
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid phaseId)
        {
            var phase = await _phaseRepository.GetByIdAsync(phaseId);
            if (phase == null) return NotFound();
            if (!await PopulateProjectAsync(phase.ProjectId, phase)) return NotFound();
            return View(new CreateProjectPhaseMilestoneVm { ProjectPhaseId = phaseId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProjectPhaseMilestoneVm vm)
        {
            var phase = await _phaseRepository.GetByIdAsync(vm.ProjectPhaseId);
            if (phase == null) return NotFound();
            if (!await PopulateProjectAsync(phase.ProjectId, phase)) return NotFound();
            if (vm.CompletedDate.HasValue && vm.CompletedDate.Value < vm.DueDate.AddYears(-50))
                ModelState.AddModelError(nameof(vm.CompletedDate), "تاريخ الإنجاز غير منطقي");
            if (!ModelState.IsValid) return View(vm);

            await _repository.AddAsync(new ProjectPhaseMilestone
            {
                Id = Guid.NewGuid(),
                ProjectPhaseId = vm.ProjectPhaseId,
                Title = vm.Title.Trim(),
                Description = vm.Description?.Trim(),
                DueDate = vm.DueDate,
                CompletedDate = vm.CompletedDate,
                Status = vm.Status.Trim(),
                ProgressPercent = vm.ProgressPercent,
                Notes = vm.Notes?.Trim(),
                IsActive = vm.IsActive,
                CreatedAtUtc = DateTime.UtcNow
            });

            TempData["Success"] = "تمت إضافة milestone";
            return RedirectToAction(nameof(Index), new { phaseId = vm.ProjectPhaseId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();
            var phase = await _phaseRepository.GetByIdAsync(entity.ProjectPhaseId);
            if (phase == null) return NotFound();
            if (!await PopulateProjectAsync(phase.ProjectId, phase)) return NotFound();
            return View(new EditProjectPhaseMilestoneVm
            {
                Id = entity.Id,
                ProjectPhaseId = entity.ProjectPhaseId,
                Title = entity.Title,
                Description = entity.Description,
                DueDate = entity.DueDate,
                CompletedDate = entity.CompletedDate,
                Status = entity.Status,
                ProgressPercent = entity.ProgressPercent,
                Notes = entity.Notes,
                IsActive = entity.IsActive
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProjectPhaseMilestoneVm vm)
        {
            var entity = await _repository.GetByIdAsync(vm.Id);
            if (entity == null) return NotFound();
            var phase = await _phaseRepository.GetByIdAsync(entity.ProjectPhaseId);
            if (phase == null) return NotFound();
            if (!await PopulateProjectAsync(phase.ProjectId, phase)) return NotFound();
            if (!ModelState.IsValid) return View(vm);

            entity.Title = vm.Title.Trim();
            entity.Description = vm.Description?.Trim();
            entity.DueDate = vm.DueDate;
            entity.CompletedDate = vm.CompletedDate;
            entity.Status = vm.Status.Trim();
            entity.ProgressPercent = vm.ProgressPercent;
            entity.Notes = vm.Notes?.Trim();
            entity.IsActive = vm.IsActive;

            await _repository.UpdateAsync(entity);
            TempData["Success"] = "تم تعديل milestone";
            return RedirectToAction(nameof(Index), new { phaseId = entity.ProjectPhaseId });
        }

        private async Task<bool> PopulateProjectAsync(Guid projectId, ProjectPhase phase)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null) return false;
            ViewBag.ProjectHeader = new ProjectHeaderVm { Id = project.Id, Code = project.Code, Name = project.Name, Status = project.Status, StartDate = project.StartDate, EndDate = project.EndDate, Budget = project.Budget, IsActive = project.IsActive };
            ViewBag.PhaseName = phase.Name;
            ViewBag.PhaseId = phase.Id;
            return true;
        }
    }
}
