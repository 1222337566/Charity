using InfrastructureManagmentWebFramework.Models.Charity.Kafala;
using InfrastrfuctureManagmentCore.Domains.Charity.Kafala;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Kafala;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

public class KafalaSponsorsController : Controller
{
    private readonly IKafalaSponsorRepository _repository;
    public KafalaSponsorsController(IKafalaSponsorRepository repository) => _repository = repository;

    public async Task<IActionResult> Index()
    {
        var items = await _repository.GetAllAsync();
        var model = items.Select(x => new KafalaSponsorListItemVm
        {
            Id = x.Id,
            SponsorCode = x.SponsorCode,
            FullName = x.FullName,
            SponsorType = x.SponsorType,
            PhoneNumber = x.PhoneNumber,
            IsActive = x.IsActive,
            ActiveCasesCount = x.KafalaCases?.Count(c => c.Status == "Active") ?? 0
        }).ToList();
        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var vm = new CreateKafalaSponsorVm();
        FillLookups(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateKafalaSponsorVm vm)
    {
        FillLookups(vm);
        if (!ModelState.IsValid) return View(vm);
        if (await _repository.SponsorCodeExistsAsync(vm.SponsorCode))
        {
            ModelState.AddModelError(nameof(vm.SponsorCode), "كود الكفيل موجود بالفعل");
            return View(vm);
        }

        await _repository.AddAsync(new KafalaSponsor
        {
            Id = Guid.NewGuid(),
            SponsorCode = vm.SponsorCode.Trim(),
            FullName = vm.FullName.Trim(),
            SponsorType = vm.SponsorType,
            NationalIdOrTaxNo = vm.NationalIdOrTaxNo?.Trim(),
            PhoneNumber = vm.PhoneNumber?.Trim(),
            Email = vm.Email?.Trim(),
            AddressLine = vm.AddressLine?.Trim(),
            IsActive = vm.IsActive,
            Notes = vm.Notes?.Trim(),
            CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            CreatedAtUtc = DateTime.UtcNow
        });

        TempData["Success"] = "تم إضافة الكفيل بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return NotFound();
        var vm = new EditKafalaSponsorVm
        {
            Id = entity.Id,
            SponsorCode = entity.SponsorCode,
            FullName = entity.FullName,
            SponsorType = entity.SponsorType,
            NationalIdOrTaxNo = entity.NationalIdOrTaxNo,
            PhoneNumber = entity.PhoneNumber,
            Email = entity.Email,
            AddressLine = entity.AddressLine,
            IsActive = entity.IsActive,
            Notes = entity.Notes
        };
        FillLookups(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditKafalaSponsorVm vm)
    {
        FillLookups(vm);
        if (!ModelState.IsValid) return View(vm);
        var entity = await _repository.GetByIdAsync(vm.Id);
        if (entity == null) return NotFound();
        if (await _repository.SponsorCodeExistsAsync(vm.SponsorCode, vm.Id))
        {
            ModelState.AddModelError(nameof(vm.SponsorCode), "كود الكفيل موجود بالفعل");
            return View(vm);
        }
        entity.SponsorCode = vm.SponsorCode.Trim();
        entity.FullName = vm.FullName.Trim();
        entity.SponsorType = vm.SponsorType;
        entity.NationalIdOrTaxNo = vm.NationalIdOrTaxNo?.Trim();
        entity.PhoneNumber = vm.PhoneNumber?.Trim();
        entity.Email = vm.Email?.Trim();
        entity.AddressLine = vm.AddressLine?.Trim();
        entity.IsActive = vm.IsActive;
        entity.Notes = vm.Notes?.Trim();
        entity.UpdatedAtUtc = DateTime.UtcNow;
        await _repository.UpdateAsync(entity);
        TempData["Success"] = "تم تعديل بيانات الكفيل";
        return RedirectToAction(nameof(Index));
    }

    private void FillLookups(CreateKafalaSponsorVm vm)
    {
        vm.SponsorTypes = new List<SelectListItem>
        {
            new("فرد", "Individual"),
            new("شركة", "Company"),
            new("مؤسسة", "Institution")
        };
    }
}
