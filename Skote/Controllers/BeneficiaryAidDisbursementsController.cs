using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentServices.Charity.Beneficiaries;
using InfrastructureManagmentServices.Charity.Funding;
using InfrastructureManagmentServices.Charity;
using InfrastructureManagmentServices.Charity.Workflow;
using InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.AidDisbursements;
using InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.Shared;
using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Domains.Accounting.Integration;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using InfrastrfuctureManagmentCore.Domains.Charity.Funders;
using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Donors;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Funders;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Lookups;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Skote.Helpers;
using System.Security.Claims;
using InfrastrfuctureManagmentCore.Domains.Charity.StockAdvanced;

[Authorize(Policy = CharityPolicies.AidDisbursementView)]
public class BeneficiaryAidDisbursementsController : Controller
{
    private readonly IBeneficiaryAidDisbursementRepository _repository;
    private readonly IBeneficiaryAidRequestRepository _aidRequestRepository;
    private readonly IBeneficiaryRepository _beneficiaryRepository;
    private readonly ICharityLookupRepository _lookupRepository;
    private readonly ICharityProjectRepository _projectRepository;
    private readonly IGrantAgreementRepository _grantAgreementRepository;
    private readonly IWorkflowService _workflowService;
    private readonly IOperationalJournalHookService _operationalJournalHookService;
    private readonly IAidRequestFundingService _aidRequestFundingService;
    private readonly IAidDisbursementFundingLineService _aidDisbursementFundingLineService;
    private readonly AppDbContext _db;
    private const string AutoInKindDisbursementSourceType = "DonationAllocationAutoInKindDisbursement";
    private const string PurchaseNeedRequestSourceType = "PurchaseNeedRequest";

    public BeneficiaryAidDisbursementsController(
        IBeneficiaryAidDisbursementRepository repository,
        IBeneficiaryAidRequestRepository aidRequestRepository,
        IBeneficiaryRepository beneficiaryRepository,
        ICharityLookupRepository lookupRepository,
        ICharityProjectRepository projectRepository,
        IGrantAgreementRepository grantAgreementRepository,
        IOperationalJournalHookService operationalJournalHookService,
        IAidRequestFundingService aidRequestFundingService,
        IAidDisbursementFundingLineService aidDisbursementFundingLineService,
        AppDbContext db,
        IWorkflowService workflowService)
    {
        _repository = repository;
        _workflowService = workflowService;
        _aidRequestRepository = aidRequestRepository;
        _beneficiaryRepository = beneficiaryRepository;
        _lookupRepository = lookupRepository;
        _projectRepository = projectRepository;
        _grantAgreementRepository = grantAgreementRepository;
        _operationalJournalHookService = operationalJournalHookService;
        _aidRequestFundingService = aidRequestFundingService;
        _aidDisbursementFundingLineService = aidDisbursementFundingLineService;
        _db = db;
    }

