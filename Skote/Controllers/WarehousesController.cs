using InfrastrfuctureManagmentCore.Domains.Warehouses;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastructureManagmentWebFramework.Models.FinancialAccounting;
using Microsoft.AspNetCore.Mvc;

public class WarehousesController : Controller
{
    private readonly IWarehouseRepository _warehouseRepository;

    public WarehousesController(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }

    public async Task<IActionResult> Index()
    {
        var warehouses = await _warehouseRepository.GetAllAsync();

        var model = warehouses.Select(x => new WarehouseListItemVm
        {
            Id = x.Id,
            WarehouseCode = x.WarehouseCode,
            WarehouseNameAr = x.WarehouseNameAr,
            WarehouseNameEn = x.WarehouseNameEn,
            Address = x.Address,
            IsMain = x.IsMain,
            IsActive = x.IsActive
        }).ToList();

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateWarehouseVm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateWarehouseVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        if (await _warehouseRepository.CodeExistsAsync(vm.WarehouseCode.Trim()))
        {
            ModelState.AddModelError(nameof(vm.WarehouseCode), "كود المخزن موجود بالفعل");
            return View(vm);
        }

        var entity = new Warehouse
        {
            Id = Guid.NewGuid(),
            WarehouseCode = vm.WarehouseCode.Trim(),
            WarehouseNameAr = vm.WarehouseNameAr.Trim(),
            WarehouseNameEn = vm.WarehouseNameEn?.Trim(),
            Address = vm.Address?.Trim(),
            Notes = vm.Notes?.Trim(),
            IsMain = vm.IsMain,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _warehouseRepository.AddAsync(entity);

        TempData["Success"] = "تم إضافة المخزن بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _warehouseRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var vm = new EditWarehouseVm
        {
            Id = entity.Id,
            WarehouseCode = entity.WarehouseCode,
            WarehouseNameAr = entity.WarehouseNameAr,
            WarehouseNameEn = entity.WarehouseNameEn,
            Address = entity.Address,
            Notes = entity.Notes,
            IsMain = entity.IsMain,
            IsActive = entity.IsActive
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditWarehouseVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var entity = await _warehouseRepository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        if (await _warehouseRepository.CodeExistsAsync(vm.WarehouseCode.Trim(), vm.Id))
        {
            ModelState.AddModelError(nameof(vm.WarehouseCode), "كود المخزن موجود بالفعل");
            return View(vm);
        }

        entity.WarehouseCode = vm.WarehouseCode.Trim();
        entity.WarehouseNameAr = vm.WarehouseNameAr.Trim();
        entity.WarehouseNameEn = vm.WarehouseNameEn?.Trim();
        entity.Address = vm.Address?.Trim();
        entity.Notes = vm.Notes?.Trim();
        entity.IsMain = vm.IsMain;
        entity.IsActive = vm.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _warehouseRepository.UpdateAsync(entity);

        TempData["Success"] = "تم تعديل المخزن بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var entity = await _warehouseRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        entity.IsActive = !entity.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _warehouseRepository.UpdateAsync(entity);

        TempData["Success"] = entity.IsActive
            ? "تم تفعيل المخزن بنجاح"
            : "تم تعطيل المخزن بنجاح";

        return RedirectToAction(nameof(Index));
    }
}