using InfrastructureManagmentWebFramework.Models.Charity.Volunteers;
using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Volunteers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

public class VolunteersController : Controller
{
    private readonly IVolunteerRepository _repository;
    public VolunteersController(IVolunteerRepository repository) => _repository = repository;

    public async Task<IActionResult> Index()
    {
        var items = await _repository.GetAllAsync();
        var model = items.Select(x => new VolunteerListItemVm
        {
            Id = x.Id,
            VolunteerCode = x.VolunteerCode,
            FullName = x.FullName,
            PhoneNumber = x.PhoneNumber,
            PreferredArea = x.PreferredArea,
            IsActive = x.IsActive,
            AssignmentsCount = x.Assignments?.Count ?? 0,
            TotalHours = x.HourLogs?.Sum(h => h.Hours) ?? 0
        }).ToList();

        return View(model);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return NotFound();

        var vm = new VolunteerListItemVm
        {
            Id = entity.Id,
            VolunteerCode = entity.VolunteerCode,
            FullName = entity.FullName,
            PhoneNumber = entity.PhoneNumber,
            PreferredArea = entity.PreferredArea,
            IsActive = entity.IsActive,
            AssignmentsCount = entity.Assignments?.Count ?? 0,
            TotalHours = entity.HourLogs?.Sum(x => x.Hours) ?? 0
        };

        ViewBag.Entity = entity;
        return View(vm);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var vm = new CreateVolunteerVm();
        FillLookups(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateVolunteerVm vm)
    {
        FillLookups(vm);
        if (!ModelState.IsValid) return View(vm);

        if (await _repository.VolunteerCodeExistsAsync(vm.VolunteerCode))
        {
            ModelState.AddModelError(nameof(vm.VolunteerCode), "كود المتطوع موجود بالفعل");
            return View(vm);
        }

        await _repository.AddAsync(new Volunteer
        {
            Id = Guid.NewGuid(),
            VolunteerCode = vm.VolunteerCode.Trim(),
            FullName = vm.FullName.Trim(),
            Qualification = vm.Qualification?.Trim(),
            AddressLine = vm.AddressLine?.Trim(),
            Nationality = vm.Nationality?.Trim(),
            NationalId = vm.NationalId?.Trim(),
            PhoneNumber = vm.PhoneNumber?.Trim(),
            Email = vm.Email?.Trim(),
            BirthDate = vm.BirthDate,
            Gender = vm.Gender,
            MaritalStatus = vm.MaritalStatus,
            PreferredArea = vm.PreferredArea?.Trim(),
            AvailabilityNotes = vm.AvailabilityNotes?.Trim(),
            IsActive = vm.IsActive,
            Notes = vm.Notes?.Trim(),
            CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            CreatedAtUtc = DateTime.UtcNow
        });

        TempData["Success"] = "تم إضافة المتطوع بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return NotFound();

        var vm = new EditVolunteerVm
        {
            Id = entity.Id,
            VolunteerCode = entity.VolunteerCode,
            FullName = entity.FullName,
            Qualification = entity.Qualification,
            AddressLine = entity.AddressLine,
            Nationality = entity.Nationality,
            NationalId = entity.NationalId,
            PhoneNumber = entity.PhoneNumber,
            Email = entity.Email,
            BirthDate = entity.BirthDate,
            Gender = entity.Gender,
            MaritalStatus = entity.MaritalStatus,
            PreferredArea = entity.PreferredArea,
            AvailabilityNotes = entity.AvailabilityNotes,
            IsActive = entity.IsActive,
            Notes = entity.Notes
        };
        FillLookups(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditVolunteerVm vm)
    {
        FillLookups(vm);
        if (!ModelState.IsValid) return View(vm);

        var entity = await _repository.GetByIdAsync(vm.Id);
        if (entity == null) return NotFound();

        if (await _repository.VolunteerCodeExistsAsync(vm.VolunteerCode, vm.Id))
        {
            ModelState.AddModelError(nameof(vm.VolunteerCode), "كود المتطوع موجود بالفعل");
            return View(vm);
        }

        entity.VolunteerCode = vm.VolunteerCode.Trim();
        entity.FullName = vm.FullName.Trim();
        entity.Qualification = vm.Qualification?.Trim();
        entity.AddressLine = vm.AddressLine?.Trim();
        entity.Nationality = vm.Nationality?.Trim();
        entity.NationalId = vm.NationalId?.Trim();
        entity.PhoneNumber = vm.PhoneNumber?.Trim();
        entity.Email = vm.Email?.Trim();
        entity.BirthDate = vm.BirthDate;
        entity.Gender = vm.Gender;
        entity.MaritalStatus = vm.MaritalStatus;
        entity.PreferredArea = vm.PreferredArea?.Trim();
        entity.AvailabilityNotes = vm.AvailabilityNotes?.Trim();
        entity.IsActive = vm.IsActive;
        entity.Notes = vm.Notes?.Trim();
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(entity);
        TempData["Success"] = "تم تعديل بيانات المتطوع";
        return RedirectToAction(nameof(Index));
    }

    private void FillLookups(CreateVolunteerVm vm)
    {
        vm.Genders = new()
        {
            new SelectListItem("ذكر", "Male"),
            new SelectListItem("أنثى", "Female")
        };

        vm.MaritalStatuses = new()
        {
            new SelectListItem("أعزب", "Single"),
            new SelectListItem("متزوج", "Married"),
            new SelectListItem("أخرى", "Other")
        };
    }
}
