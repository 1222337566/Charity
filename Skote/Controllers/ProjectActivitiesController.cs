using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastructureManagmentDataAccess.Repositories.Charity;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Activities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skote.Helpers;
[Authorize(Policy = CharityPolicies.ProjectsView)]
public class ProjectActivitiesController : Controller
{
    private readonly IProjectActivityRepository _activityRepository;
    private readonly ICharityProjectRepository _projectRepository;

    public ProjectActivitiesController(IProjectActivityRepository activityRepository, ICharityProjectRepository projectRepository)
    {
        _activityRepository = activityRepository;
        _projectRepository = projectRepository;
    }

    public async Task<IActionResult> Index(Guid projectId)
    {
        if (!await PopulateProjectAsync(projectId))
            return NotFound();

        var items = await _activityRepository.GetByProjectIdAsync(projectId);
        return View(items.Select(x => new ProjectActivityListItemVm
        {
            Id = x.Id,
            Title = x.Title,
            PlannedDate = x.PlannedDate,
            ActualDate = x.ActualDate,
            Status = x.Status,
            PlannedCost = x.PlannedCost,
            ActualCost = x.ActualCost,
            Notes = x.Notes
        }).ToList());
    }
    [Authorize(Policy = CharityPolicies.ProjectsManage)]
    [HttpGet]
    public async Task<IActionResult> Create(Guid projectId)
    {
        if (!await PopulateProjectAsync(projectId))
            return NotFound();

        return View(new CreateProjectActivityVm { ProjectId = projectId, PlannedDate = DateTime.Today, Status = "Planned" });
    }
    [Authorize(Policy = CharityPolicies.ProjectsManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProjectActivityVm vm)
    {
        if (!await PopulateProjectAsync(vm.ProjectId))
            return NotFound();

        if (!ModelState.IsValid)
            return View(vm);

        await _activityRepository.AddAsync(new ProjectActivity
        {
            Id = Guid.NewGuid(),
            ProjectId = vm.ProjectId,
            Title = vm.Title.Trim(),
            Description = vm.Description?.Trim(),
            PlannedDate = vm.PlannedDate,
            ActualDate = vm.ActualDate,
            Status = vm.Status.Trim(),
            PlannedCost = vm.PlannedCost,
            ActualCost = vm.ActualCost,
            Notes = vm.Notes?.Trim()
        });

        TempData["Success"] = "تم إضافة النشاط";
        return RedirectToAction(nameof(Index), new { projectId = vm.ProjectId });
    }
    [Authorize(Policy = CharityPolicies.ProjectsManage)]
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _activityRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        if (!await PopulateProjectAsync(entity.ProjectId))
            return NotFound();

        return View(new EditProjectActivityVm
        {
            Id = entity.Id,
            ProjectId = entity.ProjectId,
            Title = entity.Title,
            Description = entity.Description,
            PlannedDate = entity.PlannedDate,
            ActualDate = entity.ActualDate,
            Status = entity.Status,
            PlannedCost = entity.PlannedCost,
            ActualCost = entity.ActualCost,
            Notes = entity.Notes
        });
    }
    [Authorize(Policy = CharityPolicies.ProjectsManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditProjectActivityVm vm)
    {
        var entity = await _activityRepository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        if (!await PopulateProjectAsync(entity.ProjectId))
            return NotFound();

        if (!ModelState.IsValid)
            return View(vm);

        entity.Title = vm.Title.Trim();
        entity.Description = vm.Description?.Trim();
        entity.PlannedDate = vm.PlannedDate;
        entity.ActualDate = vm.ActualDate;
        entity.Status = vm.Status.Trim();
        entity.PlannedCost = vm.PlannedCost;
        entity.ActualCost = vm.ActualCost;
        entity.Notes = vm.Notes?.Trim();

        await _activityRepository.UpdateAsync(entity);
        TempData["Success"] = "تم تعديل النشاط";
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
