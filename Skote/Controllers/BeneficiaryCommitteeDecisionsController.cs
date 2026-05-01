using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.CommitteeDecisions;
using InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Skote.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize(Policy = CharityPolicies.BeneficiariesView)]
public class BeneficiaryCommitteeDecisionsController : Controller
{
    private readonly IBeneficiaryCommitteeDecisionRepository _repository;
    private readonly IBeneficiaryRepository _beneficiaryRepository;
    private readonly AppDbContext _db;
    private readonly IUserActivityService _activityService;

    public BeneficiaryCommitteeDecisionsController(
        IBeneficiaryCommitteeDecisionRepository repository,
        IBeneficiaryRepository beneficiaryRepository,
        AppDbContext db,
        IUserActivityService activityService)
    {
        _repository = repository;
        _beneficiaryRepository = beneficiaryRepository;
        _db = db;
        _activityService = activityService;
    }

    public async Task<IActionResult> Index(Guid beneficiaryId)
    {
        if (!await PopulateBeneficiaryAsync(beneficiaryId))
            return NotFound();

        var items = await _repository.GetByBeneficiaryIdAsync(beneficiaryId);
        var model = items.Select(x => new BeneficiaryCommitteeDecisionListItemVm
        {
            Id = x.Id,
            DecisionDate = x.DecisionDate,
            DecisionType = x.DecisionType,
            ApprovedAidType = x.ApprovedAidType?.NameAr,
            ApprovedAmount = x.ApprovedAmount,
            DurationInMonths = x.DurationInMonths,
            CommitteeNotes = x.CommitteeNotes,
            ApprovedStatus = x.ApprovedStatus
        }).ToList();

        return View(model);
    }

    [Authorize(Policy = CharityPolicies.CommitteeDecisionManage)]
    [HttpGet]
    public async Task<IActionResult> Create(Guid beneficiaryId)
    {
        if (!await PopulateBeneficiaryAsync(beneficiaryId))
            return NotFound();

        var vm = new CreateBeneficiaryCommitteeDecisionVm { BeneficiaryId = beneficiaryId, DecisionDate = DateTime.Today };
        await FillLookupsAsync(vm);
        return View(vm);
    }

