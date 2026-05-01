using InfrastrfuctureManagmentCore.Domains.Payroll;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Payroll;
using InfrastructureManagmentWebFramework.Models.Payroll.SalaryItems;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class SalaryItemDefinitionsController : Controller
{
    private readonly ISalaryItemDefinitionRepository _repository;
    public SalaryItemDefinitionsController(ISalaryItemDefinitionRepository repository) => _repository = repository;

    public async Task<IActionResult> Index()
    {
        var items = await _repository.GetAllAsync();
        return View(items.Select(x => new SalaryItemDefinitionListItemVm
        {
            Id = x.Id,
            Name = x.Name,
            ItemType = x.ItemType,
            CalculationMethod = x.CalculationMethod,
            DefaultValue = x.DefaultValue,
            IsActive = x.IsActive
        }).ToList());
    }

    [HttpGet]
    public IActionResult Create()
    {
        var vm = new CreateSalaryItemDefinitionVm();
        FillLookups(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSalaryItemDefinitionVm vm)
    {
        FillLookups(vm);
        if (await _repository.NameExistsAsync(vm.Name))
            ModelState.AddModelError(nameof(vm.Name), "الاسم مستخدم من قبل.");
        if (!ModelState.IsValid) return View(vm);

        await _repository.AddAsync(new SalaryItemDefinition
        {
            Id = Guid.NewGuid(),
            Name = vm.Name.Trim(),
            ItemType = vm.ItemType,
            CalculationMethod = vm.CalculationMethod,
            DefaultValue = vm.DefaultValue,
            IsTaxable = vm.IsTaxable,
            IsInsuranceIncluded = vm.IsInsuranceIncluded,
            IsActive = vm.IsActive,
            Notes = vm.Notes
        });
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return NotFound();
        var vm = new EditSalaryItemDefinitionVm
        {
            Id = entity.Id,
            Name = entity.Name,
            ItemType = entity.ItemType,
            CalculationMethod = entity.CalculationMethod,
            DefaultValue = entity.DefaultValue,
            IsTaxable = entity.IsTaxable,
            IsInsuranceIncluded = entity.IsInsuranceIncluded,
            IsActive = entity.IsActive,
            Notes = entity.Notes
        };
        FillLookups(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditSalaryItemDefinitionVm vm)
    {
        var entity = await _repository.GetByIdAsync(vm.Id);
        if (entity == null) return NotFound();
        FillLookups(vm);
        if (await _repository.NameExistsAsync(vm.Name, vm.Id))
            ModelState.AddModelError(nameof(vm.Name), "الاسم مستخدم من قبل.");
        if (!ModelState.IsValid) return View(vm);

        entity.Name = vm.Name.Trim();
        entity.ItemType = vm.ItemType;
        entity.CalculationMethod = vm.CalculationMethod;
        entity.DefaultValue = vm.DefaultValue;
        entity.IsTaxable = vm.IsTaxable;
        entity.IsInsuranceIncluded = vm.IsInsuranceIncluded;
        entity.IsActive = vm.IsActive;
        entity.Notes = vm.Notes;
        await _repository.UpdateAsync(entity);
        return RedirectToAction(nameof(Index));
    }

    private static void FillLookups(CreateSalaryItemDefinitionVm vm)
    {
        vm.ItemTypes = new List<SelectListItem>
        {
            new("إضافة", "Addition"),
            new("خصم", "Deduction")
        };
        vm.CalculationMethods = new List<SelectListItem>
        {
            new("قيمة ثابتة", "Fixed"),
            new("نسبة", "Percentage"),
            new("يدوي", "Manual")
        };
    }
}
