using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Phases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    public class ProjectPhasesController : Controller
    {
        private readonly IProjectPhaseRepository _phaseRepository;
        private readonly ICharityProjectRepository _projectRepository;
        private readonly AppDbContext _db;

        public ProjectPhasesController(IProjectPhaseRepository phaseRepository, ICharityProjectRepository projectRepository, AppDbContext db)
        {
            _phaseRepository = phaseRepository;
            _projectRepository = projectRepository;
            _db = db;
        }

        public async Task<IActionResult> Index(Guid? projectId)
        {
            var hasProjectContext = projectId.HasValue && projectId.Value != Guid.Empty;
            if (hasProjectContext)
            {
                if (!await PopulateProjectAsync(projectId!.Value)) return NotFound();
                var items = await _phaseRepository.GetByProjectIdAsync(projectId.Value);
                return View(items.Select(MapPhase).ToList());
            }

            var phases = await _db.Set<ProjectPhase>()
                .AsNoTracking()
                .OrderBy(x => x.ProjectId)
                .ThenBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .ToListAsync();

            ViewBag.ShowProjectColumn = true;
            var projectNames = await _db.Set<CharityProject>()
                .AsNoTracking()
                .ToDictionaryAsync(x => x.Id, x => x.Name);
            ViewBag.ProjectNamesByPhaseId = phases.ToDictionary(x => x.Id, x => projectNames.TryGetValue(x.ProjectId, out var name) ? name : string.Empty);

            return View(phases.Select(MapPhase).ToList());
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid projectId)
        {
            if (!await PopulateProjectAsync(projectId)) return NotFound();
            return View(new CreateProjectPhaseVm { ProjectId = projectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProjectPhaseVm vm)
        {
            if (!await PopulateProjectAsync(vm.ProjectId)) return NotFound();
            ValidateDates(vm.PlannedStartDate, vm.PlannedEndDate, vm.ActualStartDate, vm.ActualEndDate);
            if (!ModelState.IsValid) return View(vm);

            await _phaseRepository.AddAsync(new ProjectPhase
            {
                Id = Guid.NewGuid(),
                ProjectId = vm.ProjectId,
                Code = vm.Code?.Trim() ?? string.Empty,
                Name = vm.Name.Trim(),
                Description = vm.Description?.Trim(),
                SortOrder = vm.SortOrder,
                PlannedStartDate = vm.PlannedStartDate,
                PlannedEndDate = vm.PlannedEndDate,
                ActualStartDate = vm.ActualStartDate,
                ActualEndDate = vm.ActualEndDate,
                Status = vm.Status.Trim(),
                ProgressPercent = vm.ProgressPercent,
                PlannedCost = vm.PlannedCost,
                ActualCost = vm.ActualCost,
                ResponsiblePersonName = vm.ResponsiblePersonName?.Trim(),
                Notes = vm.Notes?.Trim(),
                IsActive = vm.IsActive,
                CreatedAtUtc = DateTime.UtcNow
            });

            TempData["Success"] = "تمت إضافة مرحلة المشروع";
            return RedirectToAction(nameof(Index), new { projectId = vm.ProjectId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _phaseRepository.GetByIdAsync(id);
            if (entity == null) return NotFound();
            if (!await PopulateProjectAsync(entity.ProjectId)) return NotFound();

            return View(new EditProjectPhaseVm
            {
                Id = entity.Id,
                ProjectId = entity.ProjectId,
                Code = entity.Code,
                Name = entity.Name,
                Description = entity.Description,
                SortOrder = entity.SortOrder,
                PlannedStartDate = entity.PlannedStartDate,
                PlannedEndDate = entity.PlannedEndDate,
                ActualStartDate = entity.ActualStartDate,
                ActualEndDate = entity.ActualEndDate,
                Status = entity.Status,
                ProgressPercent = entity.ProgressPercent,
                PlannedCost = entity.PlannedCost,
                ActualCost = entity.ActualCost,
                ResponsiblePersonName = entity.ResponsiblePersonName,
                Notes = entity.Notes,
                IsActive = entity.IsActive
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProjectPhaseVm vm)
        {
            var entity = await _phaseRepository.GetByIdAsync(vm.Id);
            if (entity == null) return NotFound();
            if (!await PopulateProjectAsync(entity.ProjectId)) return NotFound();
            ValidateDates(vm.PlannedStartDate, vm.PlannedEndDate, vm.ActualStartDate, vm.ActualEndDate);
            if (!ModelState.IsValid) return View(vm);

            entity.Code = vm.Code?.Trim() ?? string.Empty;
            entity.Name = vm.Name.Trim();
            entity.Description = vm.Description?.Trim();
            entity.SortOrder = vm.SortOrder;
            entity.PlannedStartDate = vm.PlannedStartDate;
            entity.PlannedEndDate = vm.PlannedEndDate;
            entity.ActualStartDate = vm.ActualStartDate;
            entity.ActualEndDate = vm.ActualEndDate;
            entity.Status = vm.Status.Trim();
            entity.ProgressPercent = vm.ProgressPercent;
            entity.PlannedCost = vm.PlannedCost;
            entity.ActualCost = vm.ActualCost;
            entity.ResponsiblePersonName = vm.ResponsiblePersonName?.Trim();
            entity.Notes = vm.Notes?.Trim();
            entity.IsActive = vm.IsActive;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _phaseRepository.UpdateAsync(entity);
            TempData["Success"] = "تم تعديل المرحلة";
            return RedirectToAction(nameof(Index), new { projectId = entity.ProjectId });
        }

        private static ProjectPhaseListItemVm MapPhase(ProjectPhase x) => new()
        {
            Id = x.Id,
            Code = x.Code,
            Name = x.Name,
            SortOrder = x.SortOrder,
            PlannedStartDate = x.PlannedStartDate,
            PlannedEndDate = x.PlannedEndDate,
            Status = x.Status,
            ProgressPercent = x.ProgressPercent,
            PlannedCost = x.PlannedCost,
            ActualCost = x.ActualCost,
            ResponsiblePersonName = x.ResponsiblePersonName
        };

        private void ValidateDates(DateTime plannedStart, DateTime plannedEnd, DateTime? actualStart, DateTime? actualEnd)
        {
            if (plannedEnd < plannedStart)
                ModelState.AddModelError(nameof(CreateProjectPhaseVm.PlannedEndDate), "النهاية المخططة يجب أن تكون بعد البداية المخططة");
            if (actualStart.HasValue && actualEnd.HasValue && actualEnd.Value < actualStart.Value)
                ModelState.AddModelError(nameof(CreateProjectPhaseVm.ActualEndDate), "النهاية الفعلية يجب أن تكون بعد البداية الفعلية");
        }

        private async Task<bool> PopulateProjectAsync(Guid projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null) return false;
            ViewBag.ProjectHeader = new ProjectHeaderVm { Id = project.Id, Code = project.Code, Name = project.Name, Status = project.Status, StartDate = project.StartDate, EndDate = project.EndDate, Budget = project.Budget, IsActive = project.IsActive };
            return true;
        }
    }
}