    public async Task<IActionResult> Index(Guid? beneficiaryId = null)
    {
        if (beneficiaryId.HasValue && beneficiaryId.Value != Guid.Empty)
        {
            if (!await PopulateBeneficiaryAsync(beneficiaryId.Value))
                return NotFound();
        }
        else
        {
            ClearBeneficiaryContext();
        }

        var items = beneficiaryId.HasValue && beneficiaryId.Value != Guid.Empty
            ? await _repository.GetByBeneficiaryIdAsync(beneficiaryId.Value)
            : await _repository.GetAllAsync();

        var projectIds = items.Where(x => x.ProjectId.HasValue).Select(x => x.ProjectId!.Value).Distinct().ToList();
        var donationIds = items.Where(x => x.DonationId.HasValue).Select(x => x.DonationId!.Value).Distinct().ToList();
        var grantIds = items.Where(x => x.GrantAgreementId.HasValue).Select(x => x.GrantAgreementId!.Value).Distinct().ToList();
        var sourceIds = items.Select(x => x.Id).ToList();
        var storeIssueIds = items.Where(x => x.StoreIssueId.HasValue).Select(x => x.StoreIssueId!.Value).Distinct().ToList();

        var projectMap = await _db.Set<CharityProject>()
            .AsNoTracking()
            .Where(x => projectIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => $"{x.Code} - {x.Name}");

        var donationMap = await _db.Set<Donation>()
            .AsNoTracking()
            .Where(x => donationIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.DonationNumber);

        var grantMap = await _db.Set<GrantAgreement>()
            .AsNoTracking()
            .Where(x => grantIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.AgreementNumber);

        var postedIds = await _db.Set<JournalEntry>()
            .AsNoTracking()
            .Where(x => x.SourceType == AccountingSourceTypes.BeneficiaryAidDisbursement && x.SourceId.HasValue && sourceIds.Contains(x.SourceId.Value) && x.Status != JournalEntryStatus.Reversed)
            .Select(x => x.SourceId!.Value)
            .Distinct()
            .ToListAsync();
        var postedSet = postedIds.ToHashSet();

        var storeIssueMap = storeIssueIds.Count == 0
            ? new Dictionary<Guid, string>()
            : await _db.Set<CharityStoreIssue>()
                .AsNoTracking()
                .Where(x => storeIssueIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, x => x.IssueNumber);
       
        var model = items.Select(x =>
        {
            var fundingTraceSummary = BuildFundingTraceSummary(x);
            var donationNumber = x.DonationId.HasValue && donationMap.TryGetValue(x.DonationId.Value, out var directDonationNumber)
                ? directDonationNumber
                : null;
     
            if (string.IsNullOrWhiteSpace(donationNumber) && !string.IsNullOrWhiteSpace(fundingTraceSummary))
            {
                donationNumber = x.FundingLines
                    .Where(f => f.DonationAllocation?.Donation != null)
                    .Select(f => f.DonationAllocation!.Donation!.DonationNumber)
                    .Distinct()
                    .Count() > 1
                    ? "متعدد"
                    : x.FundingLines
                        .Where(f => f.DonationAllocation?.Donation != null)
                        .Select(f => f.DonationAllocation!.Donation!.DonationNumber)
                        .FirstOrDefault();
            }
         //   var executionStatus = string.IsNullOrWhiteSpace(x.ExecutionStatus) ? AidDisbursementExecutionStatusCodes.Available : x.ExecutionStatus;
           // var isInKindDisbursement = string.Equals(x.SourceType, AutoInKindDisbursementSourceType, StringComparison.OrdinalIgnoreCase) || x.StoreIssueId.HasValue;
            var isInKindDisbursement = x.StoreIssueId.HasValue
             || string.Equals(x.SourceType, AutoInKindDisbursementSourceType, StringComparison.OrdinalIgnoreCase)
    || string.Equals(x.SourceType, PurchaseNeedRequestSourceType, StringComparison.OrdinalIgnoreCase)
    || (x.FundingLines?.Any(f => f.DonationAllocation?.DonationInKindItemId.HasValue == true) ?? false)
    || (!string.IsNullOrWhiteSpace(x.Notes) && x.Notes.Contains("عيني", StringComparison.OrdinalIgnoreCase));

            var executionStatusCode = string.IsNullOrWhiteSpace(x.ExecutionStatus)
                ? AidDisbursementExecutionStatusCodes.Available
                : x.ExecutionStatus;

            var canCreateStoreIssue = isInKindDisbursement
                && string.Equals(x.ApprovalStatus, AidDisbursementApprovalStatusCodes.Approved, StringComparison.OrdinalIgnoreCase)
                && !x.StoreIssueId.HasValue
                && !string.Equals(executionStatusCode, AidDisbursementExecutionStatusCodes.FullyDisbursed, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(executionStatusCode, AidDisbursementExecutionStatusCodes.Cancelled, StringComparison.OrdinalIgnoreCase);
            return new BeneficiaryAidDisbursementListItemVm
            {
                Id = x.Id,
                BeneficiaryId = x.BeneficiaryId,
                BeneficiaryCode = x.Beneficiary?.Code,
                BeneficiaryName = x.Beneficiary?.FullName,
                DisbursementDate = x.DisbursementDate,
                AidType = x.AidType?.NameAr,
                Amount = x.Amount,
                PaymentMethod = x.PaymentMethod?.MethodNameAr,
                FinancialAccount = x.FinancialAccount?.AccountNameAr,
                LinkedRequestStatus = x.AidRequest?.Status,
                ProjectName = x.ProjectId.HasValue && projectMap.TryGetValue(x.ProjectId.Value, out var projectName) ? projectName : null,
                DonationNumber = donationNumber,
                FundingTraceSummary = fundingTraceSummary,
                GrantAgreementNumber = x.GrantAgreementId.HasValue && grantMap.TryGetValue(x.GrantAgreementId.Value, out var agreementNumber) ? agreementNumber : null,
                IsJournalPosted = postedSet.Contains(x.Id),
                Notes = x.Notes,
                ApprovalStatusCode = x.ApprovalStatus,
                ApprovalStatusName = AidDisbursementApprovalStatusCodes.ToDisplayName(x.ApprovalStatus),
                ApprovedAtUtc = x.ApprovedAtUtc,
                RejectedAtUtc = x.RejectedAtUtc,
                ExecutionStatusCode = executionStatusCode,
                ExecutionStatusName = AidDisbursementExecutionStatusCodes.ToDisplayName(executionStatusCode),
                ExecutedAmount = x.ExecutedAmount,
                ExecutedAtUtc = x.ExecutedAtUtc,
                IsInKindDisbursement = isInKindDisbursement,
                StoreIssueId = x.StoreIssueId,
                StoreIssueNumber = x.StoreIssueId.HasValue && storeIssueMap.TryGetValue(x.StoreIssueId.Value, out var issueNumber) ? issueNumber : null,
                CanCreateStoreIssue = canCreateStoreIssue,
                CanExecuteCash = !isInKindDisbursement
                    && string.Equals(x.ApprovalStatus, AidDisbursementApprovalStatusCodes.Approved, StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(executionStatusCode, AidDisbursementExecutionStatusCodes.FullyDisbursed, StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(executionStatusCode, AidDisbursementExecutionStatusCodes.Cancelled, StringComparison.OrdinalIgnoreCase)
                    && (x.Amount ?? 0m) > 0m
            };
        }).ToList();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Print(Guid id)
    {
        var entity = await _db.Set<BeneficiaryAidDisbursement>()
            .AsNoTracking()
            .Include(x => x.Beneficiary)
            .Include(x => x.AidType)
            .Include(x => x.PaymentMethod)
            .Include(x => x.FinancialAccount)
            .Include(x => x.AidRequest)
            .Include(x => x.FundingLines)
                .ThenInclude(x => x.DonationAllocation)
                    .ThenInclude(x => x!.Donation)
            .Include(x => x.FundingLines)
                .ThenInclude(x => x.DonationAllocation)
                    .ThenInclude(x => x!.DonationInKindItem)
                        .ThenInclude(x => x!.Item)
            .Include(x => x.FundingLines)
                .ThenInclude(x => x.DonationAllocation)
                    .ThenInclude(x => x!.DonationInKindItem)
                        .ThenInclude(x => x!.Warehouse)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
            return NotFound();

        CharityStoreIssue? storeIssue = null;
        if (entity.StoreIssueId.HasValue)
        {
            storeIssue = await _db.Set<CharityStoreIssue>()
                .AsNoTracking()
                .Include(x => x.Warehouse)
                .Include(x => x.Lines)
                    .ThenInclude(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == entity.StoreIssueId.Value);
        }

        var inKindItems = BuildInKindPrintItems(entity, storeIssue);

        ViewBag.ApprovalStatusName = AidDisbursementApprovalStatusCodes.ToDisplayName(entity.ApprovalStatus);
        ViewBag.ExecutionStatusName = AidDisbursementExecutionStatusCodes.ToDisplayName(entity.ExecutionStatus);
        ViewBag.FundingTraceSummary = BuildFundingTraceSummary(entity);
        ViewBag.PrintedAt = DateTime.Now;
        ViewBag.IsInKindDisbursement = inKindItems.Count > 0 || entity.StoreIssueId.HasValue;
        ViewBag.InKindItems = inKindItems;
        ViewBag.StoreIssueNumber = storeIssue?.IssueNumber;
        ViewBag.StoreIssueDate = storeIssue?.IssueDate;
        ViewBag.StoreIssueWarehouseName = storeIssue?.Warehouse?.WarehouseNameAr;

        return View(entity);
    }

    [Authorize(Policy = CharityPolicies.AidDisbursementManage)]
    [HttpGet]
    public async Task<IActionResult> Create(Guid? beneficiaryId = null)
    {
        if (beneficiaryId.HasValue && beneficiaryId.Value != Guid.Empty)
        {
            if (!await PopulateBeneficiaryAsync(beneficiaryId.Value))
                return NotFound();

            var committeeValidation = await BeneficiaryCommitteeDecisionGuard.ValidateHasApprovedDecisionAsync(_db, beneficiaryId.Value, "إضافة صرف مساعدة");
            if (!committeeValidation.IsValid)
            {
                TempData["Warning"] = committeeValidation.Message;
                return RedirectToAction("Details", "Beneficiaries", new { id = beneficiaryId.Value });
            }
        }
        else
        {
            ClearBeneficiaryContext();
        }

        var vm = new CreateBeneficiaryAidDisbursementVm
        {
            BeneficiaryId = beneficiaryId ?? Guid.Empty,
            DisbursementDate = DateTime.Today
        };

        await FillLookupsAsync(vm);
        return View(vm);
    }

    [Authorize(Policy = CharityPolicies.AidDisbursementManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBeneficiaryAidDisbursementVm vm)
    {
        if (vm.BeneficiaryId == Guid.Empty)
            ModelState.AddModelError(nameof(vm.BeneficiaryId), "المستفيد مطلوب");

        if (vm.BeneficiaryId != Guid.Empty && !await PopulateBeneficiaryAsync(vm.BeneficiaryId))
            return NotFound();

        await ApplyAidRequestDefaultsAsync(vm);
        await ValidateFundingLinksAsync(vm);
        await FillLookupsAsync(vm);

        if (!ModelState.IsValid)
            return View(vm);

        var beneficiaryName = ViewBag.BeneficiaryName as string;

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var initialApprovalStatus = await ResolveInitialApprovalStatusAsync(vm);
        var autoApproved = string.Equals(initialApprovalStatus, AidDisbursementApprovalStatusCodes.Approved, StringComparison.OrdinalIgnoreCase);

        var entity = new BeneficiaryAidDisbursement
        {
            Id = Guid.NewGuid(),
            BeneficiaryId = vm.BeneficiaryId,
            AidRequestId = vm.AidRequestId,
            AidTypeId = vm.AidTypeId,
            DisbursementDate = vm.DisbursementDate,
            Amount = vm.Amount,
            PaymentMethodId = vm.PaymentMethodId,
            FinancialAccountId = vm.FinancialAccountId,
            ProjectId = vm.ProjectId,
            DonationId = vm.DonationId,
            GrantAgreementId = vm.GrantAgreementId,
            Notes = vm.Notes?.Trim(),
            ApprovalStatus = initialApprovalStatus,
            ApprovedByUserId = autoApproved ? currentUserId : null,
            ApprovedAtUtc = autoApproved ? DateTime.UtcNow : null,
            RejectedByUserId = null,
            RejectedAtUtc = null,
            CreatedByUserId = currentUserId,
            CreatedAtUtc = DateTime.UtcNow
        };

        await using (var tx = await _db.Database.BeginTransactionAsync())
        {
            await _repository.AddAsync(entity);

            var fundingResult = await _aidDisbursementFundingLineService.RebuildFundingLinesAsync(entity, currentUserId);
            if (!fundingResult.IsSuccess)
            {
                await tx.RollbackAsync();
                ModelState.AddModelError(nameof(vm.Amount), fundingResult.ErrorMessage ?? "تعذر ربط الصرف بسطور التخصيص.");
                await FillLookupsAsync(vm);
                return View(vm);
            }

            await tx.CommitAsync();
        }

        await _workflowService.InitiateAsync(
            entityType: "AidDisbursement",
            entityId: entity.Id,
            entityTitle: beneficiaryName ?? "صرف مساعدة",
            submittedByUserId: entity.CreatedByUserId);

        if (autoApproved)
        {
            await RefreshAidRequestStatusAsync(vm.AidRequestId);
            var postingResult = await _operationalJournalHookService.TryCreateBeneficiaryAidDisbursementEntryAsync(entity.Id);
            TempData[postingResult.IsSuccess ? "Success" : "Warning"] = postingResult.IsSuccess
                ? $"تم تسجيل الصرف واعتماده تلقائيًا. {postingResult.Message}"
                : postingResult.Message;
        }
        else
        {
            TempData["Success"] = "تم تسجيل الصرف بنجاح وبحالة انتظار الاعتماد.";
        }

        return RedirectToAction(nameof(Index), new { beneficiaryId = vm.BeneficiaryId });
    }

    [Authorize(Policy = CharityPolicies.AidDisbursementManage)]
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        if (!await PopulateBeneficiaryAsync(entity.BeneficiaryId))
            return NotFound();

        if (string.Equals(entity.ApprovalStatus, AidDisbursementApprovalStatusCodes.Approved, StringComparison.OrdinalIgnoreCase))
        {
            TempData["Warning"] = "لا يمكن تعديل صرف معتمد. استخدم الرفض أو الإلغاء الرسمي إذا لزم الأمر.";
            return RedirectToAction(nameof(Index), new { beneficiaryId = entity.BeneficiaryId });
        }

        var vm = new EditBeneficiaryAidDisbursementVm
        {
            Id = entity.Id,
            BeneficiaryId = entity.BeneficiaryId,
            AidRequestId = entity.AidRequestId,
            AidTypeId = entity.AidTypeId,
            DisbursementDate = entity.DisbursementDate,
            Amount = entity.Amount,
            PaymentMethodId = entity.PaymentMethodId,
            FinancialAccountId = entity.FinancialAccountId,
            ProjectId = entity.ProjectId,
            DonationId = entity.DonationId,
            GrantAgreementId = entity.GrantAgreementId,
            Notes = entity.Notes
        };

        await FillLookupsAsync(vm, entity.Id);
        return View(vm);
    }

    [Authorize(Policy = CharityPolicies.AidDisbursementManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditBeneficiaryAidDisbursementVm vm)
    {
        var entity = await _repository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        if (string.Equals(entity.ApprovalStatus, AidDisbursementApprovalStatusCodes.Approved, StringComparison.OrdinalIgnoreCase))
        {
            TempData["Warning"] = "لا يمكن تعديل صرف معتمد.";
            return RedirectToAction(nameof(Index), new { beneficiaryId = entity.BeneficiaryId });
        }

        if (vm.BeneficiaryId == Guid.Empty)
            ModelState.AddModelError(nameof(vm.BeneficiaryId), "المستفيد مطلوب");

        if (vm.BeneficiaryId != Guid.Empty && !await PopulateBeneficiaryAsync(vm.BeneficiaryId))
            return NotFound();

        await ApplyAidRequestDefaultsAsync(vm, vm.Id);
        await ValidateFundingLinksAsync(vm, vm.Id);
        await FillLookupsAsync(vm, vm.Id);

        if (!ModelState.IsValid)
            return View(vm);

        entity.BeneficiaryId = vm.BeneficiaryId;
        entity.AidRequestId = vm.AidRequestId;
        entity.AidTypeId = vm.AidTypeId;
        entity.DisbursementDate = vm.DisbursementDate;
        entity.Amount = vm.Amount;
        entity.PaymentMethodId = vm.PaymentMethodId;
        entity.FinancialAccountId = vm.FinancialAccountId;
        entity.ProjectId = vm.ProjectId;
        entity.DonationId = vm.DonationId;
        entity.GrantAgreementId = vm.GrantAgreementId;
        entity.Notes = vm.Notes?.Trim();

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var initialApprovalStatus = await ResolveInitialApprovalStatusAsync(vm);
        var autoApproved = string.Equals(initialApprovalStatus, AidDisbursementApprovalStatusCodes.Approved, StringComparison.OrdinalIgnoreCase);

        entity.ApprovalStatus = initialApprovalStatus;
        entity.ApprovedByUserId = autoApproved ? currentUserId : null;
        entity.ApprovedAtUtc = autoApproved ? DateTime.UtcNow : null;
        entity.RejectedByUserId = null;
        entity.RejectedAtUtc = null;
        await using (var tx = await _db.Database.BeginTransactionAsync())
        {
            await _repository.UpdateAsync(entity);

            var fundingResult = await _aidDisbursementFundingLineService.RebuildFundingLinesAsync(entity, currentUserId);
            if (!fundingResult.IsSuccess)
            {
                await tx.RollbackAsync();
                ModelState.AddModelError(nameof(vm.Amount), fundingResult.ErrorMessage ?? "تعذر إعادة ربط الصرف بسطور التخصيص.");
                await FillLookupsAsync(vm, vm.Id);
                return View(vm);
            }

            await tx.CommitAsync();
        }

        if (autoApproved)
        {
            await RefreshAidRequestStatusAsync(entity.AidRequestId);
            var postingResult = await _operationalJournalHookService.TryCreateBeneficiaryAidDisbursementEntryAsync(entity.Id);
            TempData[postingResult.IsSuccess ? "Success" : "Warning"] = postingResult.IsSuccess
                ? $"تم تعديل الصرف واعتماده تلقائيًا. {postingResult.Message}"
                : postingResult.Message;
        }
        else
        {
            TempData["Success"] = "تم تعديل الصرف وإعادته إلى انتظار الاعتماد.";
        }

        return RedirectToAction(nameof(Index), new { beneficiaryId = entity.BeneficiaryId });
    }
    
    [HttpGet]
    public async Task<IActionResult> ExecuteFromTreasury(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        if (!string.Equals(entity.ApprovalStatus, "Approved", StringComparison.OrdinalIgnoreCase))
        {
            TempData["Warning"] = "لا يمكن تنفيذ الصرف من الخزينة قبل اعتماد الصرف.";
            return RedirectToAction(nameof(Index), new { beneficiaryId = entity.BeneficiaryId });
        }

        if (string.Equals(entity.ExecutionStatus, "Paid", StringComparison.OrdinalIgnoreCase)
            || string.Equals(entity.ExecutionStatus, "Executed", StringComparison.OrdinalIgnoreCase))
        {
            TempData["Warning"] = "تم تنفيذ هذا الصرف من قبل.";
            return RedirectToAction(nameof(Index), new { beneficiaryId = entity.BeneficiaryId });
        }

        return View(entity);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExecuteFromTreasury1(Guid id, Guid? beneficiaryId)
    {
        var entity = await _db.Set<BeneficiaryAidDisbursement>()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
            return NotFound();

        if (!string.Equals(entity.ApprovalStatus, AidDisbursementApprovalStatusCodes.Approved, StringComparison.OrdinalIgnoreCase))
        {
            TempData["Warning"] = "لا يمكن تنفيذ الصرف من الخزينة قبل اعتماد الصرف.";
            return RedirectToAction(nameof(Index), new { beneficiaryId = beneficiaryId ?? entity.BeneficiaryId });
        }

        if (string.Equals(entity.ExecutionStatus, AidDisbursementExecutionStatusCodes.FullyDisbursed, StringComparison.OrdinalIgnoreCase))
        {
            TempData["Warning"] = "تم تنفيذ هذا الصرف من قبل.";
            return RedirectToAction(nameof(Index), new { beneficiaryId = beneficiaryId ?? entity.BeneficiaryId });
        }

        entity.ExecutionStatus = AidDisbursementExecutionStatusCodes.FullyDisbursed;
        entity.ExecutedAmount = entity.Amount ?? 0m;
        entity.ExecutedAtUtc = DateTime.UtcNow;
        entity.ExecutedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(entity.Notes))
            entity.Notes = "تم تنفيذ الصرف من الخزينة.";
        else
            entity.Notes += "\nتم تنفيذ الصرف من الخزينة.";

        await _db.SaveChangesAsync();

        TempData["Success"] = "تم تنفيذ الصرف من الخزينة وتحديث الحالة إلى تم الصرف.";
        return RedirectToAction(nameof(Index), new { beneficiaryId = beneficiaryId ?? entity.BeneficiaryId });
    }

    [Authorize(Policy = CharityPolicies.AidDisbursementManage)]

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(Guid id, Guid? beneficiaryId = null)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        if (string.Equals(entity.ApprovalStatus, AidDisbursementApprovalStatusCodes.Approved, StringComparison.OrdinalIgnoreCase))
        {
            TempData["Info"] = "هذا الصرف معتمد بالفعل.";
            return RedirectToAction(nameof(Index), new { beneficiaryId = beneficiaryId ?? entity.BeneficiaryId });
        }

        if (string.Equals(entity.ApprovalStatus, AidDisbursementApprovalStatusCodes.Rejected, StringComparison.OrdinalIgnoreCase))
        {
            TempData["Warning"] = "لا يمكن اعتماد صرف مرفوض. عدّل الصرف أولًا لإعادته إلى انتظار الاعتماد.";
            return RedirectToAction(nameof(Index), new { beneficiaryId = beneficiaryId ?? entity.BeneficiaryId });
        }

        entity.ApprovalStatus = AidDisbursementApprovalStatusCodes.Approved;
        entity.ApprovedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        entity.ApprovedAtUtc = DateTime.UtcNow;
        entity.RejectedByUserId = null;
        entity.RejectedAtUtc = null;
        await _repository.UpdateAsync(entity);

        await RefreshAidRequestStatusAsync(entity.AidRequestId);

        var postingResult = await _operationalJournalHookService.TryCreateBeneficiaryAidDisbursementEntryAsync(entity.Id);
        TempData[postingResult.IsSuccess ? "Success" : "Warning"] = postingResult.IsSuccess
            ? $"تم اعتماد الصرف بنجاح. {postingResult.Message}"
            : postingResult.Message;

        return RedirectToAction(nameof(Index), new { beneficiaryId = beneficiaryId ?? entity.BeneficiaryId });
    }

    [Authorize(Policy = CharityPolicies.AidDisbursementManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(Guid id, Guid? beneficiaryId = null)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        if (string.Equals(entity.ApprovalStatus, AidDisbursementApprovalStatusCodes.Approved, StringComparison.OrdinalIgnoreCase))
        {
            TempData["Warning"] = "لا يمكن رفض صرف تم اعتماده بالفعل.";
            return RedirectToAction(nameof(Index), new { beneficiaryId = beneficiaryId ?? entity.BeneficiaryId });
        }

        if (string.Equals(entity.ApprovalStatus, AidDisbursementApprovalStatusCodes.Rejected, StringComparison.OrdinalIgnoreCase))
        {
            TempData["Info"] = "هذا الصرف مرفوض بالفعل.";
            return RedirectToAction(nameof(Index), new { beneficiaryId = beneficiaryId ?? entity.BeneficiaryId });
        }

        await using var tx = await _db.Database.BeginTransactionAsync();
        var oldLines = await _db.Set<BeneficiaryAidDisbursementFundingLine>()
            .Where(x => x.DisbursementId == entity.Id)
            .ToListAsync();

        if (oldLines.Count > 0)
            _db.Set<BeneficiaryAidDisbursementFundingLine>().RemoveRange(oldLines);

        entity.ApprovalStatus = AidDisbursementApprovalStatusCodes.Rejected;
        entity.RejectedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        entity.RejectedAtUtc = DateTime.UtcNow;
        entity.ApprovedByUserId = null;
        entity.ApprovedAtUtc = null;
        entity.DonationId = null;

        await _repository.UpdateAsync(entity);
        await tx.CommitAsync();

        await RefreshAidRequestStatusAsync(entity.AidRequestId);
        TempData["Success"] = "تم رفض الصرف وإلغاء حجز التمويل المرتبط به.";
        return RedirectToAction(nameof(Index), new { beneficiaryId = beneficiaryId ?? entity.BeneficiaryId });
    }
    [Authorize(Policy = CharityPolicies.StoresManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateStoreIssueFromDisbursement(Guid id, Guid? beneficiaryId = null)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var disbursement = await _db.Set<BeneficiaryAidDisbursement>()
            .Include(x => x.Beneficiary)
            .Include(x => x.AidRequest)
            .Include(x => x.FundingLines)
                .ThenInclude(x => x.DonationAllocation)
                    .ThenInclude(x => x!.DonationInKindItem)
                        .ThenInclude(x => x!.Item)
            .Include(x => x.FundingLines)
                .ThenInclude(x => x.DonationAllocation)
                    .ThenInclude(x => x!.Donation)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (disbursement == null)
            return NotFound();

        var redirectBeneficiaryId = beneficiaryId ?? disbursement.BeneficiaryId;

        if (!string.Equals(disbursement.ApprovalStatus, AidDisbursementApprovalStatusCodes.Approved, StringComparison.OrdinalIgnoreCase))
        {
            TempData["Warning"] = "لا يمكن إنشاء إذن صرف مخزني قبل اعتماد سجل الصرف.";
            return RedirectToAction(nameof(Index), new { beneficiaryId = redirectBeneficiaryId });
        }

        if (disbursement.StoreIssueId.HasValue)
        {
            TempData["Info"] = "تم إنشاء إذن صرف مخزني لهذا السجل من قبل.";
            return RedirectToAction("Details", "CharityStoreIssues", new { id = disbursement.StoreIssueId.Value });
        }

        if (string.Equals(disbursement.ExecutionStatus, AidDisbursementExecutionStatusCodes.FullyDisbursed, StringComparison.OrdinalIgnoreCase))
        {
            TempData["Warning"] = "لا يمكن إنشاء إذن صرف مخزني لأن سجل الصرف منفذ بالكامل.";
            return RedirectToAction(nameof(Index), new { beneficiaryId = redirectBeneficiaryId });
        }

        if (string.Equals(disbursement.SourceType, PurchaseNeedRequestSourceType, StringComparison.OrdinalIgnoreCase))
            return await CreateStoreIssueFromPurchaseNeedDisbursementAsync(disbursement, redirectBeneficiaryId, currentUserId);

        var allocations = disbursement.FundingLines
            .Where(x => x.DonationAllocation?.DonationInKindItem != null
                && (x.DonationAllocation.AllocatedQuantity ?? 0m) > 0m)
            .Select(x => x.DonationAllocation!)
            .GroupBy(x => x.Id)
            .Select(x => x.First())
            .ToList();

        if (allocations.Count == 0)
        {
            TempData["Warning"] = "لا توجد تخصيصات عينية مرتبطة بسجل الصرف لإنشاء إذن صرف مخزني.";
            return RedirectToAction(nameof(Index), new { beneficiaryId = redirectBeneficiaryId });
        }

        var missingWarehouse = allocations.FirstOrDefault(x => !x.DonationInKindItem!.WarehouseId.HasValue);
        if (missingWarehouse != null)
        {
            TempData["Warning"] = "لا يمكن إنشاء إذن صرف مخزني لأن أحد أصناف التبرع العيني غير مرتبط بمخزن.";
            return RedirectToAction(nameof(Index), new { beneficiaryId = redirectBeneficiaryId });
        }

        var warehouseIds = allocations
            .Select(x => x.DonationInKindItem!.WarehouseId!.Value)
            .Distinct()
            .ToList();

        if (warehouseIds.Count > 1)
        {
            TempData["Warning"] = "التخصيصات العينية مرتبطة بأكثر من مخزن. يلزم فصل الصرف حسب المخزن.";
            return RedirectToAction(nameof(Index), new { beneficiaryId = redirectBeneficiaryId });
        }

        var now = DateTime.UtcNow;

        var issue = new CharityStoreIssue
        {
            Id = Guid.NewGuid(),
            IssueNumber = await GenerateStoreIssueNumberAsync(),
            WarehouseId = warehouseIds[0],
            IssueDate = DateTime.Today,
            IssueType = "Beneficiary",
            BeneficiaryId = disbursement.BeneficiaryId,
            ProjectId = disbursement.ProjectId ?? disbursement.AidRequest?.ProjectId,
            IssuedToName = disbursement.Beneficiary?.FullName,
            ApprovalStatus = "Pending",
            CreatedAtUtc = now,
            CreatedByUserId = currentUserId,
            Lines = new List<CharityStoreIssueLine>(),
            Notes = BuildAutoStoreIssueNotes(disbursement, allocations)
        };

        foreach (var allocation in allocations)
        {
            var inKindItem = allocation.DonationInKindItem!;
            var quantity = allocation.AllocatedQuantity ?? 0m;
            var unitCost = inKindItem.EstimatedUnitValue ?? 0m;

            issue.Lines.Add(new CharityStoreIssueLine
            {
                Id = Guid.NewGuid(),
                ItemId = inKindItem.ItemId,
                Quantity = quantity,
                UnitCost = unitCost,
                Notes = BuildAutoStoreIssueLineNotes(allocation, disbursement)
            });
        }

        await _db.Set<CharityStoreIssue>().AddAsync(issue);

        disbursement.StoreIssueId = issue.Id;
        disbursement.ExecutionStatus = AidDisbursementExecutionStatusCodes.Available;
        disbursement.Notes = AppendNote(disbursement.Notes, $"تم إنشاء إذن صرف مخزني رقم {issue.IssueNumber} وحالته انتظار الاعتماد.");

        await _db.SaveChangesAsync();

        TempData["Success"] = "تم إنشاء إذن الصرف المخزني من سجل الصرف. يمكنك مراجعته وطباعته ثم اعتماده.";
        return RedirectToAction("Details", "CharityStoreIssues", new { id = issue.Id });
    }



    // ══════════════ Patch E: إنشاء إذن صرف للمستفيد من طلب احتياج تم شراؤه وإضافته للمخزن ══════════════
    private async Task<IActionResult> CreateStoreIssueFromPurchaseNeedDisbursementAsync(
        BeneficiaryAidDisbursement disbursement,
        Guid redirectBeneficiaryId,
        string? currentUserId)
    {
        var needRequestId = disbursement.SourceId;
        if (!needRequestId.HasValue && TryExtractGuidMarker(disbursement.Notes, "StockNeedRequestId:", out var parsedNeedRequestId))
            needRequestId = parsedNeedRequestId;

        if (!needRequestId.HasValue || needRequestId.Value == Guid.Empty)
        {
            TempData["Warning"] = "سجل الصرف غير مرتبط بطلب احتياج شراء.";
            return RedirectToAction(nameof(Index), new { beneficiaryId = redirectBeneficiaryId });
        }

        var needRequest = await _db.Set<StockNeedRequest>()
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == needRequestId.Value);

        if (needRequest == null)
        {
            TempData["Warning"] = "تعذر العثور على طلب الاحتياج المرتبط بسجل الصرف.";
            return RedirectToAction(nameof(Index), new { beneficiaryId = redirectBeneficiaryId });
        }

        var receipt = await _db.Set<CharityStoreReceipt>()
            .Include(x => x.Lines)
            .Where(x => x.ApprovalStatus == "Approved"
                && x.SourceType == PurchaseNeedRequestSourceType
                && x.Notes != null
                && x.Notes.Contains($"StockNeedRequestId:{needRequest.Id}"))
            .OrderByDescending(x => x.ApprovedAtUtc ?? x.CreatedAtUtc)
            .FirstOrDefaultAsync();

        if (receipt == null)
        {
            TempData["Warning"] = "لا يوجد إذن إضافة مخزني معتمد مرتبط بطلب الاحتياج. اعتمد إذن الإضافة أولاً.";
            return RedirectToAction(nameof(Index), new { beneficiaryId = redirectBeneficiaryId });
        }

        var receiptLines = receipt.Lines
            .Where(x => x.ItemId != Guid.Empty && x.Quantity > 0m)
            .ToList();

        if (receiptLines.Count == 0)
        {
            TempData["Warning"] = "إذن الإضافة المعتمد لا يحتوي على أصناف صالحة للصرف.";
            return RedirectToAction(nameof(Index), new { beneficiaryId = redirectBeneficiaryId });
        }

        var now = DateTime.UtcNow;
        var issue = new CharityStoreIssue
        {
            Id = Guid.NewGuid(),
            IssueNumber = await GenerateStoreIssueNumberAsync(),
            WarehouseId = receipt.WarehouseId,
            IssueDate = DateTime.Today,
            IssueType = "Beneficiary",
            BeneficiaryId = disbursement.BeneficiaryId,
            ProjectId = disbursement.ProjectId ?? disbursement.AidRequest?.ProjectId ?? needRequest.ProjectId,
            IssuedToName = disbursement.Beneficiary?.FullName,
            ApprovalStatus = "Pending",
            CreatedAtUtc = now,
            CreatedByUserId = currentUserId,
            Notes = $"إذن صرف مخزني بناءً على طلب احتياج شراء. AidDisbursementId:{disbursement.Id} | StockNeedRequestId:{needRequest.Id} | StoreReceiptId:{receipt.Id} | BeneficiaryAidRequestId:{needRequest.BeneficiaryAidRequestId} | BeneficiaryAidRequestLineId:{needRequest.BeneficiaryAidRequestLineId}",
            Lines = receiptLines.Select(line => new CharityStoreIssueLine
            {
                Id = Guid.NewGuid(),
                ItemId = line.ItemId,
                Quantity = line.Quantity,
                UnitCost = line.UnitCost,
                Notes = $"صرف للمستفيد من طلب الاحتياج {needRequest.RequestNumber}. StoreReceiptId:{receipt.Id} | StockNeedRequestId:{needRequest.Id}"
            }).ToList()
        };

        await _db.Set<CharityStoreIssue>().AddAsync(issue);

        disbursement.StoreIssueId = issue.Id;
        disbursement.ExecutionStatus = AidDisbursementExecutionStatusCodes.Available;
        disbursement.Notes = AppendNote(disbursement.Notes, $"تم إنشاء إذن صرف مخزني رقم {issue.IssueNumber} من طلب الاحتياج {needRequest.RequestNumber} وحالته انتظار الاعتماد.");

        await _db.SaveChangesAsync();

        TempData["Success"] = "تم إنشاء إذن الصرف المخزني من طلب الاحتياج. يمكنك مراجعته وطباعته ثم اعتماده.";
        return RedirectToAction("Details", "CharityStoreIssues", new { id = issue.Id });
    }

    private static bool TryExtractGuidMarker(string? text, string marker, out Guid value)
    {
        value = Guid.Empty;
        if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(marker))
            return false;

        var start = text.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (start < 0)
            return false;

        start += marker.Length;
        var remainder = text[start..].Trim();
        var token = remainder.Split(new[] { ' ', '|', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        return Guid.TryParse(token, out value);
    }

    private async Task<string> ResolveInitialApprovalStatusAsync(CreateBeneficiaryAidDisbursementVm vm)
    {
        if (await HasAutoApprovedSourceAsync(vm))
            return AidDisbursementApprovalStatusCodes.Approved;

        return AidDisbursementApprovalStatusCodes.Pending;
    }

    private async Task<bool> HasAutoApprovedSourceAsync(CreateBeneficiaryAidDisbursementVm vm)
    {
        if (!vm.AidRequestId.HasValue)
            return false;

        if (vm.DonationId.HasValue)
        {
            return await _db.Set<DonationAllocation>()
                .AsNoTracking()
                .AnyAsync(x => x.AidRequestId == vm.AidRequestId
                    && x.DonationId == vm.DonationId
                    && x.ApprovalStatus == DonationAllocationApprovalStatusCodes.Approved);
        }

        return false;
    }

    private async Task ApplyAidRequestDefaultsAsync(CreateBeneficiaryAidDisbursementVm vm, Guid? currentDisbursementId = null)
    {
        if (!vm.AidRequestId.HasValue)
        {
            ModelState.AddModelError(nameof(vm.AidRequestId), "لا يمكن صرف المساعدة بدون طلب مساعدة معتمد.");
            return;
        }

        var aidRequest = await _aidRequestRepository.GetByIdAsync(vm.AidRequestId.Value);
        if (aidRequest == null)
        {
            ModelState.AddModelError(nameof(vm.AidRequestId), "طلب المساعدة المرتبط غير موجود");
            return;
        }

        if (aidRequest.BeneficiaryId != vm.BeneficiaryId)
        {
            ModelState.AddModelError(nameof(vm.AidRequestId), "طلب المساعدة المختار لا يخص نفس المستفيد");
            return;
        }

        if (!string.Equals(aidRequest.Status, "Approved", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(aidRequest.Status, "Disbursed", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(vm.AidRequestId), "لا يمكن الصرف إلا لطلب مساعدة معتمد.");
            return;
        }

        vm.AidTypeId = aidRequest.AidTypeId;
        vm.ProjectId ??= aidRequest.ProjectId;

        await PopulateAidRequestFundingSummaryAsync(vm, currentDisbursementId);
        if ((!vm.Amount.HasValue || vm.Amount.Value <= 0m) && vm.RemainingToDisburseAmount > 0m)
            vm.Amount = vm.RemainingToDisburseAmount;
    }

    private async Task ValidateFundingLinksAsync(CreateBeneficiaryAidDisbursementVm vm, Guid? currentDisbursementId = null)
    {
        if (!vm.Amount.HasValue || vm.Amount.Value <= 0)
            ModelState.AddModelError(nameof(vm.Amount), "المبلغ المصروف يجب أن يكون أكبر من صفر.");

        if (!vm.AidRequestId.HasValue)
            return;

        var aidRequest = await _aidRequestRepository.GetByIdAsync(vm.AidRequestId.Value);
        if (aidRequest == null)
        {
            ModelState.AddModelError(nameof(vm.AidRequestId), "طلب المساعدة المرتبط غير موجود");
            return;
        }

        var committeeValidation = await BeneficiaryCommitteeDecisionGuard.ValidateAidDisbursementAmountAsync(
            _db,
            vm.BeneficiaryId,
            aidRequest.AidTypeId,
            vm.Amount,
            currentDisbursementId);

        if (!committeeValidation.IsValid)
            ModelState.AddModelError(nameof(vm.AidRequestId), committeeValidation.Message);

        await PopulateAidRequestFundingSummaryAsync(vm, currentDisbursementId);

        var requestedAmount = vm.Amount ?? 0m;
        if (vm.FundedAmount <= 0m)
            ModelState.AddModelError(nameof(vm.AidRequestId), "لا يمكن الصرف قبل وجود تخصيص تمويل للطلب.");
        else if (requestedAmount > vm.RemainingToDisburseAmount)
            ModelState.AddModelError(nameof(vm.Amount), $"المبلغ المطلوب يتجاوز الرصيد المتاح للصرف لهذا الطلب ({vm.RemainingToDisburseAmount:N2}).");

        if (vm.ProjectId.HasValue)
        {
            var project = await _projectRepository.GetByIdAsync(vm.ProjectId.Value);
            if (project == null)
                ModelState.AddModelError(nameof(vm.ProjectId), "المشروع المختار غير موجود");
        }

        if (vm.GrantAgreementId.HasValue)
        {
            var agreement = await _grantAgreementRepository.GetByIdAsync(vm.GrantAgreementId.Value);
            if (agreement == null)
                ModelState.AddModelError(nameof(vm.GrantAgreementId), "اتفاقية التمويل المختارة غير موجودة");
        }

        if (!vm.DonationId.HasValue)
            return;

        var donation = await _db.Set<Donation>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == vm.DonationId.Value);

        if (donation == null)
        {
            ModelState.AddModelError(nameof(vm.DonationId), "التبرع المختار غير موجود");
            return;
        }

        var allocationExists = await _db.Set<DonationAllocation>()
            .AsNoTracking()
            .AnyAsync(x => x.DonationId == vm.DonationId.Value && EF.Property<Guid?>(x, "AidRequestId") == vm.AidRequestId.Value);
        if (!allocationExists)
        {
            ModelState.AddModelError(nameof(vm.DonationId), "التبرع المختار غير مخصص لهذا الطلب.");
            return;
        }

        var aidTypeName = await _db.Set<AidTypeLookup>()
            .AsNoTracking()
            .Where(x => x.Id == aidRequest.AidTypeId)
            .Select(x => x.NameAr)
            .FirstOrDefaultAsync();

        if (!CharityFundingTypeHelper.DonationMatchesAidRequest(donation.DonationType, aidTypeName))
        {
            ModelState.AddModelError(nameof(vm.DonationId), CharityFundingTypeHelper.BuildMismatchMessage(aidTypeName, donation.DonationType));
            return;
        }

        if (string.Equals(donation.DonationType, "عيني", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(vm.DonationId), "التبرعات العينية لا تُصرف مباشرة للمستفيد. يجب أولًا تسجيلها كإضافة مخزنية ثم الصرف من المخزن أو من دورة صرف عينية.");
            return;
        }

        var donationRemainingById = await _aidDisbursementFundingLineService.GetRemainingAmountByDonationAsync(
            vm.AidRequestId.Value,
            currentDisbursementId);

        vm.SelectedDonationRemainingAmount = donationRemainingById.TryGetValue(vm.DonationId.Value, out var selectedDonationRemaining)
            ? selectedDonationRemaining
            : 0m;

        if (requestedAmount > vm.SelectedDonationRemainingAmount)
            ModelState.AddModelError(nameof(vm.Amount), $"المبلغ المطلوب يتجاوز الرصيد المتاح داخل سطور التخصيص للتبرع المحدد ({vm.SelectedDonationRemainingAmount:N2}).");
    }

    private async Task PopulateAidRequestFundingSummaryAsync(CreateBeneficiaryAidDisbursementVm vm, Guid? currentDisbursementId = null)
    {
        vm.RequestedAmount = 0m;
        vm.FundedAmount = 0m;
        vm.AlreadyDisbursedAmount = 0m;
        vm.RemainingToFundAmount = 0m;
        vm.RemainingToDisburseAmount = 0m;
        vm.RemainingOnRequestAmount = 0m;
        vm.SelectedDonationRemainingAmount = 0m;
        vm.FundingStatusCode = AidRequestFundingStatusCodes.NotFunded;
        vm.FundingStatusName = "غير ممول";
        vm.DisbursementStatusCode = AidRequestFundingStatusCodes.NotDisbursed;
        vm.DisbursementStatusName = "غير مصروف";

        if (!vm.AidRequestId.HasValue)
            return;

        var snapshot = await _aidRequestFundingService.GetSnapshotAsync(
            vm.AidRequestId.Value,
            excludeDisbursementId: currentDisbursementId);

        if (snapshot == null)
            return;

        vm.RequestedAmount = snapshot.RequestedAmount;
        vm.FundedAmount = snapshot.FundedAmount;
        vm.AlreadyDisbursedAmount = snapshot.DisbursedAmount;
        vm.RemainingToFundAmount = snapshot.RemainingToFundAmount;
        vm.RemainingToDisburseAmount = snapshot.RemainingToDisburseAmount;
        vm.RemainingOnRequestAmount = snapshot.RemainingOnRequestAmount;
        vm.FundingStatusCode = snapshot.FundingStatusCode;
        vm.FundingStatusName = snapshot.FundingStatusName;
        vm.DisbursementStatusCode = snapshot.DisbursementStatusCode;
        vm.DisbursementStatusName = snapshot.DisbursementStatusName;

        if (vm.DonationId.HasValue)
        {
            var donationRemainingById = await _aidDisbursementFundingLineService.GetRemainingAmountByDonationAsync(
                vm.AidRequestId.Value,
                currentDisbursementId);

            vm.SelectedDonationRemainingAmount = donationRemainingById.TryGetValue(vm.DonationId.Value, out var selectedDonationRemaining)
                ? selectedDonationRemaining
                : 0m;
        }
    }

    private async Task FillLookupsAsync(CreateBeneficiaryAidDisbursementVm vm, Guid? currentDisbursementId = null)
    {
        var aidTypes = await _lookupRepository.GetAidTypesAsync();
        var paymentMethods = await _lookupRepository.GetActivePaymentMethodsAsync();
        var accounts = await _lookupRepository.GetActivePostingFinancialAccountsAsync();
        var beneficiaries = await _beneficiaryRepository.SearchAsync(null, null, true);
        var projects = await _projectRepository.SearchAsync(null, null, true);
        var grantAgreements = await _grantAgreementRepository.GetAllAsync();
        var aidRequests = vm.BeneficiaryId == Guid.Empty
            ? new List<BeneficiaryAidRequest>()
            : (await _aidRequestRepository.GetByBeneficiaryIdAsync(vm.BeneficiaryId))
                .Where(x => string.Equals(x.Status, "Approved", StringComparison.OrdinalIgnoreCase)
                         || string.Equals(x.Status, "Disbursed", StringComparison.OrdinalIgnoreCase))
                .ToList();

        var requestIds = aidRequests.Select(x => x.Id).Distinct().ToList();
        var snapshots = await _aidRequestFundingService.GetSnapshotsAsync(
            requestIds,
            excludeDisbursementId: currentDisbursementId);

        var filteredAidRequests = aidRequests
            .Select(x =>
            {
                snapshots.TryGetValue(x.Id, out var snapshot);
                return new
                {
                    Request = x,
                    Requested = snapshot?.RequestedAmount ?? (x.RequestedAmount ?? 0m),
                    Funded = snapshot?.FundedAmount ?? 0m,
                    Disbursed = snapshot?.DisbursedAmount ?? 0m,
                    RemainingToFund = snapshot?.RemainingToFundAmount ?? 0m,
                    RemainingToDisburse = snapshot?.RemainingToDisburseAmount ?? 0m,
                    FundingStatusName = snapshot?.FundingStatusName ?? "غير ممول",
                    DisbursementStatusName = snapshot?.DisbursementStatusName ?? "غير مصروف"
                };
            })
            .Where(x => x.Request.Id == vm.AidRequestId || x.RemainingToDisburse > 0m || x.RemainingToFund > 0m)
            .OrderBy(x => x.Request.RequestDate)
            .ThenBy(x => x.Request.Beneficiary?.FullName)
            .ToList();

        await PopulateAidRequestFundingSummaryAsync(vm, currentDisbursementId);

        var selectedAidRequest = filteredAidRequests.FirstOrDefault(x => x.Request.Id == vm.AidRequestId);
        var selectedAidTypeName = selectedAidRequest?.Request.AidType?.NameAr
            ?? aidTypes.FirstOrDefault(x => x.Id == vm.AidTypeId)?.NameAr;

        var donationCandidates = new List<Donation>();
        var donationRemainingById = new Dictionary<Guid, decimal>();

        if (vm.AidRequestId.HasValue)
        {
            donationRemainingById = await _aidDisbursementFundingLineService.GetRemainingAmountByDonationAsync(
                vm.AidRequestId.Value,
                currentDisbursementId);

            var donationIds = donationRemainingById.Keys.ToList();
            if (donationIds.Count > 0)
            {
                donationCandidates = await _db.Set<Donation>()
                    .AsNoTracking()
                    .Where(x => donationIds.Contains(x.Id))
                    .OrderByDescending(x => x.DonationDate)
                    .ToListAsync();
            }

            if (!string.IsNullOrWhiteSpace(selectedAidTypeName))
            {
                donationCandidates = donationCandidates
                    .Where(x => CharityFundingTypeHelper.DonationMatchesAidRequest(x.DonationType, selectedAidTypeName))
                    .ToList();
            }

            donationCandidates = donationCandidates
                .Where(x => x.Id == vm.DonationId || (donationRemainingById.TryGetValue(x.Id, out var remaining) && remaining > 0m))
                .ToList();

            if (vm.DonationId.HasValue && donationRemainingById.TryGetValue(vm.DonationId.Value, out var selectedDonationRemaining))
                vm.SelectedDonationRemainingAmount = selectedDonationRemaining;
        }

        vm.AidTypes = aidTypes.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.NameAr
        }).ToList();

        vm.PaymentMethods = paymentMethods.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.MethodNameAr
        }).ToList();

        vm.FinancialAccounts = accounts.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.AccountCode} - {x.AccountNameAr}"
        }).ToList();

