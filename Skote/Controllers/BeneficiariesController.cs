using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Charity.Beneficiaries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Skote.Helpers;
using System.Security.Claims;
[Authorize(Policy = CharityPolicies.BeneficiariesView)]
public class BeneficiariesController : Controller
{
    private readonly IBeneficiaryRepository _beneficiaryRepository;
    private readonly AppDbContext _db;

    public BeneficiariesController(IBeneficiaryRepository beneficiaryRepository, AppDbContext db)
    {
        _beneficiaryRepository = beneficiaryRepository;
        _db = db;
    }

    public async Task<IActionResult> Index(string? q, Guid? statusId, bool? isActive)
    {
        var beneficiaries = await _beneficiaryRepository.SearchAsync(q, statusId, isActive);
        var statuses = await _db.BeneficiaryStatusLookups
            .AsNoTracking()
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.NameAr)
            .ToListAsync();

        var rows = beneficiaries.Select(x => new BeneficiaryListRowVm
        {
            Id = x.Id,
            Code = x.Code,
            FullName = x.FullName,
            NationalId = x.NationalId,
            PhoneNumber = x.PhoneNumber,
            GenderText = x.Gender?.NameAr,
            GovernorateName = x.Governorate?.NameAr,
            StatusText = x.Status?.NameAr,
            RegistrationDate = x.RegistrationDate,
            FamilyMembersCount = x.FamilyMembersCount,
            IsActive = x.IsActive
        }).ToList();

