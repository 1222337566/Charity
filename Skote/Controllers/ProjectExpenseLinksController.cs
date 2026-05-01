using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Domains.Expenses;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.AccountingProjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class ProjectExpenseLinksController : Controller
{
    private readonly IProjectExpenseLinkRepository _repository;
    private readonly AppDbContext _db;

    public ProjectExpenseLinksController(IProjectExpenseLinkRepository repository, AppDbContext db)
    {
        _repository = repository;
        _db = db;
    }

    public async Task<IActionResult> Index(Guid? projectId)
    {
        var items = projectId.HasValue ? await _repository.GetByProjectIdAsync(projectId.Value) : await _repository.GetAllAsync();
        var model = items.Select(x => new ProjectExpenseLinkListItemVm
        {
            Id = x.Id,
            ExpenseNumber = x.Expense?.ExpenseNumber ?? string.Empty,
            ExpenseDateUtc = x.Expense?.ExpenseDateUtc ?? DateTime.MinValue,
            Amount = x.Expense?.Amount ?? 0m,
            ProjectName = x.Project?.Name ?? string.Empty,
            BudgetLineName = x.ProjectBudgetLine?.LineName,
            CostCenterName = (string)(x.CostCenter?.NameAr),
            Notes = x.Notes
        }).ToList();
        ViewBag.ProjectId = projectId;
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid? projectId = null)
    {
        var vm = new CreateProjectExpenseLinkVm { ProjectId = projectId ?? Guid.Empty };
        await FillLookupsAsync(vm, projectId);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProjectExpenseLinkVm vm)
    {
        await FillLookupsAsync(vm, vm.ProjectId == Guid.Empty ? null : vm.ProjectId);
        if (!ModelState.IsValid) return View(vm);
        if (await _repository.GetByExpenseIdAsync(vm.ExpenseId) != null)
        {
            ModelState.AddModelError(nameof(vm.ExpenseId), "هذا المصروف مرتبط بالفعل بمشروع");
            return View(vm);
        }

        await _repository.AddAsync(new ProjectExpenseLink
        {
            Id = Guid.NewGuid(), ExpenseId = vm.ExpenseId, ProjectId = vm.ProjectId, ProjectBudgetLineId = vm.ProjectBudgetLineId,
            CostCenterId = vm.CostCenterId, Notes = vm.Notes?.Trim(), CreatedAtUtc = DateTime.UtcNow
        });
        TempData["Success"] = "تم ربط المصروف بالمشروع بنجاح";
        return RedirectToAction(nameof(Index), new { projectId = vm.ProjectId });
    }

    private async Task FillLookupsAsync(CreateProjectExpenseLinkVm vm, Guid? selectedProjectId)
    {
        var linkedExpenseIds = await _db.Set<ProjectExpenseLink>().Select(x => x.ExpenseId).ToListAsync();
        vm.Expenses = await _db.Set<Expensex>().AsNoTracking().Where(x => !linkedExpenseIds.Contains(x.Id)).OrderByDescending(x => x.ExpenseDateUtc)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.ExpenseNumber + " - " + x.Amount }).ToListAsync();
        vm.Projects = await _db.Set<CharityProject>().AsNoTracking().OrderBy(x => x.Name)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();
        vm.CostCenters = await _db.Set<CostCenter>().AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.NameAr)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = (string)x.NameAr }).ToListAsync();
        var pid = selectedProjectId ?? (vm.ProjectId == Guid.Empty ? null : vm.ProjectId);
        vm.BudgetLines = pid.HasValue
            ? await _db.Set<ProjectBudgetLine>().AsNoTracking().Where(x => x.ProjectId == pid.Value).OrderBy(x => x.LineName)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.LineName }).ToListAsync()
            : new List<SelectListItem>();
    }
}
