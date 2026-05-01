using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastructureManagmentWebFramework.Models.Accounting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class CostCentersController : Controller
{
    private readonly ICostCenterRepository _costCenterRepository;

    public CostCentersController(ICostCenterRepository costCenterRepository)
    {
        _costCenterRepository = costCenterRepository;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _costCenterRepository.GetAllAsync();
        var model = items.Select(x => new CostCenterListItemVm
        {
            Id = x.Id,
            CostCenterCode = x.CostCenterCode,
            CostCenterNameAr = x.NameAr,
            ParentCostCenterId = x.ParentCostCenterId,
            Level = x.Level,
            IsProjectRelated = x.IsProjectRelated,
            IsActive = x.IsActive
        }).ToList();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new CreateCostCenterVm();
        await FillParents(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCostCenterVm vm)
    {
        await FillParents(vm);

        if (!ModelState.IsValid)
            return View(vm);

        if (await _costCenterRepository.CodeExistsAsync(vm.CostCenterCode.Trim()))
        {
            ModelState.AddModelError(nameof(vm.CostCenterCode), "كود المركز موجود بالفعل");
            return View(vm);
        }

        int level = 1;
        Guid? parentId = null;
        if (vm.ParentCostCenterId.HasValue)
        {
            var parent = await _costCenterRepository.GetByIdAsync(vm.ParentCostCenterId.Value);
            if (parent == null)
            {
                ModelState.AddModelError(nameof(vm.ParentCostCenterId), "المركز الأب غير موجود");
                return View(vm);
            }
            parentId = parent.Id;
            level = parent.Level + 1;
        }

        var entity = new CostCenter
        {
            Id = Guid.NewGuid(),
            CostCenterCode = vm.CostCenterCode.Trim(),
            NameAr = vm.CostCenterNameAr.Trim(),
            CostCenterNameEn = vm.CostCenterNameEn?.Trim(),
            ParentCostCenterId = parentId,
            Level = level,
            IsProjectRelated = vm.IsProjectRelated,
            Notes = vm.Notes?.Trim(),
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _costCenterRepository.AddAsync(entity);
        TempData["Success"] = "تم إضافة مركز التكلفة بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _costCenterRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var vm = new EditCostCenterVm
        {
            Id = entity.Id,
            CostCenterCode = entity.CostCenterCode,
            CostCenterNameAr = entity.NameAr,
            CostCenterNameEn = entity.CostCenterNameEn,
            ParentCostCenterId = entity.ParentCostCenterId,
            IsProjectRelated = entity.IsProjectRelated,
            IsActive = entity.IsActive,
            Notes = entity.Notes
        };

        await FillParentsForEdit(vm, id);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditCostCenterVm vm)
    {
        await FillParentsForEdit(vm, vm.Id);

        if (!ModelState.IsValid)
            return View(vm);

        var entity = await _costCenterRepository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        if (await _costCenterRepository.CodeExistsAsync(vm.CostCenterCode.Trim(), vm.Id))
        {
            ModelState.AddModelError(nameof(vm.CostCenterCode), "كود المركز موجود بالفعل");
            return View(vm);
        }

        int level = 1;
        Guid? parentId = null;
        if (vm.ParentCostCenterId.HasValue)
        {
            if (vm.ParentCostCenterId.Value == vm.Id)
            {
                ModelState.AddModelError(nameof(vm.ParentCostCenterId), "لا يمكن اختيار نفس المركز كأب");
                return View(vm);
            }

            var parent = await _costCenterRepository.GetByIdAsync(vm.ParentCostCenterId.Value);
            if (parent == null)
            {
                ModelState.AddModelError(nameof(vm.ParentCostCenterId), "المركز الأب غير موجود");
                return View(vm);
            }
            parentId = parent.Id;
            level = parent.Level + 1;
        }

        if (entity.IsActive && !vm.IsActive)
        {
            var hasActiveChildren = await _costCenterRepository.HasActiveChildrenAsync(entity.Id);
            if (hasActiveChildren)
            {
                ModelState.AddModelError(nameof(vm.IsActive), "لا يمكن تعطيل المركز لأنه يحتوي على مراكز فرعية نشطة");
                return View(vm);
            }
        }

        entity.CostCenterCode = vm.CostCenterCode.Trim();
        entity.NameAr = vm.CostCenterNameAr.Trim();
        entity.CostCenterNameEn = vm.CostCenterNameEn?.Trim();
        entity.ParentCostCenterId = parentId;
        entity.Level = level;
        entity.IsProjectRelated = vm.IsProjectRelated;
        entity.IsActive = vm.IsActive;
        entity.Notes = vm.Notes?.Trim();
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _costCenterRepository.UpdateAsync(entity);
        TempData["Success"] = "تم تعديل مركز التكلفة بنجاح";
        return RedirectToAction(nameof(Index));
    }

    private async Task FillParents(CreateCostCenterVm vm)
    {
        var parents = await _costCenterRepository.GetAllParentsAsync();
        vm.ParentCostCenters = parents
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.CostCenterCode} - {x.NameAr}"
            })
            .ToList();
    }

    private async Task FillParentsForEdit(EditCostCenterVm vm, Guid currentId)
    {
        var parents = await _costCenterRepository.GetAllParentsAsync();
        vm.ParentCostCenters = parents
            .Where(x => x.Id != currentId)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.CostCenterCode} - {x.NameAr}"
            })
            .ToList();
    }
}
