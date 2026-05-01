using InfrastrfuctureManagmentCore.Domains.Item;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastructureManagmentWebFramework.Models.FinancialAccounting;
using Microsoft.AspNetCore.Mvc;

public class UnitsController : Controller
{
    private readonly IUnitRepository _unitRepository;

    public UnitsController(IUnitRepository unitRepository)
    {
        _unitRepository = unitRepository;
    }

    public async Task<IActionResult> Index()
    {
        var units = await _unitRepository.GetAllAsync();

        var model = units.Select(x => new UnitListItemVm
        {
            Id = x.Id,
            UnitCode = x.UnitCode,
            UnitNameAr = x.UnitNameAr,
            UnitNameEn = x.UnitNameEn,
            Symbol = x.Symbol,
            IsActive = x.IsActive
        }).ToList();

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateUnitVm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUnitVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        if (await _unitRepository.CodeExistsAsync(vm.UnitCode.Trim()))
        {
            ModelState.AddModelError(nameof(vm.UnitCode), "كود الوحدة موجود بالفعل");
            return View(vm);
        }

        var entity = new Unit
        {
            Id = Guid.NewGuid(),
            UnitCode = vm.UnitCode.Trim(),
            UnitNameAr = vm.UnitNameAr.Trim(),
            UnitNameEn = vm.UnitNameEn?.Trim(),
            Symbol = vm.Symbol?.Trim(),
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _unitRepository.AddAsync(entity);

        TempData["Success"] = "تم إضافة الوحدة بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _unitRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var vm = new EditUnitVm
        {
            Id = entity.Id,
            UnitCode = entity.UnitCode,
            UnitNameAr = entity.UnitNameAr,
            UnitNameEn = entity.UnitNameEn,
            Symbol = entity.Symbol,
            IsActive = entity.IsActive
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditUnitVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var entity = await _unitRepository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        if (await _unitRepository.CodeExistsAsync(vm.UnitCode.Trim(), vm.Id))
        {
            ModelState.AddModelError(nameof(vm.UnitCode), "كود الوحدة موجود بالفعل");
            return View(vm);
        }

        entity.UnitCode = vm.UnitCode.Trim();
        entity.UnitNameAr = vm.UnitNameAr.Trim();
        entity.UnitNameEn = vm.UnitNameEn?.Trim();
        entity.Symbol = vm.Symbol?.Trim();
        entity.IsActive = vm.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _unitRepository.UpdateAsync(entity);

        TempData["Success"] = "تم تعديل الوحدة بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var entity = await _unitRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        entity.IsActive = !entity.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _unitRepository.UpdateAsync(entity);

        TempData["Success"] = entity.IsActive
            ? "تم تفعيل الوحدة بنجاح"
            : "تم تعطيل الوحدة بنجاح";

        return RedirectToAction(nameof(Index));
    }
}