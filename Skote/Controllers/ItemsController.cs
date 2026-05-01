using InfrastrfuctureManagmentCore.Domains.Item;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastructureManagmentWebFramework.Models.FinancialAccounting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class ItemsController : Controller
{
    private readonly IItemRepository _itemRepository;
    private readonly IItemGroupRepository _itemGroupRepository;
    private readonly IUnitRepository _unitRepository;

    public ItemsController(
        IItemRepository itemRepository,
        IItemGroupRepository itemGroupRepository,
        IUnitRepository unitRepository)
    {
        _itemRepository = itemRepository;
        _itemGroupRepository = itemGroupRepository;
        _unitRepository = unitRepository;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _itemRepository.GetAllAsync();

        var model = items.Select(x => new ItemListItemVm
        {
            Id = x.Id,
            ItemCode = x.ItemCode,
            ItemNameAr = x.ItemNameAr,
            Barcode = x.Barcode,
            GroupName = x.ItemGroup != null ? x.ItemGroup.GroupNameAr : "-",
            UnitName = x.Unit != null ? x.Unit.UnitNameAr : "-",
            PurchasePrice = x.PurchasePrice,
            SalePrice = x.SalePrice,
            IsService = x.IsService,
            IsStockItem = x.IsStockItem,
            IsTaxable = x.IsTaxable,
            TaxRate = x.TaxRate,
            IsActive = x.IsActive,
            OpticalItemTypeText = x.OpticalItemType switch
            {
                OpticalItemType.Frame => "Frame",
                OpticalItemType.Lens => "Lens",
                OpticalItemType.ContactLens => "Contact Lens",
                OpticalItemType.Accessory => "Accessory",
                OpticalItemType.Service => "Service",
                _ => "General"
            },
            BrandName = x.BrandName,
            ModelName = x.ModelName,
            Color = x.Color,
            RequiresPrescription = x.RequiresPrescription,
        }).ToList();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new CreateItemVm();
        await FillLookups(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateItemVm vm)
    {
        await FillLookups(vm);

        if (!ModelState.IsValid)
            return View(vm);

        if (await _itemRepository.CodeExistsAsync(vm.ItemCode.Trim()))
        {
            ModelState.AddModelError(nameof(vm.ItemCode), "كود الصنف موجود بالفعل");
            return View(vm);
        }

        if (!string.IsNullOrWhiteSpace(vm.Barcode) &&
            await _itemRepository.BarcodeExistsAsync(vm.Barcode.Trim()))
        {
            ModelState.AddModelError(nameof(vm.Barcode), "الباركود موجود بالفعل");
            return View(vm);
        }

        if (vm.IsService)
            vm.IsStockItem = false;

        var entity = new Item
        {
            Id = Guid.NewGuid(),
            ItemCode = vm.ItemCode.Trim(),
            ItemNameAr = vm.ItemNameAr.Trim(),
            ItemNameEn = vm.ItemNameEn?.Trim(),
            Barcode = string.IsNullOrWhiteSpace(vm.Barcode) ? null : vm.Barcode.Trim(),
            ItemGroupId = vm.ItemGroupId,
            UnitId = vm.UnitId,
            IsService = vm.IsService,
            IsStockItem = vm.IsStockItem,
            IsActive = true,
            PurchasePrice = vm.PurchasePrice,
            SalePrice = vm.SalePrice,
            MinimumQuantity = vm.MinimumQuantity,
            ReorderQuantity = vm.ReorderQuantity,
            IsTaxable = vm.IsTaxable,
            TaxRate = vm.IsTaxable ? vm.TaxRate : 0,
            Description = vm.Description?.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            OpticalItemType = vm.OpticalItemType,
            BrandName = vm.BrandName?.Trim(),
            ModelName = vm.ModelName?.Trim(),
            Color = vm.Color?.Trim(),

            EyeSize = vm.EyeSize,
            BridgeSize = vm.BridgeSize,
            TempleLength = vm.TempleLength,

            LensMaterial = vm.LensMaterial?.Trim(),
            LensIndex = vm.LensIndex?.Trim(),
            LensCoating = vm.LensCoating?.Trim(),

            RequiresPrescription = vm.RequiresPrescription,
        };

        await _itemRepository.AddAsync(entity);

        TempData["Success"] = "تم إضافة الصنف بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _itemRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var vm = new EditItemVm
        {
            Id = entity.Id,
            ItemCode = entity.ItemCode,
            ItemNameAr = entity.ItemNameAr,
            ItemNameEn = entity.ItemNameEn,
            Barcode = entity.Barcode,
            ItemGroupId = entity.ItemGroupId,
            UnitId = entity.UnitId,
            IsService = entity.IsService,
            IsStockItem = entity.IsStockItem,
            IsActive = entity.IsActive,
            PurchasePrice = entity.PurchasePrice,
            SalePrice = entity.SalePrice,
            MinimumQuantity = entity.MinimumQuantity,
            ReorderQuantity = entity.ReorderQuantity,
            IsTaxable = entity.IsTaxable,
            TaxRate = entity.TaxRate,
            Description = entity.Description,
            OpticalItemType = entity.OpticalItemType,
            BrandName = entity.BrandName,
            ModelName = entity.ModelName,
            Color = entity.Color,

            EyeSize = entity.EyeSize,
            BridgeSize = entity.BridgeSize,
            TempleLength = entity.TempleLength,

            LensMaterial = entity.LensMaterial,
            LensIndex = entity.LensIndex,
            LensCoating = entity.LensCoating,

            RequiresPrescription = entity.RequiresPrescription,
        };

        await FillLookups(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditItemVm vm)
    {
        await FillLookups(vm);

        if (!ModelState.IsValid)
            return View(vm);

        var entity = await _itemRepository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        if (await _itemRepository.CodeExistsAsync(vm.ItemCode.Trim(), vm.Id))
        {
            ModelState.AddModelError(nameof(vm.ItemCode), "كود الصنف موجود بالفعل");
            return View(vm);
        }

        if (!string.IsNullOrWhiteSpace(vm.Barcode) &&
            await _itemRepository.BarcodeExistsAsync(vm.Barcode.Trim(), vm.Id))
        {
            ModelState.AddModelError(nameof(vm.Barcode), "الباركود موجود بالفعل");
            return View(vm);
        }

        if (vm.IsService)
            vm.IsStockItem = false;

        entity.ItemCode = vm.ItemCode.Trim();
        entity.ItemNameAr = vm.ItemNameAr.Trim();
        entity.ItemNameEn = vm.ItemNameEn?.Trim();
        entity.Barcode = string.IsNullOrWhiteSpace(vm.Barcode) ? null : vm.Barcode.Trim();
        entity.ItemGroupId = vm.ItemGroupId;
        entity.UnitId = vm.UnitId;
        entity.IsService = vm.IsService;
        entity.IsStockItem = vm.IsStockItem;
        entity.IsActive = vm.IsActive;
        entity.PurchasePrice = vm.PurchasePrice;
        entity.SalePrice = vm.SalePrice;
        entity.MinimumQuantity = vm.MinimumQuantity;
        entity.ReorderQuantity = vm.ReorderQuantity;
        entity.IsTaxable = vm.IsTaxable;
        entity.TaxRate = vm.IsTaxable ? vm.TaxRate : 0;
        entity.Description = vm.Description?.Trim();
        entity.UpdatedAtUtc = DateTime.UtcNow;
        entity.OpticalItemType = vm.OpticalItemType;
        entity.BrandName = vm.BrandName?.Trim();
        entity.ModelName = vm.ModelName?.Trim();
        entity.Color = vm.Color?.Trim();

        entity.EyeSize = vm.EyeSize;
        entity.BridgeSize = vm.BridgeSize;
        entity.TempleLength = vm.TempleLength;

        entity.LensMaterial = vm.LensMaterial?.Trim();
        entity.LensIndex = vm.LensIndex?.Trim();
        entity.LensCoating = vm.LensCoating?.Trim();

        entity.RequiresPrescription = vm.RequiresPrescription;

        await _itemRepository.UpdateAsync(entity);

        TempData["Success"] = "تم تعديل الصنف بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var entity = await _itemRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        entity.IsActive = !entity.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _itemRepository.UpdateAsync(entity);

        TempData["Success"] = entity.IsActive
            ? "تم تفعيل الصنف بنجاح"
            : "تم تعطيل الصنف بنجاح";

        return RedirectToAction(nameof(Index));
    }

    private async Task FillLookups(CreateItemVm vm)
    {
        var groups = await _itemGroupRepository.GetActiveAsync();
        var units = await _unitRepository.GetActiveAsync();

        vm.ItemGroups = groups.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.GroupCode} - {x.GroupNameAr}"
        }).ToList();

        vm.Units = units.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.UnitCode} - {x.UnitNameAr}"
        }).ToList();
    }

    private async Task FillLookups(EditItemVm vm)
    {
        var groups = await _itemGroupRepository.GetActiveAsync();
        var units = await _unitRepository.GetActiveAsync();

        vm.ItemGroups = groups.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.GroupCode} - {x.GroupNameAr}"
        }).ToList();

        vm.Units = units.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.UnitCode} - {x.UnitNameAr}"
        }).ToList();
    }
}