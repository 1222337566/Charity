using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.FamilyMembers;
using InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class BeneficiaryFamilyMembersController : Controller
{
    private readonly IBeneficiaryFamilyMemberRepository _repository;
    private readonly IBeneficiaryRepository _beneficiaryRepository;
    private readonly AppDbContext _db;

    public BeneficiaryFamilyMembersController(
        IBeneficiaryFamilyMemberRepository repository,
        IBeneficiaryRepository beneficiaryRepository,
        AppDbContext db)
    {
        _repository = repository;
        _beneficiaryRepository = beneficiaryRepository;
        _db = db;
    }

    public async Task<IActionResult> Index(Guid beneficiaryId)
    {
        if (!await PopulateBeneficiaryAsync(beneficiaryId))
            return NotFound();

        var items = await _repository.GetByBeneficiaryIdAsync(beneficiaryId);
        var model = items.Select(x => new BeneficiaryFamilyMemberListItemVm
        {
            Id = x.Id,
            FullName = x.FullName,
            Relationship = x.Relationship,
            NationalId = x.NationalId,
            BirthDate = x.BirthDate,
            GenderText = x.Gender?.NameAr,
            EducationStatus = x.EducationStatus,
            WorkStatus = x.WorkStatus,
            MonthlyIncome = x.MonthlyIncome,
            IsDependent = x.IsDependent
        }).ToList();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid beneficiaryId)
    {
        if (!await PopulateBeneficiaryAsync(beneficiaryId))
            return NotFound();

        var vm = new CreateBeneficiaryFamilyMemberVm
        {
            BeneficiaryId = beneficiaryId,
            IsDependent = true
        };

        await FillLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBeneficiaryFamilyMemberVm vm)
    {
        if (!await PopulateBeneficiaryAsync(vm.BeneficiaryId))
            return NotFound();

        await FillLookupsAsync(vm);

        if (!ModelState.IsValid)
            return View(vm);

        var entity = new BeneficiaryFamilyMember
        {
            Id = Guid.NewGuid(),
            BeneficiaryId = vm.BeneficiaryId,
            FullName = vm.FullName.Trim(),
            Relationship = vm.Relationship.Trim(),
            NationalId = vm.NationalId?.Trim(),
            BirthDate = vm.BirthDate,
            GenderId = vm.GenderId,
            EducationStatus = vm.EducationStatus?.Trim(),
            WorkStatus = vm.WorkStatus?.Trim(),
            MonthlyIncome = vm.MonthlyIncome,
            HealthCondition = vm.HealthCondition?.Trim(),
            IsDependent = vm.IsDependent,
            Notes = vm.Notes?.Trim()
        };

        await _repository.AddAsync(entity);
        TempData["Success"] = "تم إضافة فرد الأسرة بنجاح";
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

        var vm = new EditBeneficiaryFamilyMemberVm
        {
            Id = entity.Id,
            BeneficiaryId = entity.BeneficiaryId,
            FullName = entity.FullName,
            Relationship = entity.Relationship,
            NationalId = entity.NationalId,
            BirthDate = entity.BirthDate,
            GenderId = entity.GenderId,
            EducationStatus = entity.EducationStatus,
            WorkStatus = entity.WorkStatus,
            MonthlyIncome = entity.MonthlyIncome,
            HealthCondition = entity.HealthCondition,
            IsDependent = entity.IsDependent,
            Notes = entity.Notes
        };

        await FillLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditBeneficiaryFamilyMemberVm vm)
    {
        var entity = await _repository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        if (!await PopulateBeneficiaryAsync(entity.BeneficiaryId))
            return NotFound();

        await FillLookupsAsync(vm);

        if (!ModelState.IsValid)
            return View(vm);

        entity.FullName = vm.FullName.Trim();
        entity.Relationship = vm.Relationship.Trim();
        entity.NationalId = vm.NationalId?.Trim();
        entity.BirthDate = vm.BirthDate;
        entity.GenderId = vm.GenderId;
        entity.EducationStatus = vm.EducationStatus?.Trim();
        entity.WorkStatus = vm.WorkStatus?.Trim();
        entity.MonthlyIncome = vm.MonthlyIncome;
        entity.HealthCondition = vm.HealthCondition?.Trim();
        entity.IsDependent = vm.IsDependent;
        entity.Notes = vm.Notes?.Trim();

        await _repository.UpdateAsync(entity);
        TempData["Success"] = "تم تعديل بيانات فرد الأسرة بنجاح";
        return RedirectToAction(nameof(Index), new { beneficiaryId = entity.BeneficiaryId });
    }

    private async Task FillLookupsAsync(CreateBeneficiaryFamilyMemberVm vm)
    {
        var genders = await _db.GenderLookups.AsNoTracking()
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.NameAr)
            .ToListAsync();

        vm.Genders = genders.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.NameAr
        }).ToList();
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
