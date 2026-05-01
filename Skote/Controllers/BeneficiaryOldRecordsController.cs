using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries;
using InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.OldRecords;
using InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class BeneficiaryOldRecordsController : Controller
{
    private readonly IBeneficiaryOldRecordRepository _repository;
    private readonly IBeneficiaryRepository _beneficiaryRepository;

    public BeneficiaryOldRecordsController(
        IBeneficiaryOldRecordRepository repository,
        IBeneficiaryRepository beneficiaryRepository)
    {
        _repository = repository;
        _beneficiaryRepository = beneficiaryRepository;
    }

    public async Task<IActionResult> Index(Guid beneficiaryId)
    {
        if (!await PopulateBeneficiaryAsync(beneficiaryId))
            return NotFound();

        var items = await _repository.GetByBeneficiaryIdAsync(beneficiaryId);
        var model = items.Select(x => new BeneficiaryOldRecordListItemVm
        {
            Id = x.Id,
            RecordDate = x.RecordDate,
            Title = x.Title,
            Details = x.Details,
            IsActive = x.IsActive
        }).ToList();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid beneficiaryId)
    {
        if (!await PopulateBeneficiaryAsync(beneficiaryId))
            return NotFound();

        return View(new CreateBeneficiaryOldRecordVm
        {
            BeneficiaryId = beneficiaryId,
            RecordDate = DateTime.Today,
            IsActive = true
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBeneficiaryOldRecordVm vm)
    {
        if (!await PopulateBeneficiaryAsync(vm.BeneficiaryId))
            return NotFound();

        if (!ModelState.IsValid)
            return View(vm);

        var entity = new BeneficiaryOldRecord
        {
            Id = Guid.NewGuid(),
            BeneficiaryId = vm.BeneficiaryId,
            RecordDate = vm.RecordDate,
            Title = vm.Title.Trim(),
            Details = vm.Details?.Trim(),
            CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            CreatedAtUtc = DateTime.UtcNow,
            IsActive = vm.IsActive
        };

        await _repository.AddAsync(entity);
        TempData["Success"] = "تم حفظ السجل السابق بنجاح";
        return RedirectToAction(nameof(Index), new { beneficiaryId = vm.BeneficiaryId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        if (!await PopulateBeneficiaryAsync(entity.BeneficiaryId))
            return NotFound();

        var vm = new EditBeneficiaryOldRecordVm
        {
            Id = entity.Id,
            BeneficiaryId = entity.BeneficiaryId,
            RecordDate = entity.RecordDate,
            Title = entity.Title,
            Details = entity.Details,
            IsActive = entity.IsActive
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditBeneficiaryOldRecordVm vm)
    {
        var entity = await _repository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        if (!await PopulateBeneficiaryAsync(entity.BeneficiaryId))
            return NotFound();

        if (!ModelState.IsValid)
            return View(vm);

        entity.RecordDate = vm.RecordDate;
        entity.Title = vm.Title.Trim();
        entity.Details = vm.Details?.Trim();
        entity.IsActive = vm.IsActive;

        await _repository.UpdateAsync(entity);
        TempData["Success"] = "تم تعديل السجل السابق بنجاح";
        return RedirectToAction(nameof(Index), new { beneficiaryId = entity.BeneficiaryId });
    }
    private async Task<BeneficiaryHeaderVm?> BuildBeneficiaryHeaderAsync(Guid? beneficiaryId)
    {
        if (!beneficiaryId.HasValue || beneficiaryId.Value == Guid.Empty)
            return null;

        var beneficiary = await _beneficiaryRepository.GetByIdAsync(beneficiaryId.Value);
        if (beneficiary == null)
            return null;

        return new BeneficiaryHeaderVm
        {
            Id = beneficiary.Id,
            Code = beneficiary.Code,
            FullName = beneficiary.FullName,
            NationalId = beneficiary.NationalId,
            PhoneNumber = beneficiary.PhoneNumber,
            StatusName = beneficiary.Status?.NameAr
        };
    }
    private async Task<bool> PopulateBeneficiaryAsync(Guid beneficiaryId)
    {
        var beneficiary = await _beneficiaryRepository.GetByIdAsync(beneficiaryId);
        if (beneficiary == null)
            return false;

        ViewBag.BeneficiaryId = beneficiary.Id;
        ViewBag.BeneficiaryCode = beneficiary.Code;
        ViewBag.BeneficiaryName = beneficiary.FullName;
        return true;
    }
}
