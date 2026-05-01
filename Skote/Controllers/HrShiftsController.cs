using InfrastrfuctureManagmentCore.Domains.HR;
using InfrastrfuctureManagmentCore.Persistence.Repositories.HR;
using InfrastructureManagmentWebFramework.Models.HR.Shifts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skote.Helpers;
[Authorize(Policy = CharityPolicies.HrManage)]
public class HrShiftsController : Controller
{
    private readonly IHrShiftRepository _repository;
    public HrShiftsController(IHrShiftRepository repository) => _repository = repository;

    public async Task<IActionResult> Index()
    {
        var items = await _repository.GetAllAsync();
        return View(items.Select(x => new ShiftListItemVm
        {
            Id = x.Id,
            Name = x.Name,
            StartTime = x.StartTime,
            EndTime = x.EndTime,
            GraceMinutes = x.GraceMinutes,
            IsActive = x.IsActive
        }).ToList());
    }
    [Authorize(Policy = CharityPolicies.HrView)]
    [HttpGet]
    public IActionResult Create() => View(new CreateShiftVm());

    [Authorize(Policy = CharityPolicies.HrView)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateShiftVm vm)
    {
        if (await _repository.NameExistsAsync(vm.Name))
            ModelState.AddModelError(nameof(vm.Name), "اسم الشيفت مستخدم من قبل.");
        if (!ModelState.IsValid)
            return View(vm);

        await _repository.AddAsync(new HrShift
        {
            Id = Guid.NewGuid(),
            Name = vm.Name.Trim(),
            StartTime = vm.StartTime,
            EndTime = vm.EndTime,
            GraceMinutes = vm.GraceMinutes,
            Notes = vm.Notes,
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
        return View(new EditShiftVm
        {
            Id = entity.Id,
            Name = entity.Name,
            StartTime = entity.StartTime,
            EndTime = entity.EndTime,
            GraceMinutes = entity.GraceMinutes,
            Notes = entity.Notes,
            IsActive = entity.IsActive
        });
    }
    [Authorize(Policy = CharityPolicies.HrView)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditShiftVm vm)
    {
        var entity = await _repository.GetByIdAsync(vm.Id);
        if (entity == null) return NotFound();
        if (await _repository.NameExistsAsync(vm.Name, vm.Id))
            ModelState.AddModelError(nameof(vm.Name), "اسم الشيفت مستخدم من قبل.");
        if (!ModelState.IsValid)
            return View(vm);

        entity.Name = vm.Name.Trim();
        entity.StartTime = vm.StartTime;
        entity.EndTime = vm.EndTime;
        entity.GraceMinutes = vm.GraceMinutes;
        entity.Notes = vm.Notes;
        entity.IsActive = vm.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        await _repository.UpdateAsync(entity);
        return RedirectToAction(nameof(Index));
    }
}
