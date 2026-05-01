using InfrastrfuctureManagmentCore.Domains.Accounting.Integration;
using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.AccountingIntegration;
using InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class AccountingIntegrationProfilesController : Controller
{
    private readonly IAccountingIntegrationProfileRepository _profileRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ICostCenterRepository _costCenterRepository;
    private readonly AppDbContext _db;

    public AccountingIntegrationProfilesController(
        IAccountingIntegrationProfileRepository profileRepository,
        IAccountRepository accountRepository,
        ICostCenterRepository costCenterRepository,
        AppDbContext db)
    {
        _profileRepository = profileRepository;
        _accountRepository = accountRepository;
        _costCenterRepository = costCenterRepository;
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _profileRepository.GetAllAsync();

        var sourceDefinitions = await _db.Set<AccountingIntegrationSourceDefinition>()
            .AsNoTracking()
            .ToDictionaryAsync(x => x.SourceType, x => x.NameAr);

        var aidTypes = await _db.Set<AidTypeLookup>()
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Id, x => x.NameAr);

        var model = items.Select(x => new AccountingIntegrationProfileListItemVm
        {
            Id = x.Id,
            SourceType = x.SourceType,
            SourceTypeNameAr = sourceDefinitions.TryGetValue(x.SourceType, out var sourceName) ? sourceName : null,
            EventCode = x.EventCode,
            EventNameAr = x.EventNameAr,
            ProfileNameAr = x.ProfileNameAr,
            MatchDonationType = x.MatchDonationType,
            MatchTargetingScopeCode = x.MatchTargetingScopeCode,
            MatchPurposeName = x.MatchPurposeName,
            MatchAidTypeNameAr = x.MatchAidTypeId.HasValue && aidTypes.TryGetValue(x.MatchAidTypeId.Value, out var aidTypeName) ? aidTypeName : null,
            MatchStoreMovementType = x.MatchStoreMovementType,
            Priority = x.Priority,
            DebitAccountCode = x.DebitAccount?.AccountCode,
            DebitAccountName = x.DebitAccount?.AccountNameAr,
            CreditAccountCode = x.CreditAccount?.AccountCode,
            CreditAccountName = x.CreditAccount?.AccountNameAr,
            CostCenterNameAr = x.DefaultCostCenter?.NameAr,
            UseSourceFinancialAccountAsDebit = x.UseSourceFinancialAccountAsDebit,
            UseSourceFinancialAccountAsCredit = x.UseSourceFinancialAccountAsCredit,
            AutoPost = x.AutoPost,
            IsActive = x.IsActive
        }).ToList();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new CreateAccountingIntegrationProfileVm();
        await FillLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateAccountingIntegrationProfileVm vm)
    {
        await FillLookupsAsync(vm);
        ValidateProfileVm(vm);
        if (!ModelState.IsValid)
            return View(vm);

        var entity = new AccountingIntegrationProfile
        {
            Id = Guid.NewGuid(),
            SourceType = Normalize(vm.SourceType),
            EventCode = NormalizeNullable(vm.EventCode),
            EventNameAr = NormalizeNullable(vm.EventNameAr),
            ProfileNameAr = vm.ProfileNameAr.Trim(),
            Description = NormalizeNullable(vm.Description),
            MatchDonationType = NormalizeNullable(vm.MatchDonationType),
            MatchTargetingScopeCode = NormalizeNullable(vm.MatchTargetingScopeCode),
            MatchPurposeName = NormalizeNullable(vm.MatchPurposeName),
            MatchAidTypeId = vm.MatchAidTypeId,
            MatchStoreMovementType = NormalizeNullable(vm.MatchStoreMovementType),
            Priority = vm.Priority,
            DebitAccountId = vm.DebitAccountId,
            CreditAccountId = vm.CreditAccountId,
            UseSourceFinancialAccountAsDebit = vm.UseSourceFinancialAccountAsDebit,
            UseSourceFinancialAccountAsCredit = vm.UseSourceFinancialAccountAsCredit,
            DefaultCostCenterId = vm.DefaultCostCenterId,
            AutoPost = vm.AutoPost,
            IsActive = vm.IsActive,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _profileRepository.AddAsync(entity);
        TempData["Success"] = "تم حفظ Profile الربط المحاسبي";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _profileRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var vm = new EditAccountingIntegrationProfileVm
        {
            Id = entity.Id,
            SourceType = entity.SourceType,
            EventCode = entity.EventCode,
            EventNameAr = entity.EventNameAr,
            ProfileNameAr = entity.ProfileNameAr,
            Description = entity.Description,
            MatchDonationType = entity.MatchDonationType,
            MatchTargetingScopeCode = entity.MatchTargetingScopeCode,
            MatchPurposeName = entity.MatchPurposeName,
            MatchAidTypeId = entity.MatchAidTypeId,
            MatchStoreMovementType = entity.MatchStoreMovementType,
            Priority = entity.Priority,
            DebitAccountId = entity.DebitAccountId,
            CreditAccountId = entity.CreditAccountId,
            UseSourceFinancialAccountAsDebit = entity.UseSourceFinancialAccountAsDebit,
            UseSourceFinancialAccountAsCredit = entity.UseSourceFinancialAccountAsCredit,
            DefaultCostCenterId = entity.DefaultCostCenterId,
            AutoPost = entity.AutoPost,
            IsActive = entity.IsActive
        };

        await FillLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditAccountingIntegrationProfileVm vm)
    {
        await FillLookupsAsync(vm);
        ValidateProfileVm(vm);
        if (!ModelState.IsValid)
            return View(vm);

        var entity = await _profileRepository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        entity.SourceType = Normalize(vm.SourceType);
        entity.EventCode = NormalizeNullable(vm.EventCode);
        entity.EventNameAr = NormalizeNullable(vm.EventNameAr);
        entity.ProfileNameAr = vm.ProfileNameAr.Trim();
        entity.Description = NormalizeNullable(vm.Description);
        entity.MatchDonationType = NormalizeNullable(vm.MatchDonationType);
        entity.MatchTargetingScopeCode = NormalizeNullable(vm.MatchTargetingScopeCode);
        entity.MatchPurposeName = NormalizeNullable(vm.MatchPurposeName);
        entity.MatchAidTypeId = vm.MatchAidTypeId;
        entity.MatchStoreMovementType = NormalizeNullable(vm.MatchStoreMovementType);
        entity.Priority = vm.Priority;
        entity.DebitAccountId = vm.DebitAccountId;
        entity.CreditAccountId = vm.CreditAccountId;
        entity.UseSourceFinancialAccountAsDebit = vm.UseSourceFinancialAccountAsDebit;
        entity.UseSourceFinancialAccountAsCredit = vm.UseSourceFinancialAccountAsCredit;
        entity.DefaultCostCenterId = vm.DefaultCostCenterId;
        entity.AutoPost = vm.AutoPost;
        entity.IsActive = vm.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _profileRepository.UpdateAsync(entity);
        TempData["Success"] = "تم تحديث Profile الربط المحاسبي";
        return RedirectToAction(nameof(Index));
    }

    private async Task FillLookupsAsync(CreateAccountingIntegrationProfileVm vm)
    {
        var accounts = await _accountRepository.GetAllAsync();
        var costCenters = await _costCenterRepository.GetAllAsync();

        var sourceDefinitions = await _db.Set<AccountingIntegrationSourceDefinition>()
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.NameAr)
            .ToListAsync();

        vm.SourceTypes = sourceDefinitions
            .Select(x => new SelectListItem($"{x.NameAr} ({x.SourceType})", x.SourceType))
            .ToList();

        vm.Accounts = accounts
            .Where(x => x.IsPosting && x.IsActive)
            .Select(x => new SelectListItem($"{x.AccountCode} - {x.AccountNameAr}", x.Id.ToString()))
            .ToList();

        vm.CostCenters = costCenters
            .Where(x => x.IsActive)
            .Select(x => new SelectListItem($"{x.CostCenterCode} - {x.NameAr}", x.Id.ToString()))
            .ToList();

        vm.AidTypes = await _db.Set<AidTypeLookup>()
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.NameAr)
            .Select(x => new SelectListItem(x.NameAr, x.Id.ToString()))
            .ToListAsync();

        vm.DonationTypes = new List<SelectListItem>
        {
            new("-- الكل --", ""),
            new("نقدي", "نقدي"),
            new("عيني", "عيني")
        };

        vm.TargetingScopes = new List<SelectListItem>
        {
            new("-- الكل --", ""),
            new("التبرعات العامة", DonationTargetingScopeOption.GeneralFund),
            new("غرض / باب عام", DonationTargetingScopeOption.GeneralPurpose),
            new("طلبات مساعدة محددة", DonationTargetingScopeOption.SpecificRequests)
        };

        vm.StoreMovementTypes = new List<SelectListItem>
        {
            new("-- الكل --", ""),
            new("تبرع عيني", "DonationInKind"),
            new("صرف مساعدة عينية", "AidInKindIssue"),
            new("صرف مشروع", "ProjectIssue"),
            new("إضافة مخزنية", "StoreReceipt"),
            new("صرف مخزني", "StoreIssue")
        };
    }

    private void ValidateProfileVm(CreateAccountingIntegrationProfileVm vm)
    {
        if (string.IsNullOrWhiteSpace(vm.SourceType))
            ModelState.AddModelError(nameof(vm.SourceType), "نوع المصدر مطلوب");

        if (!vm.UseSourceFinancialAccountAsDebit && !vm.DebitAccountId.HasValue)
            ModelState.AddModelError(nameof(vm.DebitAccountId), "حدد حساب المدين أو فعّل استخدام الحساب المالي من الحركة");

        if (!vm.UseSourceFinancialAccountAsCredit && !vm.CreditAccountId.HasValue)
            ModelState.AddModelError(nameof(vm.CreditAccountId), "حدد حساب الدائن أو فعّل استخدام الحساب المالي من الحركة");

        if (vm.UseSourceFinancialAccountAsDebit && vm.UseSourceFinancialAccountAsCredit)
            ModelState.AddModelError(string.Empty, "لا يمكن جعل طرفي القيد يعتمدان على الحساب المالي من الحركة معًا");

        if (vm.Priority <= 0)
            ModelState.AddModelError(nameof(vm.Priority), "الأولوية يجب أن تكون أكبر من صفر");
    }

    private static string Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();

    private static string? NormalizeNullable(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
