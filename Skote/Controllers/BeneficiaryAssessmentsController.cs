using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.Assessments;
using InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class BeneficiaryAssessmentsController : Controller
{
    private readonly IBeneficiaryAssessmentRepository _repository;
    private readonly IBeneficiaryRepository _beneficiaryRepository;
    private readonly AppDbContext _db;

    public BeneficiaryAssessmentsController(
        IBeneficiaryAssessmentRepository repository,
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
        var model = items.Select(x => new BeneficiaryAssessmentListItemVm
        {
            Id = x.Id,
            VisitDate = x.VisitDate,
            RecommendedAidType = x.RecommendedAidType?.NameAr,
            RecommendationAmount = x.RecommendationAmount,
            AssessmentScore = x.AssessmentScore,
            DecisionSuggested = x.DecisionSuggested,
            RecommendationText = x.RecommendationText
        }).ToList();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid beneficiaryId)
    {
        if (!await PopulateBeneficiaryAsync(beneficiaryId))
            return NotFound();

        var vm = new CreateBeneficiaryAssessmentVm { BeneficiaryId = beneficiaryId, VisitDate = DateTime.Today };
        await FillLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBeneficiaryAssessmentVm vm)
    {
        if (!await PopulateBeneficiaryAsync(vm.BeneficiaryId))
            return NotFound();

        await FillLookupsAsync(vm);

        if (!ModelState.IsValid)
            return View(vm);

        var entity = new BeneficiaryAssessment
        {
            Id = Guid.NewGuid(),
            BeneficiaryId = vm.BeneficiaryId,
            VisitDate = vm.VisitDate,
            ResearcherUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            HousingCondition = vm.HousingCondition?.Trim(),
            EconomicCondition = vm.EconomicCondition?.Trim(),
            HealthCondition = vm.HealthCondition?.Trim(),
            SocialCondition = vm.SocialCondition?.Trim(),
            RecommendedAidTypeId = vm.RecommendedAidTypeId,
            RecommendationAmount = vm.RecommendationAmount,
            AssessmentScore = vm.AssessmentScore,
            RecommendationText = vm.RecommendationText?.Trim(),
            DecisionSuggested = vm.DecisionSuggested?.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        await _repository.AddAsync(entity);
        TempData["Success"] = "تم حفظ البحث الاجتماعي بنجاح";
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

        var vm = new EditBeneficiaryAssessmentVm
        {
            Id = entity.Id,
            BeneficiaryId = entity.BeneficiaryId,
            VisitDate = entity.VisitDate,
            HousingCondition = entity.HousingCondition,
            EconomicCondition = entity.EconomicCondition,
            HealthCondition = entity.HealthCondition,
            SocialCondition = entity.SocialCondition,
            RecommendedAidTypeId = entity.RecommendedAidTypeId,
            RecommendationAmount = entity.RecommendationAmount,
            AssessmentScore = entity.AssessmentScore,
            RecommendationText = entity.RecommendationText,
            DecisionSuggested = entity.DecisionSuggested
        };

        await FillLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditBeneficiaryAssessmentVm vm)
    {
        var entity = await _repository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        if (!await PopulateBeneficiaryAsync(entity.BeneficiaryId))
            return NotFound();

        await FillLookupsAsync(vm);

        if (!ModelState.IsValid)
            return View(vm);

        entity.VisitDate = vm.VisitDate;
        entity.HousingCondition = vm.HousingCondition?.Trim();
        entity.EconomicCondition = vm.EconomicCondition?.Trim();
        entity.HealthCondition = vm.HealthCondition?.Trim();
        entity.SocialCondition = vm.SocialCondition?.Trim();
        entity.RecommendedAidTypeId = vm.RecommendedAidTypeId;
        entity.RecommendationAmount = vm.RecommendationAmount;
        entity.AssessmentScore = vm.AssessmentScore;
        entity.RecommendationText = vm.RecommendationText?.Trim();
        entity.DecisionSuggested = vm.DecisionSuggested?.Trim();

        await _repository.UpdateAsync(entity);
        TempData["Success"] = "تم تعديل البحث الاجتماعي بنجاح";
        return RedirectToAction(nameof(Index), new { beneficiaryId = entity.BeneficiaryId });
    }

    private async Task FillLookupsAsync(CreateBeneficiaryAssessmentVm vm)
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
