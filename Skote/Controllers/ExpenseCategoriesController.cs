using InfrastrfuctureManagmentCore.Domains.Expenses;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Expenses;
using InfrastructureManagmentWebFramework.Models.Expenses;
using Microsoft.AspNetCore.Mvc;

public class ExpenseCategoriesController : Controller
{
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;

    public ExpenseCategoriesController(IExpenseCategoryRepository expenseCategoryRepository)
    {
        _expenseCategoryRepository = expenseCategoryRepository;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _expenseCategoryRepository.GetAllAsync();

        var model = categories.Select(x => new ExpenseCategoryListItemVm
        {
            Id = x.Id,
            CategoryCode = x.CategoryCode,
            CategoryNameAr = x.NameAr,
            IsActive = x.IsActive
        }).ToList();

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateExpenseCategoryVm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateExpenseCategoryVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        if (await _expenseCategoryRepository.CodeExistsAsync(vm.CategoryCode.Trim()))
        {
            ModelState.AddModelError(nameof(vm.CategoryCode), "كود التصنيف موجود بالفعل");
            return View(vm);
        }

        var entity = new ExpenseCategory
        {
            Id = Guid.NewGuid(),
            CategoryCode = vm.CategoryCode.Trim(),
            NameAr = vm.CategoryNameAr.Trim(),
            CategoryNameEn = vm.CategoryNameEn?.Trim(),
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _expenseCategoryRepository.AddAsync(entity);

        TempData["Success"] = "تم إضافة تصنيف المصروف بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _expenseCategoryRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var vm = new EditExpenseCategoryVm
        {
            Id = entity.Id,
            CategoryCode = entity.CategoryCode,
            CategoryNameAr = entity.NameAr,
            CategoryNameEn = entity.CategoryNameEn,
            IsActive = entity.IsActive
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditExpenseCategoryVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var entity = await _expenseCategoryRepository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        if (await _expenseCategoryRepository.CodeExistsAsync(vm.CategoryCode.Trim(), vm.Id))
        {
            ModelState.AddModelError(nameof(vm.CategoryCode), "كود التصنيف موجود بالفعل");
            return View(vm);
        }

        entity.CategoryCode = vm.CategoryCode.Trim();
        entity.NameAr = vm.CategoryNameAr.Trim();
        entity.CategoryNameEn = vm.CategoryNameEn?.Trim();
        entity.IsActive = vm.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _expenseCategoryRepository.UpdateAsync(entity);

        TempData["Success"] = "تم تعديل تصنيف المصروف بنجاح";
        return RedirectToAction(nameof(Index));
    }
}