using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile;
using InfrastructureManagmentWebFramework.Models.Charity.Projects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class CharityProjectsController : Controller
{
    private readonly ICharityProjectRepository _projectRepository;

    public CharityProjectsController(ICharityProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<IActionResult> Index(ProjectListFilterVm filter)
    {
        FillStatusLookups(filter);
        var items = await _projectRepository.SearchAsync(filter.Q, filter.Status, filter.IsActive);

        var vm = new ProjectListPageVm
        {
            Filter = filter,
            Items = items.Select(x => new ProjectListRowVm
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Budget = x.Budget,
                Status = x.Status,
                IsActive = x.IsActive
            }).ToList()
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var entity = await _projectRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        ViewBag.ProjectHeader = new ProjectHeaderVm
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Status = entity.Status,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Budget = entity.Budget,
            IsActive = entity.IsActive
        };

        var vm = new ProjectDetailsVm
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Description = entity.Description,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Budget = entity.Budget,
            Status = entity.Status,
            TargetBeneficiariesCount = entity.TargetBeneficiariesCount,
            Location = entity.Location,
            Objectives = entity.Objectives,
            Kpis = entity.Kpis,
            Notes = entity.Notes,
            IsActive = entity.IsActive,
            BudgetLinesCount = entity.BudgetLines.Count,
            ActivitiesCount = entity.Activities.Count,
            BeneficiariesCount = entity.Beneficiaries.Count,
            GrantsCount = entity.Grants.Count,
            PlannedBudgetLinesTotal = entity.BudgetLines.Sum(x => x.PlannedAmount),
            ActualBudgetLinesTotal = entity.BudgetLines.Sum(x => x.ActualAmount)
        };

        return View(vm);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var vm = new CreateCharityProjectVm
        {
            StartDate = DateTime.Today,
            Code = $"PRJ-{DateTime.Today:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..4].ToUpper()}",
            Status = ProjectStatusOption.Values.First(),
            IsActive = true
        };
        FillLookups(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCharityProjectVm vm)
    {
        FillLookups(vm);

        if (!ModelState.IsValid)
            return View(vm);

        if (await _projectRepository.CodeExistsAsync(vm.Code.Trim()))
        {
            ModelState.AddModelError(nameof(vm.Code), "كود المشروع موجود بالفعل");
            return View(vm);
        }

        var entity = new CharityProject
        {
            Id = Guid.NewGuid(),
            Code = vm.Code.Trim(),
            Name = vm.Name.Trim(),
            Description = vm.Description?.Trim(),
            StartDate = vm.StartDate,
            EndDate = vm.EndDate,
            Budget = vm.Budget,
            Status = vm.Status,
            TargetBeneficiariesCount = vm.TargetBeneficiariesCount,
            Location = vm.Location?.Trim(),
            Objectives = vm.Objectives?.Trim(),
            Kpis = vm.Kpis?.Trim(),
            Notes = vm.Notes?.Trim(),
            IsActive = vm.IsActive,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _projectRepository.AddAsync(entity);
        TempData["Success"] = "تم إنشاء المشروع بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _projectRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var vm = new EditCharityProjectVm
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Description = entity.Description,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Budget = entity.Budget,
            Status = entity.Status,
            TargetBeneficiariesCount = entity.TargetBeneficiariesCount,
            Location = entity.Location,
            Objectives = entity.Objectives,
            Kpis = entity.Kpis,
            Notes = entity.Notes,
            IsActive = entity.IsActive
        };
        FillLookups(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditCharityProjectVm vm)
    {
        var entity = await _projectRepository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        FillLookups(vm);

        if (!ModelState.IsValid)
            return View(vm);

        if (await _projectRepository.CodeExistsAsync(vm.Code.Trim(), vm.Id))
        {
            ModelState.AddModelError(nameof(vm.Code), "كود المشروع موجود بالفعل");
            return View(vm);
        }

        entity.Code = vm.Code.Trim();
        entity.Name = vm.Name.Trim();
        entity.Description = vm.Description?.Trim();
        entity.StartDate = vm.StartDate;
        entity.EndDate = vm.EndDate;
        entity.Budget = vm.Budget;
        entity.Status = vm.Status;
        entity.TargetBeneficiariesCount = vm.TargetBeneficiariesCount;
        entity.Location = vm.Location?.Trim();
        entity.Objectives = vm.Objectives?.Trim();
        entity.Kpis = vm.Kpis?.Trim();
        entity.Notes = vm.Notes?.Trim();
        entity.IsActive = vm.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _projectRepository.UpdateAsync(entity);
        TempData["Success"] = "تم تعديل المشروع بنجاح";
        return RedirectToAction(nameof(Details), new { id = vm.Id });
    }

    private static void FillLookups(CreateCharityProjectVm vm)
        => vm.Statuses = ProjectStatusOption.Values.Select(x => new SelectListItem { Value = x, Text = x }).ToList();

    private static void FillStatusLookups(ProjectListFilterVm vm)
    {
        vm.Statuses = ProjectStatusOption.Values.Select(x => new SelectListItem { Value = x, Text = x }).ToList();
        vm.Statuses.Insert(0, new SelectListItem { Value = "", Text = "الكل" });
    }
}
