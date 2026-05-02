using InfrastrfuctureManagmentCore.Domains.HR;
using InfrastrfuctureManagmentCore.Persistence.Repositories.HR;
using InfrastructureManagmentWebFramework.Models.HR.JobTitles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skote.Helpers;
[Authorize(Policy = CharityPolicies.HrManage)]
public class HrJobTitlesController : Controller
{
    private readonly IHrJobTitleRepository _repository;
    public HrJobTitlesController(IHrJobTitleRepository repository) => _repository = repository;

    public async Task<IActionResult> Index()
    {
        var items = await _repository.GetAllAsync();
        return View(items.Select(x => new JobTitleListItemVm
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            IsActive = x.IsActive
        }).ToList());
    }
    [Authorize(Policy = CharityPolicies.HrView)]
    [HttpGet]
    public IActionResult Create() => View(new CreateJobTitleVm());
    [Authorize(Policy = CharityPolicies.HrView)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateJobTitleVm vm)
    {
        if (await _repository.NameExistsAsync(vm.Name))
            ModelState.AddModelError(nameof(vm.Name), "المسمى الوظيفي مستخدم من قبل.");

        if (!ModelState.IsValid)
            return View(vm);

        await _repository.AddAsync(new HrJobTitle
        {
            Id = Guid.NewGuid(),
            Name = vm.Name.Trim(),
            Description = vm.Description,
            IsActive = vm.IsActive
        });

        return RedirectToAction(nameof(Index));
    }
    [Authorize(Policy = CharityPolicies.HrView)]
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return NotFound();
        return View(new EditJobTitleVm
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            IsActive = entity.IsActive
        });
    }
    [Authorize(Policy = CharityPolicies.HrView)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditJobTitleVm vm)
    {
        var entity = await _repository.GetByIdAsync(vm.Id);
        if (entity == null) return NotFound();

        if (await _repository.NameExistsAsync(vm.Name, vm.Id))
            ModelState.AddModelError(nameof(vm.Name), "المسمى الوظيفي مستخدم من قبل.");

        if (!ModelState.IsValid)
            return View(vm);

        entity.Name = vm.Name.Trim();
        entity.Description = vm.Description;
        entity.IsActive = vm.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        await _repository.UpdateAsync(entity);
        return RedirectToAction(nameof(Index));
    }
}
