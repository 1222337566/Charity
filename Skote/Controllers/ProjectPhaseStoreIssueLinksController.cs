using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.AccountingProjects.Phases;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    public class ProjectPhaseStoreIssueLinksController : Controller
    {
        private readonly IProjectPhaseStoreIssueLinkRepository _repository;
        private readonly ICharityProjectRepository _projectRepository;
        private readonly IProjectPhaseRepository _phaseRepository;
        private readonly AppDbContext _db;

        public ProjectPhaseStoreIssueLinksController(IProjectPhaseStoreIssueLinkRepository repository, ICharityProjectRepository projectRepository, IProjectPhaseRepository phaseRepository, AppDbContext db)
        {
            _repository = repository;
            _projectRepository = projectRepository;
            _phaseRepository = phaseRepository;
            _db = db;
        }

        public async Task<IActionResult> Index(Guid? projectId)
        {
            var hasProjectContext = projectId.HasValue && projectId.Value != Guid.Empty;
            if (hasProjectContext)
            {
                if (!await PopulateProjectAsync(projectId!.Value)) return NotFound();
                var items = await _repository.GetByProjectIdAsync(projectId.Value);
                var vm = items.Select(MapItem).ToList();
                ViewBag.ProjectId = projectId.Value;
                return View(vm);
            }

            ViewBag.ShowProjectColumn = true;
            var allItems = await _db.Set<ProjectPhaseStoreIssueLink>()
                .AsNoTracking()
                .OrderByDescending(x => x.StoreIssue!.IssueDate)
                .ThenBy(x => x.Project!.Name)
                .Select(x => new ProjectPhaseStoreIssueLinkListItemVm
                {
                    Id = x.Id,
                    ProjectId = x.ProjectId,
                    PhaseId = x.ProjectPhaseId,
                    IssueNumber = x.StoreIssue != null ? x.StoreIssue.IssueNumber : string.Empty,
                    IssueDate = x.StoreIssue != null ? x.StoreIssue.IssueDate : x.CreatedAtUtc,
                    Amount = x.StoreIssue != null ? x.StoreIssue.Lines.Sum(l => l.Quantity * l.UnitCost) : 0m,
                    ProjectName = x.Project != null ? x.Project.Name : string.Empty,
                    PhaseName = x.ProjectPhase != null ? x.ProjectPhase.Name : string.Empty,
                    CostCenterName = x.CostCenter != null ? x.CostCenter.NameAr : null,
                    IncludeInActualCost = x.IncludeInActualCost,
                    Notes = x.Notes
                })
                .ToListAsync();
            return View(allItems);
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid projectId)
        {
            if (!await PopulateProjectAsync(projectId)) return NotFound();
            var vm = new CreateProjectPhaseStoreIssueLinkVm { ProjectId = projectId };
            await FillLookupsAsync(vm, projectId);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProjectPhaseStoreIssueLinkVm vm)
        {
            if (!await PopulateProjectAsync(vm.ProjectId)) return NotFound();
            if (await _repository.GetByStoreIssueIdAsync(vm.StoreIssueId) != null)
                ModelState.AddModelError(nameof(vm.StoreIssueId), "إذن الصرف مربوط بمرحلة بالفعل");
            if (!ModelState.IsValid)
            {
                await FillLookupsAsync(vm, vm.ProjectId);
                return View(vm);
            }

            var phase = await _phaseRepository.GetByIdAsync(vm.ProjectPhaseId);
            if (phase == null || phase.ProjectId != vm.ProjectId)
            {
                ModelState.AddModelError(nameof(vm.ProjectPhaseId), "المرحلة المختارة غير تابعة للمشروع");
                await FillLookupsAsync(vm, vm.ProjectId);
                return View(vm);
            }

            await _repository.AddAsync(new ProjectPhaseStoreIssueLink
            {
                Id = Guid.NewGuid(),
                StoreIssueId = vm.StoreIssueId,
                ProjectId = vm.ProjectId,
                ProjectPhaseId = vm.ProjectPhaseId,
                CostCenterId = vm.CostCenterId,
                IncludeInActualCost = vm.IncludeInActualCost,
                Notes = vm.Notes?.Trim(),
                CreatedAtUtc = DateTime.UtcNow
            });

            TempData["Success"] = "تم ربط إذن الصرف بمرحلة المشروع";
            return RedirectToAction(nameof(Index), new { projectId = vm.ProjectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, Guid? projectId)
        {
            await _repository.DeleteAsync(id);
            TempData["Success"] = "تم حذف الربط";
            return RedirectToAction(nameof(Index), new { projectId });
        }

        private static ProjectPhaseStoreIssueLinkListItemVm MapItem(ProjectPhaseStoreIssueLink x) => new()
        {
            Id = x.Id,
            ProjectId = x.ProjectId,
            PhaseId = x.ProjectPhaseId,
            IssueNumber = x.StoreIssue?.IssueNumber ?? string.Empty,
            IssueDate = x.StoreIssue?.IssueDate ?? x.CreatedAtUtc,
            Amount = x.StoreIssue?.Lines?.Sum(l => l.Quantity * l.UnitCost) ?? 0m,
            ProjectName = x.Project?.Name ?? string.Empty,
            PhaseName = x.ProjectPhase?.Name ?? string.Empty,
            CostCenterName = x.CostCenter?.NameAr,
            IncludeInActualCost = x.IncludeInActualCost,
            Notes = x.Notes
        };

        private async Task FillLookupsAsync(CreateProjectPhaseStoreIssueLinkVm vm, Guid projectId)
        {
            vm.StoreIssues = await _db.Set<CharityStoreIssue>().AsNoTracking()
                .OrderByDescending(x => x.IssueDate)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.IssueNumber + " | " + x.IssueDate.ToString("yyyy-MM-dd") })
                .ToListAsync();

            vm.Projects = await _db.Set<CharityProject>().AsNoTracking().OrderBy(x => x.Name)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();

            vm.Phases = await _db.Set<ProjectPhase>().AsNoTracking().Where(x => x.ProjectId == projectId)
                .OrderBy(x => x.SortOrder).ThenBy(x => x.Name)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();

            vm.CostCenters = await _db.Set<CostCenter>().AsNoTracking().Where(x => x.IsActive)
                .OrderBy(x => x.NameAr)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.NameAr }).ToListAsync();
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
