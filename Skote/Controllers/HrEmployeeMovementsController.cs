using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.HR.Advanced.EmployeeMovements;
using InfrastrfuctureManagmentCore.Domains.HR;
using InfrastrfuctureManagmentCore.Domains.HR.Advanced;
using InfrastrfuctureManagmentCore.Persistence.Repositories.HR.Advanced;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class HrEmployeeMovementsController : Controller
{
    private readonly IHrEmployeeMovementRepository _repository;
    private readonly AppDbContext _db;
    public HrEmployeeMovementsController(IHrEmployeeMovementRepository repository, AppDbContext db)
    {
        _repository = repository;
        _db = db;
    }

    public async Task<IActionResult> Index(Guid? employeeId)
    {
        var items = await _repository.GetAllAsync(employeeId);
        ViewBag.EmployeeId = employeeId;
        return View(items.Select(x => new EmployeeMovementListItemVm
        {
            Id = x.Id,
            EmployeeId = x.EmployeeId,
            EmployeeName = x.Employee?.FullName ?? "-",
            MovementType = x.MovementType,
            EffectiveDate = x.EffectiveDate,
            FromDepartmentName = x.FromDepartment?.Name,
            ToDepartmentName = x.ToDepartment?.Name,
            FromJobTitleName = x.FromJobTitle?.Name,
            ToJobTitleName = x.ToJobTitle?.Name,
            DecisionNumber = x.DecisionNumber
        }).ToList());
    }

    public async Task<IActionResult> Create(Guid? employeeId)
    {
        var vm = new CreateEmployeeMovementVm { EmployeeId = employeeId ?? Guid.Empty };
        await FillLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEmployeeMovementVm vm)
    {
        await FillLookupsAsync(vm);
        if (!ModelState.IsValid) return View(vm);

        var entity = new HrEmployeeMovement
        {
            Id = Guid.NewGuid(),
            EmployeeId = vm.EmployeeId,
            MovementType = vm.MovementType,
            EffectiveDate = vm.EffectiveDate,
            FromDepartmentId = vm.FromDepartmentId,
            ToDepartmentId = vm.ToDepartmentId,
            FromJobTitleId = vm.FromJobTitleId,
            ToJobTitleId = vm.ToJobTitleId,
            DecisionNumber = vm.DecisionNumber?.Trim(),
            Notes = vm.Notes?.Trim(),
            CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            CreatedAtUtc = DateTime.UtcNow
        };

        await _repository.AddAsync(entity);
        TempData["Success"] = "تم تسجيل الحركة الوظيفية";
        return RedirectToAction(nameof(Index), new { employeeId = vm.EmployeeId });
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return NotFound();

        var vm = new EditEmployeeMovementVm
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            MovementType = entity.MovementType,
            EffectiveDate = entity.EffectiveDate,
            FromDepartmentId = entity.FromDepartmentId,
            ToDepartmentId = entity.ToDepartmentId,
            FromJobTitleId = entity.FromJobTitleId,
            ToJobTitleId = entity.ToJobTitleId,
            DecisionNumber = entity.DecisionNumber,
            Notes = entity.Notes
        };
        await FillLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditEmployeeMovementVm vm)
    {
        await FillLookupsAsync(vm);
        if (!ModelState.IsValid) return View(vm);

        var entity = await _repository.GetByIdAsync(vm.Id);
        if (entity == null) return NotFound();

        entity.EmployeeId = vm.EmployeeId;
        entity.MovementType = vm.MovementType;
        entity.EffectiveDate = vm.EffectiveDate;
        entity.FromDepartmentId = vm.FromDepartmentId;
        entity.ToDepartmentId = vm.ToDepartmentId;
        entity.FromJobTitleId = vm.FromJobTitleId;
        entity.ToJobTitleId = vm.ToJobTitleId;
        entity.DecisionNumber = vm.DecisionNumber?.Trim();
        entity.Notes = vm.Notes?.Trim();

        await _repository.UpdateAsync(entity);
        TempData["Success"] = "تم تعديل الحركة الوظيفية";
        return RedirectToAction(nameof(Index), new { employeeId = vm.EmployeeId });
    }

    private async Task FillLookupsAsync(CreateEmployeeMovementVm vm)
    {
        vm.Employees = await _db.Set<HrEmployee>().AsNoTracking()
            .OrderBy(x => x.FullName)
            .Select(x => new SelectListItem(x.FullName, x.Id.ToString()))
            .ToListAsync();

        vm.Departments = await _db.Set<HrDepartment>().AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
            .ToListAsync();

        vm.JobTitles = await _db.Set<HrJobTitle>().AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
            .ToListAsync();

        vm.MovementTypes = new()
        {
            new("نقل", "Transfer"), new("ترقية", "Promotion"), new("ندب", "Assignment"),
            new("تغيير حالة", "StatusChange"), new("إنهاء خدمة", "Termination")
        };
    }
}
