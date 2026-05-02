using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.HR.Advanced.Sanctions;
using InfrastrfuctureManagmentCore.Domains.HR;
using InfrastrfuctureManagmentCore.Domains.HR.Advanced;
using InfrastrfuctureManagmentCore.Persistence.Repositories.HR.Advanced;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class HrSanctionRecordsController : Controller
{
    private readonly IHrSanctionRecordRepository _repository;
    private readonly AppDbContext _db;
    public HrSanctionRecordsController(IHrSanctionRecordRepository repository, AppDbContext db) { _repository = repository; _db = db; }

    public async Task<IActionResult> Index(Guid? employeeId)
    {
        var items = await _repository.GetAllAsync(employeeId);
        ViewBag.EmployeeId = employeeId;
        return View(items.Select(x => new SanctionRecordListItemVm
        {
            Id = x.Id,
            EmployeeId = x.EmployeeId,
            EmployeeName = x.Employee?.FullName ?? "-",
            SanctionType = x.SanctionType,
            SanctionDate = x.SanctionDate,
            Amount = x.Amount,
            Reason = x.Reason
        }).ToList());
    }

    public async Task<IActionResult> Create(Guid? employeeId)
    {
        var vm = new CreateSanctionRecordVm { EmployeeId = employeeId ?? Guid.Empty };
        await FillLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSanctionRecordVm vm)
    {
        await FillLookupsAsync(vm);
        if (!ModelState.IsValid) return View(vm);

        await _repository.AddAsync(new HrSanctionRecord
        {
            Id = Guid.NewGuid(),
            EmployeeId = vm.EmployeeId,
            SanctionType = vm.SanctionType,
            SanctionDate = vm.SanctionDate,
            Amount = vm.Amount,
            Reason = vm.Reason.Trim(),
            Notes = vm.Notes?.Trim(),
            CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            CreatedAtUtc = DateTime.UtcNow
        });

        TempData["Success"] = "تم تسجيل الجزاء";
        return RedirectToAction(nameof(Index), new { employeeId = vm.EmployeeId });
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return NotFound();
        var vm = new EditSanctionRecordVm
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            SanctionType = entity.SanctionType,
            SanctionDate = entity.SanctionDate,
            Amount = entity.Amount,
            Reason = entity.Reason,
            Notes = entity.Notes
        };
        await FillLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditSanctionRecordVm vm)
    {
        await FillLookupsAsync(vm);
        if (!ModelState.IsValid) return View(vm);

        var entity = await _repository.GetByIdAsync(vm.Id);
        if (entity == null) return NotFound();
        entity.EmployeeId = vm.EmployeeId;
        entity.SanctionType = vm.SanctionType;
        entity.SanctionDate = vm.SanctionDate;
        entity.Amount = vm.Amount;
        entity.Reason = vm.Reason.Trim();
        entity.Notes = vm.Notes?.Trim();
        await _repository.UpdateAsync(entity);

        TempData["Success"] = "تم تعديل الجزاء";
        return RedirectToAction(nameof(Index), new { employeeId = vm.EmployeeId });
    }

    private async Task FillLookupsAsync(CreateSanctionRecordVm vm)
    {
        vm.Employees = await _db.Set<HrEmployee>().AsNoTracking().OrderBy(x => x.FullName)
            .Select(x => new SelectListItem(x.FullName, x.Id.ToString())).ToListAsync();
        vm.SanctionTypes = new()
        {
            new("إنذار", "Warning"), new("خصم", "Deduction"), new("إيقاف", "Suspension"),
            new("تحقيق", "Investigation"), new("أخرى", "Other")
        };
    }
}