        var vm = new BeneficiaryListPageVm
        {
            Filter = new BeneficiaryListFilterVm
            {
                Q = q,
                StatusId = statusId,
                IsActive = isActive,
                Statuses = statuses.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.NameAr
                }).ToList()
            },
            Rows = rows,
            TotalCount = rows.Count,
            ActiveCount = rows.Count(x => x.IsActive),
            ApprovedCount = rows.Count(x => string.Equals(x.StatusText, "معتمد", StringComparison.OrdinalIgnoreCase))
        };

        return View(vm);
    }
    [Authorize(Policy = CharityPolicies.BeneficiariesManage)]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new CreateBeneficiaryVm
        {
            Code = await GenerateBeneficiaryCodeAsync(),
            StatusId = CharityLookupSeedIds.BeneficiaryStatusNew,
            RegistrationDate = DateTime.Today
        };

        await FillLookupsAsync(vm);
        return View(vm);
    }
    [Authorize(Policy = CharityPolicies.BeneficiariesManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBeneficiaryVm vm)
    {
        ModelState.Remove(nameof(vm.Code));
        ModelState.Remove(nameof(vm.StatusId));

        vm.Code = await GenerateBeneficiaryCodeAsync();
        vm.StatusId = CharityLookupSeedIds.BeneficiaryStatusNew;

        await FillLookupsAsync(vm);

        if (!ModelState.IsValid)
            return View(vm);

        if (!string.IsNullOrWhiteSpace(vm.NationalId) && await _beneficiaryRepository.NationalIdExistsAsync(vm.NationalId.Trim()))
        {
            ModelState.AddModelError(nameof(vm.NationalId), "الرقم القومي مسجل بالفعل");
            return View(vm);
        }

        var entity = new Beneficiary
        {
            Id = Guid.NewGuid(),
            Code = vm.Code.Trim(),
            FullName = vm.FullName.Trim(),
            NationalId = vm.NationalId?.Trim(),
            BirthDate = vm.BirthDate,
            GenderId = vm.GenderId,
            MaritalStatusId = vm.MaritalStatusId,
            PhoneNumber = vm.PhoneNumber?.Trim(),
            AlternatePhoneNumber = vm.AlternatePhoneNumber?.Trim(),
            AddressLine = vm.AddressLine?.Trim(),
            GovernorateId = vm.GovernorateId,
            CityId = vm.CityId,
            AreaId = vm.AreaId,
            FamilyMembersCount = vm.FamilyMembersCount,
            MonthlyIncome = vm.MonthlyIncome,
            IncomeSource = vm.IncomeSource?.Trim(),
            HealthStatus = vm.HealthStatus?.Trim(),
            EducationStatus = vm.EducationStatus?.Trim(),
            WorkStatus = vm.WorkStatus?.Trim(),
            HousingStatus = vm.HousingStatus?.Trim(),
            StatusId = CharityLookupSeedIds.BeneficiaryStatusNew,
            RegistrationDate = vm.RegistrationDate,
            Notes = vm.Notes?.Trim(),
            CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            CreatedAtUtc = DateTime.UtcNow,
            IsActive = true
        };

        await _beneficiaryRepository.AddAsync(entity);
        TempData["Success"] = "تم إضافة المستفيد بنجاح";
        return RedirectToAction(nameof(Index));
    }
    [Authorize(Policy = CharityPolicies.BeneficiariesManage)]
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _beneficiaryRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var vm = new EditBeneficiaryVm
        {
            Id = entity.Id,
            Code = entity.Code,
            FullName = entity.FullName,
            NationalId = entity.NationalId,
            BirthDate = entity.BirthDate,
            GenderId = entity.GenderId,
            MaritalStatusId = entity.MaritalStatusId,
            PhoneNumber = entity.PhoneNumber,
            AlternatePhoneNumber = entity.AlternatePhoneNumber,
            AddressLine = entity.AddressLine,
            GovernorateId = entity.GovernorateId,
            CityId = entity.CityId,
            AreaId = entity.AreaId,
            FamilyMembersCount = entity.FamilyMembersCount,
            MonthlyIncome = entity.MonthlyIncome,
            IncomeSource = entity.IncomeSource,
            HealthStatus = entity.HealthStatus,
            EducationStatus = entity.EducationStatus,
            WorkStatus = entity.WorkStatus,
            HousingStatus = entity.HousingStatus,
            StatusId = entity.StatusId,
            RegistrationDate = entity.RegistrationDate,
            Notes = entity.Notes,
            IsActive = entity.IsActive
        };

        await FillLookupsAsync(vm);
        return View(vm);
    }
    [Authorize(Policy = CharityPolicies.BeneficiariesManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditBeneficiaryVm vm)
    {
        ModelState.Remove(nameof(vm.Code));
        ModelState.Remove(nameof(vm.StatusId));

        await FillLookupsAsync(vm);

        if (!ModelState.IsValid)
            return View(vm);

        var entity = await _beneficiaryRepository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        vm.Code = entity.Code;
        vm.StatusId = entity.StatusId;

        if (!string.IsNullOrWhiteSpace(vm.NationalId) && await _beneficiaryRepository.NationalIdExistsAsync(vm.NationalId.Trim(), vm.Id))
        {
            ModelState.AddModelError(nameof(vm.NationalId), "الرقم القومي مسجل بالفعل");
            return View(vm);
        }

        // الكود وحالة الملف يتم التحكم فيهما تلقائيًا ولا يتم تعديلهما من شاشة بيانات المستفيد.
        entity.FullName = vm.FullName.Trim();
        entity.NationalId = vm.NationalId?.Trim();
        entity.BirthDate = vm.BirthDate;
        entity.GenderId = vm.GenderId;
        entity.MaritalStatusId = vm.MaritalStatusId;
        entity.PhoneNumber = vm.PhoneNumber?.Trim();
        entity.AlternatePhoneNumber = vm.AlternatePhoneNumber?.Trim();
        entity.AddressLine = vm.AddressLine?.Trim();
        entity.GovernorateId = vm.GovernorateId;
        entity.CityId = vm.CityId;
        entity.AreaId = vm.AreaId;
        entity.FamilyMembersCount = vm.FamilyMembersCount;
        entity.MonthlyIncome = vm.MonthlyIncome;
        entity.IncomeSource = vm.IncomeSource?.Trim();
        entity.HealthStatus = vm.HealthStatus?.Trim();
        entity.EducationStatus = vm.EducationStatus?.Trim();
        entity.WorkStatus = vm.WorkStatus?.Trim();
        entity.HousingStatus = vm.HousingStatus?.Trim();
        // StatusId يتغير من دورة البحث الإنساني فقط.
        entity.RegistrationDate = vm.RegistrationDate;
        entity.Notes = vm.Notes?.Trim();
        entity.IsActive = vm.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _beneficiaryRepository.UpdateAsync(entity);
        TempData["Success"] = "تم تعديل بيانات المستفيد بنجاح";
        return RedirectToAction(nameof(Details), new { id = vm.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var entity = await _beneficiaryRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var vm = new BeneficiaryDetailsVm
        {
            Id = entity.Id,
            Code = entity.Code,
            FullName = entity.FullName,
            NationalId = entity.NationalId,
            BirthDate = entity.BirthDate,
            GenderText = entity.Gender?.NameAr,
            MaritalStatusText = entity.MaritalStatus?.NameAr,
            PhoneNumber = entity.PhoneNumber,
            AlternatePhoneNumber = entity.AlternatePhoneNumber,
            AddressLine = entity.AddressLine,
            GovernorateName = entity.Governorate?.NameAr,
            CityName = entity.City?.NameAr,
            AreaName = entity.Area?.NameAr,
            FamilyMembersCount = entity.FamilyMembersCount,
            MonthlyIncome = entity.MonthlyIncome,
            IncomeSource = entity.IncomeSource,
            HealthStatus = entity.HealthStatus,
            EducationStatus = entity.EducationStatus,
            WorkStatus = entity.WorkStatus,
            HousingStatus = entity.HousingStatus,
            StatusText = entity.Status?.NameAr,
            RegistrationDate = entity.RegistrationDate,
            Notes = entity.Notes,
            IsActive = entity.IsActive,
            FamilyMembers = entity.FamilyMembers
                .OrderBy(x => x.FullName)
                .Select(x => new BeneficiaryFamilyMemberItemVm
                {
                    FullName = x.FullName,
                    Relationship = x.Relationship,
                    NationalId = x.NationalId,
                    BirthDate = x.BirthDate,
                    MonthlyIncome = x.MonthlyIncome,
                    IsDependent = x.IsDependent
                }).ToList(),
            Documents = entity.Documents
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new BeneficiaryDocumentItemVm
                {
                    DocumentType = x.DocumentType,
                    
                }).ToList(),
            Assessments = entity.Assessments
                .OrderByDescending(x => x.VisitDate)
                .Select(x => new BeneficiaryAssessmentItemVm
                {
                    VisitDate = x.VisitDate,
                    RecommendedAidType = x.RecommendedAidType != null ? x.RecommendedAidType.NameAr : null,
                    RecommendationAmount = x.RecommendationAmount,
                    AssessmentScore = x.AssessmentScore,
                    RecommendationText = x.RecommendationText
                }).ToList(),
            CommitteeDecisions = entity.CommitteeDecisions
                .OrderByDescending(x => x.DecisionDate)
                .Select(x => new BeneficiaryCommitteeDecisionItemVm
                {
                    DecisionDate = x.DecisionDate,
                    DecisionType = x.DecisionType,
                    ApprovedAidType = x.ApprovedAidType != null ? x.ApprovedAidType.NameAr : null,
                    ApprovedAmount = x.ApprovedAmount,
                    DurationInMonths = x.DurationInMonths,
                    CommitteeNotes = x.CommitteeNotes,
                    ApprovedStatus = x.ApprovedStatus
                }).ToList(),
            AidRequests = entity.AidRequests
                .OrderByDescending(x => x.RequestDate)
                .Select(x => new BeneficiaryAidRequestItemVm
                {
                    RequestDate = x.RequestDate,
                    AidType = x.AidType != null ? x.AidType.NameAr : null,
                    RequestedAmount = x.RequestedAmount,
                    Status = x.Status,
                    Reason = x.Reason
                }).ToList(),
            AidDisbursements = entity.AidDisbursements
                .OrderByDescending(x => x.DisbursementDate)
                .Select(x => new BeneficiaryAidDisbursementItemVm
                {
                    DisbursementDate = x.DisbursementDate,
                    AidType = x.AidType != null ? x.AidType.NameAr : null,
                    Amount = x.Amount,
                    Notes = x.Notes
                }).ToList(),
            OldRecords = entity.OldRecords
                .OrderByDescending(x => x.RecordDate)
                .Select(x => new BeneficiaryOldRecordItemVm
                {
                    RecordDate = x.RecordDate,
                    Title = x.Title,
                    Details = x.Details
                }).ToList()
        };
        var categoryAssignments = await _db.Set<BeneficiaryCategoryAssignment>()
            .AsNoTracking()
            .Include(x => x.Category)
            .Where(x => x.BeneficiaryId == id)
            .OrderByDescending(x => x.AssignedAtUtc)
            .ToListAsync();

        ViewBag.BeneficiaryCategoryAssignments = categoryAssignments;
        return View(vm);
    }

    private async Task FillLookupsAsync(CreateBeneficiaryVm vm)
    {
        var genders = await _db.GenderLookups.AsNoTracking().OrderBy(x => x.DisplayOrder).ThenBy(x => x.NameAr).ToListAsync();
        var maritalStatuses = await _db.MaritalStatusLookups.AsNoTracking().OrderBy(x => x.DisplayOrder).ThenBy(x => x.NameAr).ToListAsync();
        var governorates = await _db.Governorates.AsNoTracking().OrderBy(x => x.NameAr).ToListAsync();
        var cities = await _db.Cities.AsNoTracking().OrderBy(x => x.NameAr).ToListAsync();
        var areas = await _db.Areas.AsNoTracking().OrderBy(x => x.NameAr).ToListAsync();
        var statuses = await _db.BeneficiaryStatusLookups.AsNoTracking().OrderBy(x => x.DisplayOrder).ThenBy(x => x.NameAr).ToListAsync();

        vm.Genders = genders.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.NameAr }).ToList();
        vm.MaritalStatuses = maritalStatuses.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.NameAr }).ToList();
        vm.Governorates = governorates.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.NameAr }).ToList();
        vm.Cities = cities.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.NameAr }).ToList();
        vm.Areas = areas.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.NameAr }).ToList();
        vm.Statuses = statuses.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.NameAr }).ToList();
    }

    private async Task<string> GenerateBeneficiaryCodeAsync()
    {
        string code;
        do
        {
            code = $"BEN-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
            await Task.Delay(2);
        }
        while (await _beneficiaryRepository.CodeExistsAsync(code));

        return code;
    }
}
