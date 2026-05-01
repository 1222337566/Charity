using InfrastrfuctureManagmentCore.Domains.Charity.Funders;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Funders;
using InfrastructureManagmentWebFramework.Models.Charity.FunderProfile;
using InfrastructureManagmentWebFramework.Models.Charity.FunderProfile.GrantConditions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skote.Helpers;
[Authorize(Policy = CharityPolicies.FundersView)]
public class GrantConditionsController : Controller
{
    private readonly IGrantConditionRepository _grantConditionRepository;
    private readonly IGrantAgreementRepository _grantAgreementRepository;

    public GrantConditionsController(IGrantConditionRepository grantConditionRepository, IGrantAgreementRepository grantAgreementRepository)
    {
        _grantConditionRepository = grantConditionRepository;
        _grantAgreementRepository = grantAgreementRepository;
    }

    public async Task<IActionResult> Index(Guid grantAgreementId)
    {
        if (!await PopulateAgreementAsync(grantAgreementId))
            return NotFound();

        var items = await _grantConditionRepository.GetByGrantAgreementIdAsync(grantAgreementId);
        var model = items.Select(x => new GrantConditionListItemVm
        {
            Id = x.Id,
            ConditionTitle = x.ConditionTitle,
            ConditionDetails = x.ConditionDetails,
            IsMandatory = x.IsMandatory,
            DueDate = x.DueDate,
            IsFulfilled = x.IsFulfilled,
            FulfilledDate = x.FulfilledDate,
            Notes = x.Notes
        }).ToList();

        return View(model);
    }
    [Authorize(Policy = CharityPolicies.FundersManage)]
    [HttpGet]
    public async Task<IActionResult> Create(Guid grantAgreementId)
    {
        if (!await PopulateAgreementAsync(grantAgreementId))
            return NotFound();

        var vm = new CreateGrantConditionVm
        {
            GrantAgreementId = grantAgreementId
        };

        return View(vm);
    }
    [Authorize(Policy = CharityPolicies.FundersManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateGrantConditionVm vm)
    {
        if (!await PopulateAgreementAsync(vm.GrantAgreementId))
            return NotFound();

        if (!ModelState.IsValid)
            return View(vm);

        if (vm.IsFulfilled && !vm.FulfilledDate.HasValue)
            vm.FulfilledDate = DateTime.Today;

        var entity = new GrantCondition
        {
            Id = Guid.NewGuid(),
            GrantAgreementId = vm.GrantAgreementId,
            ConditionTitle = vm.ConditionTitle.Trim(),
            ConditionDetails = vm.ConditionDetails.Trim(),
            IsMandatory = vm.IsMandatory,
            DueDate = vm.DueDate,
            IsFulfilled = vm.IsFulfilled,
            FulfilledDate = vm.FulfilledDate,
            Notes = vm.Notes?.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        await _grantConditionRepository.AddAsync(entity);
        TempData["Success"] = "تم إضافة الشرط بنجاح";
        return RedirectToAction(nameof(Index), new { grantAgreementId = vm.GrantAgreementId });
    }
    [Authorize(Policy = CharityPolicies.FundersManage)]
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _grantConditionRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        if (!await PopulateAgreementAsync(entity.GrantAgreementId))
            return NotFound();

        var vm = new EditGrantConditionVm
        {
            Id = entity.Id,
            GrantAgreementId = entity.GrantAgreementId,
            ConditionTitle = entity.ConditionTitle,
            ConditionDetails = entity.ConditionDetails,
            IsMandatory = entity.IsMandatory,
            DueDate = entity.DueDate,
            IsFulfilled = entity.IsFulfilled,
            FulfilledDate = entity.FulfilledDate,
            Notes = entity.Notes
        };

        return View(vm);
    }
    [Authorize(Policy = CharityPolicies.FundersManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditGrantConditionVm vm)
    {
        var entity = await _grantConditionRepository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        if (!await PopulateAgreementAsync(entity.GrantAgreementId))
            return NotFound();

        if (!ModelState.IsValid)
            return View(vm);

        if (vm.IsFulfilled && !vm.FulfilledDate.HasValue)
            vm.FulfilledDate = DateTime.Today;
        if (!vm.IsFulfilled)
            vm.FulfilledDate = null;

        entity.ConditionTitle = vm.ConditionTitle.Trim();
        entity.ConditionDetails = vm.ConditionDetails.Trim();
        entity.IsMandatory = vm.IsMandatory;
        entity.DueDate = vm.DueDate;
        entity.IsFulfilled = vm.IsFulfilled;
        entity.FulfilledDate = vm.FulfilledDate;
        entity.Notes = vm.Notes?.Trim();
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _grantConditionRepository.UpdateAsync(entity);
        TempData["Success"] = "تم تعديل الشرط بنجاح";
        return RedirectToAction(nameof(Index), new { grantAgreementId = entity.GrantAgreementId });
    }

    private async Task<bool> PopulateAgreementAsync(Guid grantAgreementId)
    {
        var agreement = await _grantAgreementRepository.GetByIdAsync(grantAgreementId);
        if (agreement == null || agreement.Funder == null)
            return false;

        ViewBag.GrantAgreementHeader = new GrantAgreementHeaderVm
        {
            Id = agreement.Id,
            FunderId = agreement.FunderId,
            AgreementNumber = agreement.AgreementNumber,
            Title = agreement.Title,
            FunderName = agreement.Funder.Name,
            Status = agreement.Status,
            TotalAmount = agreement.TotalAmount,
            Currency = agreement.Currency
        };

        return true;
    }
}
