using InfrastrfuctureManagmentCore.Domains.Payroll;
using InfrastrfuctureManagmentCore.Persistence.Repositories.HR;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Payroll;
using InfrastructureManagmentWebFramework.Models.HR.EmployeeProfile;
using InfrastructureManagmentWebFramework.Models.Payroll.EmployeeStructures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class EmployeeSalaryStructuresController : Controller
{
    private readonly IEmployeeSalaryStructureRepository _repository;
    private readonly IHrEmployeeRepository _employeeRepository;
    private readonly ISalaryItemDefinitionRepository _salaryItemRepository;

    public EmployeeSalaryStructuresController(
        IEmployeeSalaryStructureRepository repository,
        IHrEmployeeRepository employeeRepository,
        ISalaryItemDefinitionRepository salaryItemRepository)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
        _salaryItemRepository = salaryItemRepository;
    }

    public async Task<IActionResult> Index(Guid employeeId)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        if (employee == null) return NotFound();
        FillEmployeeHeader(employee);

        var items = await _repository.GetByEmployeeIdAsync(employeeId);
        return View(items.Select(x => new EmployeeSalaryStructureListItemVm
        {
            Id = x.Id,
            EmployeeId = x.EmployeeId,
            SalaryItemName = x.SalaryItemDefinition?.Name ?? string.Empty,
            ItemType = x.SalaryItemDefinition?.ItemType ?? string.Empty,
            Value = x.Value,
            FromDate = x.FromDate,
            ToDate = x.ToDate,
            IsActive = x.IsActive
        }).ToList());
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid employeeId)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        if (employee == null) return NotFound();
        FillEmployeeHeader(employee);
        var vm = new CreateEmployeeSalaryStructureVm { EmployeeId = employeeId };
        await FillLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEmployeeSalaryStructureVm vm)
    {
        var employee = await _employeeRepository.GetByIdAsync(vm.EmployeeId);
        if (employee == null) return NotFound();
        FillEmployeeHeader(employee);
        await FillLookupsAsync(vm);
        if (!ModelState.IsValid) return View(vm);

        await _repository.AddAsync(new EmployeeSalaryStructure
        {
            Id = Guid.NewGuid(),
            EmployeeId = vm.EmployeeId,
            SalaryItemDefinitionId = vm.SalaryItemDefinitionId,
            Value = vm.Value,
            FromDate = vm.FromDate,
            ToDate = vm.ToDate,
            IsActive = vm.IsActive,
            Notes = vm.Notes
        });
        return RedirectToAction(nameof(Index), new { employeeId = vm.EmployeeId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return NotFound();
        var employee = await _employeeRepository.GetByIdAsync(entity.EmployeeId);
        if (employee == null) return NotFound();
        FillEmployeeHeader(employee);
        var vm = new EditEmployeeSalaryStructureVm
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            SalaryItemDefinitionId = entity.SalaryItemDefinitionId,
            Value = entity.Value,
            FromDate = entity.FromDate,
            ToDate = entity.ToDate,
            IsActive = entity.IsActive,
            Notes = entity.Notes
        };
        await FillLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditEmployeeSalaryStructureVm vm)
    {
        var entity = await _repository.GetByIdAsync(vm.Id);
        if (entity == null) return NotFound();
        var employee = await _employeeRepository.GetByIdAsync(vm.EmployeeId);
        if (employee == null) return NotFound();
        FillEmployeeHeader(employee);
        await FillLookupsAsync(vm);
        if (!ModelState.IsValid) return View(vm);

        entity.SalaryItemDefinitionId = vm.SalaryItemDefinitionId;
        entity.Value = vm.Value;
        entity.FromDate = vm.FromDate;
        entity.ToDate = vm.ToDate;
        entity.IsActive = vm.IsActive;
        entity.Notes = vm.Notes;
        await _repository.UpdateAsync(entity);
        return RedirectToAction(nameof(Index), new { employeeId = vm.EmployeeId });
    }

    private async Task FillLookupsAsync(CreateEmployeeSalaryStructureVm vm)
    {
        var items = await _salaryItemRepository.GetActiveAsync();
        vm.SalaryItems = items.Select(x => new SelectListItem($"{x.Name} ({(x.ItemType == "Addition" ? "إضافة" : "خصم")})", x.Id.ToString())).ToList();
    }

    private void FillEmployeeHeader(InfrastrfuctureManagmentCore.Domains.HR.HrEmployee employee)
    {
        ViewBag.EmployeeHeader = new EmployeeHeaderVm
        {
            Id = employee.Id,
            Code = employee.Code,
            FullName = employee.FullName,
            DepartmentName = employee.Department?.Name,
            JobTitleName = employee.JobTitle?.Name,
            Status = employee.Status,
            PhoneNumber = employee.PhoneNumber,
            HireDate = employee.HireDate,
            BasicSalary = employee.BasicSalary,
            IsActive = employee.IsActive
        };
    }
}
