using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.BudgetLines;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skote.Helpers;
[Authorize(Policy = CharityPolicies.ProjectsView)]
public class ProjectBudgetLinesController : Controller
{
    private readonly IProjectBudgetLineRepository _budgetLineRepository;
    private readonly ICharityProjectRepository _projectRepository;

    public ProjectBudgetLinesController(IProjectBudgetLineRepository budgetLineRepository, ICharityProjectRepository projectRepository)
    {
        _budgetLineRepository = budgetLineRepository;
        _projectRepository = projectRepository;
    }

    public async Task<IActionResult> Index(Guid projectId)
    {
        if (!await PopulateProjectAsync(projectId))
            return NotFound();

        var items = await _budgetLineRepository.GetByProjectIdAsync(projectId);
        return View(items.Select(x => new ProjectBudgetLineListItemVm
        {
            Id = x.Id,
            LineName = x.LineName,
            LineType = x.LineType,
            PlannedAmount = x.PlannedAmount,
            ActualAmount = x.ActualAmount,
            Notes = x.Notes
        }).ToList());
    }
    [Authorize(Policy = CharityPolicies.ProjectsManage)]
    [HttpGet]
    public async Task<IActionResult> Create(Guid projectId)
    {
        if (!await PopulateProjectAsync(projectId))
            return NotFound();

        return View(new CreateProjectBudgetLineVm { ProjectId = projectId });
    }
    [Authorize(Policy = CharityPolicies.ProjectsManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProjectBudgetLineVm vm)
    {
        if (!await PopulateProjectAsync(vm.ProjectId))
            return NotFound();

        if (!ModelState.IsValid)
            return View(vm);

        await _budgetLineRepository.AddAsync(new ProjectBudgetLine
        {
            Id = Guid.NewGuid(),
            ProjectId = vm.ProjectId,
            LineName = vm.LineName.Trim(),
            LineType = vm.LineType.Trim(),
            PlannedAmount = vm.PlannedAmount,
            ActualAmount = vm.ActualAmount,
            Notes = vm.Notes?.Trim()
        });

        TempData["Success"] = "تم إضافة بند الموازنة";
        return RedirectToAction(nameof(Index), new { projectId = vm.ProjectId });
    }
    [Authorize(Policy = CharityPolicies.ProjectsManage)]
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _budgetLineRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        if (!await PopulateProjectAsync(entity.ProjectId))
            return NotFound();

        return View(new EditProjectBudgetLineVm
        {
            Id = entity.Id,
            ProjectId = entity.ProjectId,
            LineName = entity.LineName,
            LineType = entity.LineType,
            PlannedAmount = entity.PlannedAmount,
            ActualAmount = entity.ActualAmount,
            Notes = entity.Notes
        });
    }
    [Authorize(Policy = CharityPolicies.ProjectsManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditProjectBudgetLineVm vm)
    {
        var entity = await _budgetLineRepository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        if (!await PopulateProjectAsync(entity.ProjectId))
            return NotFound();

        if (!ModelState.IsValid)
            return View(vm);

        entity.LineName = vm.LineName.Trim();
        entity.LineType = vm.LineType.Trim();
        entity.PlannedAmount = vm.PlannedAmount;
        entity.ActualAmount = vm.ActualAmount;
        entity.Notes = vm.Notes?.Trim();

        await _budgetLineRepository.UpdateAsync(entity);
        TempData["Success"] = "تم تعديل بند الموازنة";
        return RedirectToAction(nameof(Index), new { projectId = entity.ProjectId });
    }

    private async Task<bool> PopulateProjectAsync(Guid projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null)
            return false;

        ViewBag.ProjectHeader = new ProjectHeaderVm
        {
            Id = project.Id,
            Code = project.Code,
            Name = project.Name,
            Status = project.Status,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Budget = project.Budget,
            IsActive = project.IsActive
        };

        return true;
    }
}
