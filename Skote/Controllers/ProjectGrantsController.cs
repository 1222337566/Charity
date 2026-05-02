using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Funders;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Grants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Skote.Helpers;
[Authorize(Policy = CharityPolicies.ProjectsView)]
public class ProjectGrantsController : Controller
{
    private readonly IProjectGrantRepository _projectGrantRepository;
    private readonly ICharityProjectRepository _projectRepository;
    private readonly IGrantAgreementRepository _grantAgreementRepository;

    public ProjectGrantsController(IProjectGrantRepository projectGrantRepository,
        ICharityProjectRepository projectRepository,
        IGrantAgreementRepository grantAgreementRepository)
    {
        _projectGrantRepository = projectGrantRepository;
        _projectRepository = projectRepository;
        _grantAgreementRepository = grantAgreementRepository;
    }

    public async Task<IActionResult> Index(Guid projectId)
    {
        if (!await PopulateProjectAsync(projectId))
            return NotFound();

        var items = await _projectGrantRepository.GetByProjectIdAsync(projectId);
        return View(items.Select(x => new ProjectGrantListItemVm
        {
            Id = x.Id,
            GrantAgreementId = x.GrantAgreementId,
            AgreementNumber = x.GrantAgreement?.AgreementNumber ?? string.Empty,
            AgreementTitle = x.GrantAgreement?.Title ?? string.Empty,
            FunderName = x.GrantAgreement?.Funder?.Name ?? string.Empty,
            AllocatedAmount = x.AllocatedAmount,
            AllocatedDate = x.AllocatedDate,
            Notes = x.Notes
        }).ToList());
    }
    [Authorize(Policy = CharityPolicies.ProjectsManage)]
    [HttpGet]
    public async Task<IActionResult> Create(Guid projectId)
    {
        if (!await PopulateProjectAsync(projectId))
            return NotFound();

        var vm = new CreateProjectGrantVm
        {
            ProjectId = projectId,
            AllocatedDate = DateTime.Today
        };
        await FillLookupsAsync(vm);
        return View(vm);
    }
    [Authorize(Policy = CharityPolicies.ProjectsManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProjectGrantVm vm)
    {
        if (!await PopulateProjectAsync(vm.ProjectId))
            return NotFound();

        await FillLookupsAsync(vm);

        if (!ModelState.IsValid)
            return View(vm);

        if (await _projectGrantRepository.ExistsAsync(vm.ProjectId, vm.GrantAgreementId))
        {
            ModelState.AddModelError(nameof(vm.GrantAgreementId), "الاتفاقية مربوطة بالفعل بهذا المشروع");
            return View(vm);
        }

        await _projectGrantRepository.AddAsync(new ProjectGrant
        {
            Id = Guid.NewGuid(),
            ProjectId = vm.ProjectId,
            GrantAgreementId = vm.GrantAgreementId,
            AllocatedAmount = vm.AllocatedAmount,
            AllocatedDate = vm.AllocatedDate,
            Notes = vm.Notes?.Trim()
        });

        TempData["Success"] = "تم ربط اتفاقية التمويل بالمشروع";
        return RedirectToAction(nameof(Index), new { projectId = vm.ProjectId });
    }
    [Authorize(Policy = CharityPolicies.ProjectsManage)]
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _projectGrantRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        if (!await PopulateProjectAsync(entity.ProjectId))
            return NotFound();

        var vm = new EditProjectGrantVm
        {
            Id = entity.Id,
            ProjectId = entity.ProjectId,
            GrantAgreementId = entity.GrantAgreementId,
            AllocatedAmount = entity.AllocatedAmount,
            AllocatedDate = entity.AllocatedDate,
            Notes = entity.Notes
        };

        await FillLookupsAsync(vm);
        return View(vm);
    }
    [Authorize(Policy = CharityPolicies.ProjectsManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditProjectGrantVm vm)
    {
        var entity = await _projectGrantRepository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        if (!await PopulateProjectAsync(entity.ProjectId))
            return NotFound();

        await FillLookupsAsync(vm);

        if (!ModelState.IsValid)
            return View(vm);

        if (entity.GrantAgreementId != vm.GrantAgreementId && await _projectGrantRepository.ExistsAsync(entity.ProjectId, vm.GrantAgreementId))
        {
            ModelState.AddModelError(nameof(vm.GrantAgreementId), "الاتفاقية مربوطة بالفعل بهذا المشروع");
            return View(vm);
        }

        entity.GrantAgreementId = vm.GrantAgreementId;
        entity.AllocatedAmount = vm.AllocatedAmount;
        entity.AllocatedDate = vm.AllocatedDate;
        entity.Notes = vm.Notes?.Trim();

        await _projectGrantRepository.UpdateAsync(entity);
        TempData["Success"] = "تم تعديل ربط التمويل بالمشروع";
        return RedirectToAction(nameof(Index), new { projectId = entity.ProjectId });
    }

    private async Task FillLookupsAsync(CreateProjectGrantVm vm)
    {
        var funders = await _grantAgreementRepository.GetAllAsync();
        vm.GrantAgreements = funders
            .OrderBy(x => x.AgreementNumber)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.AgreementNumber} - {x.Title}"
            }).ToList();
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
