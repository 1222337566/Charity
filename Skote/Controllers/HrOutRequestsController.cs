using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.HR.Advanced.OutRequests;
using InfrastrfuctureManagmentCore.Domains.HR;
using InfrastrfuctureManagmentCore.Domains.HR.Advanced;
using InfrastrfuctureManagmentCore.Persistence.Repositories.HR.Advanced;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class HrOutRequestsController : Controller
{
    private readonly IHrOutRequestRepository _repository;
    private readonly AppDbContext _db;
    public HrOutRequestsController(IHrOutRequestRepository repository, AppDbContext db) { _repository = repository; _db = db; }

    public async Task<IActionResult> Index(Guid? employeeId, string? status)
    {
        var items = await _repository.GetAllAsync(employeeId, status);
        ViewBag.EmployeeId = employeeId;
        ViewBag.Status = status;
        return View(items.Select(x => new OutRequestListItemVm
        {
            Id = x.Id,
            EmployeeId = x.EmployeeId,
            EmployeeName = x.Employee?.FullName ?? "-",
            OutDate = x.OutDate,
            FromTime = x.FromTime,
            ToTime = x.ToTime,
            Status = x.Status,
            Reason = x.Reason
        }).ToList());
    }

    public async Task<IActionResult> Create(Guid? employeeId)
    {
        var vm = new CreateOutRequestVm { EmployeeId = employeeId ?? Guid.Empty };
        await FillLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateOutRequestVm vm)
    {
        await FillLookupsAsync(vm);
        if (!ModelState.IsValid) return View(vm);
        if (vm.ToTime <= vm.FromTime)
        {
            ModelState.AddModelError(nameof(vm.ToTime), "وقت العودة يجب أن يكون بعد وقت الخروج");
            return View(vm);
        }

        await _repository.AddAsync(new HrOutRequest
        {
            Id = Guid.NewGuid(),
            EmployeeId = vm.EmployeeId,
            OutDate = vm.OutDate,
            FromTime = vm.FromTime,
            ToTime = vm.ToTime,
            Reason = vm.Reason.Trim(),
            Status = vm.Status,
            Notes = vm.Notes?.Trim(),
            CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            CreatedAtUtc = DateTime.UtcNow,
            ApprovedByUserId = vm.Status == "Approved" ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null,
            ApprovedAtUtc = vm.Status == "Approved" ? DateTime.UtcNow : null
        });

        TempData["Success"] = "تم تسجيل طلب الخروج";
        return RedirectToAction(nameof(Index), new { employeeId = vm.EmployeeId });
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return NotFound();
        var vm = new EditOutRequestVm
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            OutDate = entity.OutDate,
            FromTime = entity.FromTime,
            ToTime = entity.ToTime,
            Reason = entity.Reason,
            Status = entity.Status,
            Notes = entity.Notes
        };
        await FillLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditOutRequestVm vm)
    {
        await FillLookupsAsync(vm);
        if (!ModelState.IsValid) return View(vm);
        if (vm.ToTime <= vm.FromTime)
        {
            ModelState.AddModelError(nameof(vm.ToTime), "وقت العودة يجب أن يكون بعد وقت الخروج");
            return View(vm);
        }

        var entity = await _repository.GetByIdAsync(vm.Id);
        if (entity == null) return NotFound();
        entity.EmployeeId = vm.EmployeeId;
        entity.OutDate = vm.OutDate;
        entity.FromTime = vm.FromTime;
        entity.ToTime = vm.ToTime;
        entity.Reason = vm.Reason.Trim();
        entity.Status = vm.Status;
        entity.Notes = vm.Notes?.Trim();
        entity.ApprovedByUserId = vm.Status == "Approved" ? User.FindFirstValue(ClaimTypes.NameIdentifier) : entity.ApprovedByUserId;
        entity.ApprovedAtUtc = vm.Status == "Approved" ? (entity.ApprovedAtUtc ?? DateTime.UtcNow) : entity.ApprovedAtUtc;
        await _repository.UpdateAsync(entity);

        TempData["Success"] = "تم تعديل طلب الخروج";
        return RedirectToAction(nameof(Index), new { employeeId = vm.EmployeeId });
    }

    private async Task FillLookupsAsync(CreateOutRequestVm vm)
    {
        vm.Employees = await _db.Set<HrEmployee>().AsNoTracking().OrderBy(x => x.FullName)
            .Select(x => new SelectListItem(x.FullName, x.Id.ToString())).ToListAsync();
        vm.Statuses = new()
        {
            new("قيد المراجعة", "Pending"), new("معتمد", "Approved"), new("مرفوض", "Rejected")
        };
    }
}
