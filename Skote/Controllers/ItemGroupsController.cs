using InfrastrfuctureManagmentCore.Domains.Item;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastructureManagmentWebFramework.Models.FinancialAccounting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class ItemGroupsController : Controller
{
    private readonly IItemGroupRepository _itemGroupRepository;

    public ItemGroupsController(IItemGroupRepository itemGroupRepository)
    {
        _itemGroupRepository = itemGroupRepository;
    }

    public async Task<IActionResult> Index()
    {
        var groups = await _itemGroupRepository.GetAllAsync();

        var model = groups.Select(x => new ItemGroupListItemVm
        {
            Id = x.Id,
            GroupCode = x.GroupCode,
            GroupNameAr = x.GroupNameAr,
            GroupNameEn = x.GroupNameEn,
            ParentGroupId = x.ParentGroupId,
            ParentGroupName = x.ParentGroup != null ? x.ParentGroup.GroupNameAr : null,
            IsActive = x.IsActive
        }).ToList();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new CreateItemGroupVm();
        await FillParents(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateItemGroupVm vm)
    {
        await FillParents(vm);

        if (!ModelState.IsValid)
            return View(vm);

        if (await _itemGroupRepository.CodeExistsAsync(vm.GroupCode.Trim()))
        {
            ModelState.AddModelError(nameof(vm.GroupCode), "كود المجموعة موجود بالفعل");
            return View(vm);
        }

        var entity = new ItemGroup
        {
            Id = Guid.NewGuid(),
            GroupCode = vm.GroupCode.Trim(),
            GroupNameAr = vm.GroupNameAr.Trim(),
            GroupNameEn = vm.GroupNameEn?.Trim(),
            ParentGroupId = vm.ParentGroupId,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _itemGroupRepository.AddAsync(entity);

        TempData["Success"] = "تم إضافة المجموعة بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _itemGroupRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var vm = new EditItemGroupVm
        {
            Id = entity.Id,
            GroupCode = entity.GroupCode,
            GroupNameAr = entity.GroupNameAr,
            GroupNameEn = entity.GroupNameEn,
            ParentGroupId = entity.ParentGroupId,
            IsActive = entity.IsActive
        };

        await FillParents(vm, entity.Id);

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditItemGroupVm vm)
    {
        await FillParents(vm, vm.Id);

        if (!ModelState.IsValid)
            return View(vm);

        var entity = await _itemGroupRepository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        if (await _itemGroupRepository.CodeExistsAsync(vm.GroupCode.Trim(), vm.Id))
        {
            ModelState.AddModelError(nameof(vm.GroupCode), "كود المجموعة موجود بالفعل");
            return View(vm);
        }

        if (vm.ParentGroupId == vm.Id)
        {
            ModelState.AddModelError(nameof(vm.ParentGroupId), "لا يمكن اختيار نفس المجموعة كأب");
            return View(vm);
        }

        entity.GroupCode = vm.GroupCode.Trim();
        entity.GroupNameAr = vm.GroupNameAr.Trim();
        entity.GroupNameEn = vm.GroupNameEn?.Trim();
        entity.ParentGroupId = vm.ParentGroupId;
        entity.IsActive = vm.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _itemGroupRepository.UpdateAsync(entity);

        TempData["Success"] = "تم تعديل المجموعة بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var entity = await _itemGroupRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        if (entity.IsActive)
        {
            var hasChildren = await _itemGroupRepository.HasActiveChildrenAsync(entity.Id);
            if (hasChildren)
            {
                TempData["Error"] = "لا يمكن تعطيل المجموعة لأنها تحتوي على مجموعات فرعية نشطة";
                return RedirectToAction(nameof(Index));
            }
        }

        entity.IsActive = !entity.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _itemGroupRepository.UpdateAsync(entity);

        TempData["Success"] = entity.IsActive
            ? "تم تفعيل المجموعة بنجاح"
            : "تم تعطيل المجموعة بنجاح";

        return RedirectToAction(nameof(Index));
    }

    private async Task FillParents(CreateItemGroupVm vm)
    {
        var parents = await _itemGroupRepository.GetActiveAsync();

        vm.ParentGroups = parents.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.GroupCode} - {x.GroupNameAr}"
        }).ToList();
    }

    private async Task FillParents(EditItemGroupVm vm, Guid currentId)
    {
        var parents = await _itemGroupRepository.GetActiveAsync();

        vm.ParentGroups = parents
            .Where(x => x.Id != currentId)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.GroupCode} - {x.GroupNameAr}"
            }).ToList();
    }
}