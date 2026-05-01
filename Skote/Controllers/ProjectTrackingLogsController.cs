using System.Security.Claims;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Tracking;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    public class ProjectTrackingLogsController : Controller
    {
        private readonly IProjectTrackingLogRepository _repository;
        private readonly IProjectPhaseRepository _phaseRepository;
        private readonly ICharityProjectRepository _projectRepository;
        private readonly AppDbContext _db;

        public ProjectTrackingLogsController(IProjectTrackingLogRepository repository, IProjectPhaseRepository phaseRepository, ICharityProjectRepository projectRepository, AppDbContext db)
        {
            _repository = repository;
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
                var items = await _repository.GetByProjectIdAsync(projectId.Value);
                return View(items.Select(MapLog).ToList());
            }

            var rows = await _db.Set<ProjectTrackingLog>()
                .AsNoTracking()
                .OrderByDescending(x => x.EntryDate)
                .ThenByDescending(x => x.CreatedAtUtc)
                .Select(x => new
                {
                    Vm = new ProjectTrackingLogListItemVm
                    {
                        Id = x.Id,
                        ProjectPhaseId = x.ProjectPhaseId,
                        EntryDate = x.EntryDate,
                        EntryType = x.EntryType,
                        Title = x.Title,
                        Details = x.Details,
                        Status = x.Status,
                        ProgressPercent = x.ProgressPercent,
                        RequiresAttention = x.RequiresAttention
                    },
                    ProjectName = x.Project != null ? x.Project.Name : string.Empty,
                    ProjectId = x.ProjectId,
                    PhaseName = x.Phase != null ? x.Phase.Name : null
                })
                .ToListAsync();

            ViewBag.ShowProjectColumn = true;
            ViewBag.ProjectNamesByLogId = rows.ToDictionary(x => x.Vm.Id, x => x.ProjectName);
            ViewBag.ProjectIdsByLogId = rows.ToDictionary(x => x.Vm.Id, x => x.ProjectId);
            ViewBag.PhaseNamesByLogId = rows.Where(x => !string.IsNullOrWhiteSpace(x.PhaseName)).ToDictionary(x => x.Vm.Id, x => x.PhaseName!);
            return View(rows.Select(x => x.Vm).ToList());
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid projectId)
        {
            if (!await PopulateProjectAsync(projectId)) return NotFound();
            return View(new CreateProjectTrackingLogVm { ProjectId = projectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProjectTrackingLogVm vm)
        {
            if (!await PopulateProjectAsync(vm.ProjectId)) return NotFound();
            if (!ModelState.IsValid) return View(vm);

            await _repository.AddAsync(new ProjectTrackingLog
            {
                Id = Guid.NewGuid(),
                ProjectId = vm.ProjectId,
                ProjectPhaseId = vm.ProjectPhaseId,
                EntryDate = vm.EntryDate,
                EntryType = vm.EntryType.Trim(),
                Title = vm.Title.Trim(),
                Details = vm.Details.Trim(),
                Status = vm.Status?.Trim(),
                ProgressPercent = vm.ProgressPercent,
                RequiresAttention = vm.RequiresAttention,
                CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CreatedAtUtc = DateTime.UtcNow
            });

            TempData["Success"] = "تمت إضافة سجل المتابعة";
            return RedirectToAction(nameof(Index), new { projectId = vm.ProjectId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();
            if (!await PopulateProjectAsync(entity.ProjectId)) return NotFound();

            return View(new EditProjectTrackingLogVm
            {
                Id = entity.Id,
                ProjectId = entity.ProjectId,
                ProjectPhaseId = entity.ProjectPhaseId,
                EntryDate = entity.EntryDate,
                EntryType = entity.EntryType,
                Title = entity.Title,
                Details = entity.Details,
                Status = entity.Status,
                ProgressPercent = entity.ProgressPercent,
                RequiresAttention = entity.RequiresAttention
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProjectTrackingLogVm vm)
        {
            var entity = await _repository.GetByIdAsync(vm.Id);
            if (entity == null) return NotFound();
            if (!await PopulateProjectAsync(entity.ProjectId)) return NotFound();
            if (!ModelState.IsValid) return View(vm);

            entity.ProjectPhaseId = vm.ProjectPhaseId;
            entity.EntryDate = vm.EntryDate;
            entity.EntryType = vm.EntryType.Trim();
            entity.Title = vm.Title.Trim();
            entity.Details = vm.Details.Trim();
            entity.Status = vm.Status?.Trim();
            entity.ProgressPercent = vm.ProgressPercent;
            entity.RequiresAttention = vm.RequiresAttention;

            await _repository.UpdateAsync(entity);
            TempData["Success"] = "تم تعديل سجل المتابعة";
            return RedirectToAction(nameof(Index), new { projectId = entity.ProjectId });
        }

        private static ProjectTrackingLogListItemVm MapLog(ProjectTrackingLog x) => new()
        {
            Id = x.Id,
            ProjectPhaseId = x.ProjectPhaseId,
            EntryDate = x.EntryDate,
            EntryType = x.EntryType,
            Title = x.Title,
            Details = x.Details,
            Status = x.Status,
            ProgressPercent = x.ProgressPercent,
            RequiresAttention = x.RequiresAttention
        };

        private async Task<bool> PopulateProjectAsync(Guid projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null) return false;
            ViewBag.ProjectHeader = new ProjectHeaderVm { Id = project.Id, Code = project.Code, Name = project.Name, Status = project.Status, StartDate = project.StartDate, EndDate = project.EndDate, Budget = project.Budget, IsActive = project.IsActive };
            var phases = await _phaseRepository.GetByProjectIdAsync(projectId);
            ViewBag.PhaseOptions = phases.Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();
            return true;
        }
    }
}