        vm.Beneficiaries = beneficiaries.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = string.IsNullOrWhiteSpace(x.Code) ? x.FullName : $"{x.Code} - {x.FullName}"
        }).ToList();

        vm.AidRequests = filteredAidRequests.Select(x => new SelectListItem
        {
            Value = x.Request.Id.ToString(),
            Text = $"{x.Request.RequestDate:yyyy-MM-dd} - {x.Request.AidType?.NameAr ?? "بدون نوع"} - المطلوب {x.Requested:N2} - الممول {x.Funded:N2} - المصروف {x.Disbursed:N2} - المتاح للصرف {x.RemainingToDisburse:N2} - {x.FundingStatusName} / {x.DisbursementStatusName}"
        }).ToList();

        vm.Projects = projects.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.Code} - {x.Name}"
        }).ToList();

        vm.Donations = donationCandidates.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.DonationNumber} - {x.DonationType} - {x.DonationDate:yyyy-MM-dd} - المتاح لهذا الطلب {(donationRemainingById.TryGetValue(x.Id, out var remaining) ? remaining : 0m):N2}"
        }).ToList();

        vm.GrantAgreements = grantAgreements.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.AgreementNumber} - {x.Title}"
        }).ToList();
    }

    private async Task RefreshAidRequestStatusAsync(Guid? aidRequestId)
    {
        if (!aidRequestId.HasValue)
            return;

        var aidRequest = await _aidRequestRepository.GetByIdAsync(aidRequestId.Value);
        if (aidRequest == null)
            return;

        if (string.Equals(aidRequest.Status, "Pending", StringComparison.OrdinalIgnoreCase)
            || string.Equals(aidRequest.Status, "Rejected", StringComparison.OrdinalIgnoreCase))
            return;

        var requestedAmount = aidRequest.RequestedAmount ?? 0m;
        var approvedDisbursedAmount = await _db.Set<BeneficiaryAidDisbursement>()
            .AsNoTracking()
            .Where(x => x.AidRequestId == aidRequestId.Value
                && x.ApprovalStatus == AidDisbursementApprovalStatusCodes.Approved)
            .SumAsync(x => x.Amount ?? 0m);

        var newStatus = requestedAmount > 0m && approvedDisbursedAmount >= requestedAmount
            ? "Disbursed"
            : "Approved";

        if (!string.Equals(aidRequest.Status, newStatus, StringComparison.OrdinalIgnoreCase))
        {
            aidRequest.Status = newStatus;
            await _aidRequestRepository.UpdateAsync(aidRequest);
        }
    }

    private static string? BuildFundingTraceSummary(InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries.BeneficiaryAidDisbursement disbursement)
    {
        if (disbursement.FundingLines == null || disbursement.FundingLines.Count == 0)
            return null;

        var parts = disbursement.FundingLines
            .Where(x => x.DonationAllocation?.Donation != null)
            .GroupBy(x => x.DonationAllocation!.Donation!.DonationNumber)
            .Select(g => new
            {
                DonationNumber = g.Key,
                Amount = g.Sum(x => x.AmountConsumed)
            })
            .OrderByDescending(x => x.Amount)
            .ToList();

        if (parts.Count == 0)
            return null;

        if (parts.Count == 1)
            return $"{parts[0].DonationNumber} ({parts[0].Amount:N2})";

        var preview = string.Join(" + ", parts.Take(2).Select(x => $"{x.DonationNumber} ({x.Amount:N2})"));
        return parts.Count > 2 ? $"{preview} + {parts.Count - 2} أخرى" : preview;
    }

    private async Task<bool> PopulateBeneficiaryAsync(Guid beneficiaryId)
    {
        var beneficiary = await _beneficiaryRepository.GetByIdAsync(beneficiaryId);
        if (beneficiary == null)
            return false;

        ViewBag.HasBeneficiaryContext = true;
        ViewBag.BeneficiaryId = beneficiary.Id;
        ViewBag.BeneficiaryCode = beneficiary.Code;
        ViewBag.BeneficiaryName = beneficiary.FullName;
        return true;
    }

    private void ClearBeneficiaryContext()
    {
        ViewBag.HasBeneficiaryContext = false;
        ViewBag.BeneficiaryId = Guid.Empty;
        ViewBag.BeneficiaryCode = string.Empty;
        ViewBag.BeneficiaryName = string.Empty;
    }
    private async Task<string> GenerateStoreIssueNumberAsync()
    {
        var prefix = $"ISS-{DateTime.Today:yyyyMMdd}";

        var existingNumbers = await _db.Set<CharityStoreIssue>()
            .AsNoTracking()
            .Where(x => x.IssueNumber.StartsWith(prefix))
            .Select(x => x.IssueNumber)
            .ToListAsync();

        var next = existingNumbers.Count + 1;
        string number;

        do
        {
            number = $"{prefix}-{next:000}";
            next++;
        }
        while (existingNumbers.Contains(number, StringComparer.OrdinalIgnoreCase));

        return number;
    }

    private static string BuildAutoStoreIssueNotes(BeneficiaryAidDisbursement disbursement, IReadOnlyCollection<DonationAllocation> allocations)
    {
        var allocationMarkers = string.Join(" ", allocations.Select(x => $"DonationAllocationId:{x.Id}"));
        var donationMarkers = string.Join(" ", allocations.Select(x => $"DonationId:{x.DonationId}").Distinct());
        var itemMarkers = string.Join(" ", allocations
            .Where(x => x.DonationInKindItemId.HasValue)
            .Select(x => $"DonationInKindItemId:{x.DonationInKindItemId!.Value}")
            .Distinct());

        var lineMarker = disbursement.AidRequestLineId.HasValue
            ? $" AidRequestLineId:{disbursement.AidRequestLineId.Value}"
            : string.Empty;

        return $"إذن صرف مخزني مولد من سجل صرف عيني. AidDisbursementId:{disbursement.Id} AidRequestId:{disbursement.AidRequestId}{lineMarker} {allocationMarkers} {donationMarkers} {itemMarkers}";
    }

    private static string BuildAutoStoreIssueLineNotes(DonationAllocation allocation, BeneficiaryAidDisbursement disbursement)
    {
        var itemName = allocation.DonationInKindItem?.Item?.ItemNameAr;
        var lineMarker = disbursement.AidRequestLineId.HasValue
            ? $" AidRequestLineId:{disbursement.AidRequestLineId.Value}"
            : string.Empty;

        return $"صرف عيني{(string.IsNullOrWhiteSpace(itemName) ? string.Empty : $" للصنف {itemName}")} من تخصيص تبرع. AidDisbursementId:{disbursement.Id} DonationAllocationId:{allocation.Id} DonationId:{allocation.DonationId} DonationInKindItemId:{allocation.DonationInKindItemId}{lineMarker}";
    }

    private static string AppendNote(string? oldNotes, string newNote)
    {
        if (string.IsNullOrWhiteSpace(oldNotes))
            return newNote;

        if (oldNotes.Contains(newNote, StringComparison.OrdinalIgnoreCase))
            return oldNotes;

        return oldNotes.TrimEnd() + Environment.NewLine + newNote;
    }
    private static List<BeneficiaryAidDisbursementInKindPrintItemVm> BuildInKindPrintItems(
    BeneficiaryAidDisbursement disbursement,
    CharityStoreIssue? storeIssue)
    {
        var result = new List<BeneficiaryAidDisbursementInKindPrintItemVm>();

        var allocations = disbursement.FundingLines
            .Where(x => x.DonationAllocation?.DonationInKindItem != null)
            .Select(x => x.DonationAllocation!)
            .GroupBy(x => x.Id)
            .Select(x => x.First())
            .ToList();

        foreach (var allocation in allocations)
        {
            var inKindItem = allocation.DonationInKindItem!;
            var quantity = allocation.AllocatedQuantity ?? inKindItem.Quantity;
            var unitValue = inKindItem.EstimatedUnitValue;

            result.Add(new BeneficiaryAidDisbursementInKindPrintItemVm
            {
                DonationAllocationId = allocation.Id,
                DonationInKindItemId = allocation.DonationInKindItemId,
                ItemCode = inKindItem.Item?.ItemCode,
                ItemName = inKindItem.Item?.ItemNameAr ?? "-",
                WarehouseName = inKindItem.Warehouse?.WarehouseNameAr,
                DonationNumber = allocation.Donation?.DonationNumber,
                BatchNo = inKindItem.BatchNo,
                ExpiryDate = inKindItem.ExpiryDate,
                Quantity = quantity,
                EstimatedUnitValue = unitValue,
                EstimatedTotalValue = quantity * (unitValue ?? 0m),
                Notes = allocation.Notes ?? inKindItem.Notes
            });
        }

        // Fallback: لو السجل قديم ومربوط بإذن صرف مخزني فقط بدون FundingLines
        if (result.Count == 0 && storeIssue != null)
        {
            foreach (var line in storeIssue.Lines)
            {
                result.Add(new BeneficiaryAidDisbursementInKindPrintItemVm
                {
                    StoreIssueLineId = line.Id,
                    ItemCode = line.Item?.ItemCode,
                    ItemName = line.Item?.ItemNameAr ?? "-",
                    WarehouseName = storeIssue.Warehouse?.WarehouseNameAr,
                    Quantity = line.Quantity,
                    EstimatedUnitValue = line.UnitCost,
                    EstimatedTotalValue = line.Quantity * line.UnitCost,
                    Notes = line.Notes
                });
            }
        }

        return result;
    }

 

}
