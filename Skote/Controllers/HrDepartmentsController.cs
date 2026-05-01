using InfrastrfuctureManagmentCore.Domains.HR;
using InfrastrfuctureManagmentCore.Persistence.Repositories.HR;
using InfrastructureManagmentWebFramework.Models.HR.Departments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skote.Helpers;
[Authorize(Policy = CharityPolicies.HrManage)]
public class HrDepartmentsController : Controller
{
    private readonly IHrDepartmentRepository _repository;
    public HrDepartmentsController(IHrDepartmentRepository repository) => _repository = repository;

    public async Task<IActionResult> Index()
    {
        var items = await _repository.GetAllAsync();
        return View(items.Select(x => new DepartmentListItemVm
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            IsActive = x.IsActive
        }).ToList());
    }
    [Authorize(Policy = CharityPolicies.HrView)]
    [HttpGet]
    public IActionResult Create() => View(new CreateDepartmentVm());
    [Authorize(Policy = CharityPolicies.HrView)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDepartmentVm vm)
    {
        if (await _repository.NameExistsAsync(vm.Name))
            ModelState.AddModelError(nameof(vm.Name), "اسم القسم مستخدم من قبل.");

        if (!ModelState.IsValid)
            return View(vm);

        await _repository.AddAsync(new HrDepartment
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
        return View(new EditDepartmentVm
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
    public async Task<IActionResult> Edit(EditDepartmentVm vm)
    {
        var entity = await _repository.GetByIdAsync(vm.Id);
        if (entity == null) return NotFound();

        if (await _repository.NameExistsAsync(vm.Name, vm.Id))
            ModelState.AddModelError(nameof(vm.Name), "اسم القسم مستخدم من قبل.");

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
