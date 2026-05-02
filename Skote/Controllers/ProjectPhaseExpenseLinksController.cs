using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Domains.Expenses;
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
    public class ProjectPhaseExpenseLinksController : Controller
    {
        private readonly IProjectPhaseExpenseLinkRepository _repository;
        private readonly ICharityProjectRepository _projectRepository;
        private readonly IProjectPhaseRepository _phaseRepository;
        private readonly AppDbContext _db;

        public ProjectPhaseExpenseLinksController(IProjectPhaseExpenseLinkRepository repository, ICharityProjectRepository projectRepository, IProjectPhaseRepository phaseRepository, AppDbContext db)
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
            var allItems = await _db.Set<ProjectPhaseExpenseLink>()
                .AsNoTracking()
                .OrderByDescending(x => x.Expense!.ExpenseDateUtc)
                .ThenBy(x => x.Project!.Name)
                .Select(x => new ProjectPhaseExpenseLinkListItemVm
                {
                    Id = x.Id,
                    ProjectId = x.ProjectId,
                    PhaseId = x.ProjectPhaseId,
                    ExpenseNumber = x.Expense != null ? x.Expense.ExpenseNumber : string.Empty,
                    ExpenseDateUtc = x.Expense != null ? x.Expense.ExpenseDateUtc : x.CreatedAtUtc,
                    Amount = x.Expense != null ? x.Expense.Amount : 0m,
                    ProjectName = x.Project != null ? x.Project.Name : string.Empty,
                    PhaseName = x.ProjectPhase != null ? x.ProjectPhase.Name : string.Empty,
                    BudgetLineName = x.ProjectBudgetLine != null ? x.ProjectBudgetLine.LineName : null,
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
            var vm = new CreateProjectPhaseExpenseLinkVm { ProjectId = projectId };
            await FillLookupsAsync(vm, projectId);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProjectPhaseExpenseLinkVm vm)
        {
            if (!await PopulateProjectAsync(vm.ProjectId)) return NotFound();
            if (await _repository.GetByExpenseIdAsync(vm.ExpenseId) != null)
                ModelState.AddModelError(nameof(vm.ExpenseId), "هذا المصروف مربوط بمرحلة بالفعل");
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

            await _repository.AddAsync(new ProjectPhaseExpenseLink
            {
                Id = Guid.NewGuid(),
                ExpenseId = vm.ExpenseId,
                ProjectId = vm.ProjectId,
                ProjectPhaseId = vm.ProjectPhaseId,
                ProjectBudgetLineId = vm.ProjectBudgetLineId,
                CostCenterId = vm.CostCenterId,
                IncludeInActualCost = vm.IncludeInActualCost,
                Notes = vm.Notes?.Trim(),
                CreatedAtUtc = DateTime.UtcNow
            });

            TempData["Success"] = "تم ربط المصروف بمرحلة المشروع";
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

        private static ProjectPhaseExpenseLinkListItemVm MapItem(ProjectPhaseExpenseLink x) => new()
        {
            Id = x.Id,
            ProjectId = x.ProjectId,
            PhaseId = x.ProjectPhaseId,
            ExpenseNumber = x.Expense?.ExpenseNumber ?? string.Empty,
            ExpenseDateUtc = x.Expense?.ExpenseDateUtc ?? x.CreatedAtUtc,
            Amount = x.Expense?.Amount ?? 0m,
            ProjectName = x.Project?.Name ?? string.Empty,
            PhaseName = x.ProjectPhase?.Name ?? string.Empty,
            BudgetLineName = x.ProjectBudgetLine?.LineName,
            CostCenterName = x.CostCenter?.NameAr,
            IncludeInActualCost = x.IncludeInActualCost,
            Notes = x.Notes
        };

        private async Task FillLookupsAsync(CreateProjectPhaseExpenseLinkVm vm, Guid projectId)
        {
            vm.Expenses = await _db.Set<Expensex>().AsNoTracking()
                .OrderByDescending(x => x.ExpenseDateUtc)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.ExpenseNumber + " | " + x.Amount })
                .ToListAsync();

            vm.Projects = await _db.Set<CharityProject>().AsNoTracking().OrderBy(x => x.Name)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();

            vm.Phases = await _db.Set<ProjectPhase>().AsNoTracking().Where(x => x.ProjectId == projectId)
                .OrderBy(x => x.SortOrder).ThenBy(x => x.Name)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();

            vm.BudgetLines = await _db.Set<ProjectBudgetLine>().AsNoTracking().Where(x => x.ProjectId == projectId)
                .OrderBy(x => x.LineName)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.LineName }).ToListAsync();

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
