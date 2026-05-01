using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.AccountingProjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class ProjectAccountingProfilesController : Controller
{
    private readonly IProjectAccountingProfileRepository _repository;
    private readonly AppDbContext _db;

    public ProjectAccountingProfilesController(IProjectAccountingProfileRepository repository, AppDbContext db)
    {
        _repository = repository;
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _repository.GetAllAsync();
        var model = items.Select(x => new ProjectAccountingProfileListItemVm
        {
            Id = x.Id,
            ProjectId = x.ProjectId,
            ProjectName = x.Project?.Name ?? string.Empty,
            CostCenterName = (string)(x.DefaultCostCenter?.NameAr),
            RevenueAccountName = x.DefaultRevenueAccount?.AccountNameAr,
            ExpenseAccountName = x.DefaultExpenseAccount?.AccountNameAr,
            AutoTagJournalLinesWithProject = x.AutoTagJournalLinesWithProject,
            AutoUseProjectCostCenter = x.AutoUseProjectCostCenter,
            IsActive = x.IsActive
        }).ToList();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new CreateProjectAccountingProfileVm();
        await FillLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProjectAccountingProfileVm vm)
    {
        await FillLookupsAsync(vm);
        if (!ModelState.IsValid) return View(vm);
        if (await _repository.GetByProjectIdAsync(vm.ProjectId) != null)
        {
            ModelState.AddModelError(nameof(vm.ProjectId), "يوجد Profile محاسبية للمشروع بالفعل");
            return View(vm);
        }

        await _repository.AddAsync(new ProjectAccountingProfile
        {
            Id = Guid.NewGuid(), ProjectId = vm.ProjectId, DefaultCostCenterId = vm.DefaultCostCenterId,
            DefaultRevenueAccountId = vm.DefaultRevenueAccountId, DefaultExpenseAccountId = vm.DefaultExpenseAccountId,
            AutoTagJournalLinesWithProject = vm.AutoTagJournalLinesWithProject, AutoUseProjectCostCenter = vm.AutoUseProjectCostCenter,
            IsActive = vm.IsActive, Notes = vm.Notes?.Trim(), CreatedAtUtc = DateTime.UtcNow
        });
        TempData["Success"] = "تم إضافة Profile محاسبية للمشروع";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return NotFound();
        var vm = new EditProjectAccountingProfileVm
        {
            Id = entity.Id, ProjectId = entity.ProjectId, DefaultCostCenterId = entity.DefaultCostCenterId,
            DefaultRevenueAccountId = entity.DefaultRevenueAccountId, DefaultExpenseAccountId = entity.DefaultExpenseAccountId,
            AutoTagJournalLinesWithProject = entity.AutoTagJournalLinesWithProject, AutoUseProjectCostCenter = entity.AutoUseProjectCostCenter,
            IsActive = entity.IsActive, Notes = entity.Notes
        };
        await FillLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditProjectAccountingProfileVm vm)
    {
        await FillLookupsAsync(vm);
        if (!ModelState.IsValid) return View(vm);
        var entity = await _repository.GetByIdAsync(vm.Id);
        if (entity == null) return NotFound();

        entity.ProjectId = vm.ProjectId;
        entity.DefaultCostCenterId = vm.DefaultCostCenterId;
        entity.DefaultRevenueAccountId = vm.DefaultRevenueAccountId;
        entity.DefaultExpenseAccountId = vm.DefaultExpenseAccountId;
        entity.AutoTagJournalLinesWithProject = vm.AutoTagJournalLinesWithProject;
        entity.AutoUseProjectCostCenter = vm.AutoUseProjectCostCenter;
        entity.IsActive = vm.IsActive;
        entity.Notes = vm.Notes?.Trim();
        entity.UpdatedAtUtc = DateTime.UtcNow;
        await _repository.UpdateAsync(entity);
        TempData["Success"] = "تم تعديل Profile المشروع";
        return RedirectToAction(nameof(Index));
    }

    private async Task FillLookupsAsync(CreateProjectAccountingProfileVm vm)
    {
        vm.Projects = await _db.Set<CharityProject>().AsNoTracking().OrderBy(x => x.Name)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();
        vm.CostCenters = await _db.Set<CostCenter>().AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.NameAr)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = (string)x.NameAr }).ToListAsync();
        vm.Accounts = await _db.Set<FinancialAccount>().AsNoTracking().Where(x => x.IsActive && x.IsPosting).OrderBy(x => x.AccountCode)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.AccountCode + " - " + x.AccountNameAr }).ToListAsync();
    }
}