    [Authorize(Policy = CharityPolicies.CommitteeDecisionManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBeneficiaryCommitteeDecisionVm vm)
    {
        if (!await PopulateBeneficiaryAsync(vm.BeneficiaryId))
            return NotFound();

        await FillLookupsAsync(vm);

        if (!ModelState.IsValid)
            return View(vm);

        var entity = new BeneficiaryCommitteeDecision
        {
            Id = Guid.NewGuid(),
            BeneficiaryId = vm.BeneficiaryId,
            DecisionDate = vm.DecisionDate,
            DecisionType = vm.DecisionType.Trim(),
            ApprovedAidTypeId = vm.ApprovedAidTypeId,
            ApprovedAmount = vm.ApprovedAmount,
            DurationInMonths = vm.DurationInMonths,
            CommitteeNotes = vm.CommitteeNotes?.Trim(),
            ApprovedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            ApprovedStatus = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _repository.AddAsync(entity);

        await _activityService.LogBusinessAsync(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            UserActivityBusinessActions.CommitteeDecisionCreated,
            $"تم إنشاء قرار لجنة للمستفيد {ViewBag.BeneficiaryName ?? vm.BeneficiaryId.ToString()}",
            entityName: "قرار لجنة",
            entityId: entity.Id.ToString(),
            newValues: new Dictionary<string, string?>
            {
                ["BeneficiaryId"] = vm.BeneficiaryId.ToString(),
                ["DecisionDate"] = vm.DecisionDate.ToString("yyyy-MM-dd"),
                ["DecisionType"] = vm.DecisionType,
                ["ApprovedAidTypeId"] = vm.ApprovedAidTypeId?.ToString(),
                ["ApprovedAmount"] = vm.ApprovedAmount?.ToString("0.##"),
                ["DurationInMonths"] = vm.DurationInMonths?.ToString()
            },
            ct: HttpContext.RequestAborted);

        TempData["Success"] = "تم حفظ قرار اللجنة بنجاح";
        return RedirectToAction(nameof(Index), new { beneficiaryId = vm.BeneficiaryId });
    }

    [Authorize(Policy = CharityPolicies.CommitteeDecisionManage)]
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        if (!await PopulateBeneficiaryAsync(entity.BeneficiaryId))
            return NotFound();

        var vm = new EditBeneficiaryCommitteeDecisionVm
        {
            Id = entity.Id,
            BeneficiaryId = entity.BeneficiaryId,
            DecisionDate = entity.DecisionDate,
            DecisionType = entity.DecisionType,
            ApprovedAidTypeId = entity.ApprovedAidTypeId,
            ApprovedAmount = entity.ApprovedAmount,
            DurationInMonths = entity.DurationInMonths,
            CommitteeNotes = entity.CommitteeNotes
        };

        await FillLookupsAsync(vm);
        return View(vm);
    }

    [Authorize(Policy = CharityPolicies.CommitteeDecisionManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditBeneficiaryCommitteeDecisionVm vm)
    {
        var entity = await _repository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        if (!await PopulateBeneficiaryAsync(entity.BeneficiaryId))
            return NotFound();

        await FillLookupsAsync(vm);

        if (!ModelState.IsValid)
            return View(vm);

        var oldValues = new Dictionary<string, string?>
        {
            ["DecisionDate"] = entity.DecisionDate.ToString("yyyy-MM-dd"),
            ["DecisionType"] = entity.DecisionType,
            ["ApprovedAidTypeId"] = entity.ApprovedAidTypeId?.ToString(),
            ["ApprovedAmount"] = entity.ApprovedAmount?.ToString("0.##"),
            ["DurationInMonths"] = entity.DurationInMonths?.ToString(),
            ["CommitteeNotes"] = entity.CommitteeNotes
        };

        entity.DecisionDate = vm.DecisionDate;
        entity.DecisionType = vm.DecisionType.Trim();
        entity.ApprovedAidTypeId = vm.ApprovedAidTypeId;
        entity.ApprovedAmount = vm.ApprovedAmount;
        entity.DurationInMonths = vm.DurationInMonths;
        entity.CommitteeNotes = vm.CommitteeNotes?.Trim();
        entity.ApprovedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        entity.ApprovedStatus = true;

        await _repository.UpdateAsync(entity);

        await _activityService.LogBusinessAsync(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            UserActivityBusinessActions.CommitteeDecisionUpdated,
            $"تم تعديل قرار اللجنة للمستفيد {ViewBag.BeneficiaryName ?? entity.BeneficiaryId.ToString()}",
            entityName: "قرار لجنة",
            entityId: entity.Id.ToString(),
            oldValues: oldValues,
            newValues: new Dictionary<string, string?>
            {
                ["DecisionDate"] = vm.DecisionDate.ToString("yyyy-MM-dd"),
                ["DecisionType"] = vm.DecisionType,
                ["ApprovedAidTypeId"] = vm.ApprovedAidTypeId?.ToString(),
                ["ApprovedAmount"] = vm.ApprovedAmount?.ToString("0.##"),
                ["DurationInMonths"] = vm.DurationInMonths?.ToString(),
                ["CommitteeNotes"] = vm.CommitteeNotes
            },
            ct: HttpContext.RequestAborted);

        TempData["Success"] = "تم تعديل قرار اللجنة بنجاح";
        return RedirectToAction(nameof(Index), new { beneficiaryId = entity.BeneficiaryId });
    }

    private async Task FillLookupsAsync(CreateBeneficiaryCommitteeDecisionVm vm)
    {
        var aidTypes = await _db.AidTypeLookups.AsNoTracking()
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.NameAr)
            .ToListAsync();

        vm.AidTypes = aidTypes.Select(x => new SelectListItem
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
