using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Donors;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Charity.DonorProfile;
using InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using System.Data;
using System.Security.Claims;
using InfrastructureManagmentCore.Domains.Identity;

public class DonationAllocationsController : Controller
{
    private readonly IDonationAllocationRepository _repository;
    private readonly IDonationRepository _donationRepository;
    private readonly IDonationInKindItemRepository _donationInKindItemRepository;
    private readonly IDonorRepository _donorRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _db;

    private const string AutoDisbursementSourceType = "DonationAllocationAutoDisbursement";
    private const string AutoInKindDisbursementSourceType = "DonationAllocationAutoInKindDisbursement";
    private const string AllocationApprovedStatus = "Approved";
    private const string DisbursementPendingStatus = "Pending";
    private const string DisbursementAvailableExecutionStatus = "Available";

    public DonationAllocationsController(
        IDonationAllocationRepository repository,
        IDonationRepository donationRepository,
        IDonationInKindItemRepository donationInKindItemRepository,
        IDonorRepository donorRepository,
        AppDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        _repository = repository;
        _donationRepository = donationRepository;
        _donationInKindItemRepository = donationInKindItemRepository;
        _donorRepository = donorRepository;
        _db = db;
        _userManager = userManager;
    }

    private static string ResolveTargetingScopeCode(Donation donation)
        => NormalizeText(donation.TargetingScopeCode);

    private static string? ResolvePurposeName(Donation donation)
    {
        var purposeName = NormalizeText(donation.GeneralPurposeName);
        return string.IsNullOrWhiteSpace(purposeName) ? null : purposeName;
    }

    private static string NormalizeSourceScopeCode(string? scopeCode)
    {
        var normalized = NormalizeText(scopeCode);
        return DonationTargetingScopeOption.Values.Any(x => string.Equals(x, normalized, StringComparison.OrdinalIgnoreCase))
            ? normalized
            : DonationTargetingScopeOption.GeneralFund;
    }

    private static bool SupportsDirectRequestAllocation(Donation donation)
    {
        if (string.Equals(donation.DonationType, "عيني", StringComparison.OrdinalIgnoreCase))
            return true;

        return string.Equals(ResolveTargetingScopeCode(donation), DonationTargetingScopeOption.SpecificRequests, StringComparison.OrdinalIgnoreCase);
    }
    private static bool IsGeneralPoolScope(string? scopeCode)
        => string.Equals(scopeCode, DonationTargetingScopeOption.GeneralFund, StringComparison.OrdinalIgnoreCase)
            || string.Equals(scopeCode, DonationTargetingScopeOption.GeneralPurpose, StringComparison.OrdinalIgnoreCase);

    private static bool MatchesGeneralPoolDonation(Donation donation, string scopeCode, string? purposeName)
    {
        var donationScope = ResolveTargetingScopeCode(donation);
        if (!string.Equals(donationScope, scopeCode, StringComparison.OrdinalIgnoreCase))
            return false;

        if (string.Equals(scopeCode, DonationTargetingScopeOption.GeneralPurpose, StringComparison.OrdinalIgnoreCase))
        {
            var requestedPurpose = NormalizeText(purposeName);
            if (string.IsNullOrWhiteSpace(requestedPurpose))
                return false;

            var donationPurpose = NormalizeText(ResolvePurposeName(donation));
            return !string.IsNullOrWhiteSpace(donationPurpose)
                && donationPurpose.Contains(requestedPurpose, StringComparison.OrdinalIgnoreCase);
        }


        return true;
    }

    private static string NormalizeText(string? value)
        => string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();

    private static bool AidRequestMatchesPurpose(BeneficiaryAidRequest request, string? purposeName)
    {
        var purpose = NormalizeText(purposeName);
        if (string.IsNullOrWhiteSpace(purpose))
            return true;

        var reasonValue = request.GetType().GetProperty("Reason")?.GetValue(request)?.ToString();
        if (!string.IsNullOrWhiteSpace(reasonValue) && reasonValue.Contains(purpose, StringComparison.OrdinalIgnoreCase))
            return true;

        var projectValue = request.GetType().GetProperty("Project")?.GetValue(request);
        if (projectValue != null)
        {
            var projectName = projectValue.GetType().GetProperty("Name")?.GetValue(projectValue)?.ToString();
            var projectCode = projectValue.GetType().GetProperty("Code")?.GetValue(projectValue)?.ToString();
            if ((!string.IsNullOrWhiteSpace(projectName) && projectName.Contains(purpose, StringComparison.OrdinalIgnoreCase))
                || (!string.IsNullOrWhiteSpace(projectCode) && projectCode.Contains(purpose, StringComparison.OrdinalIgnoreCase)))
                return true;
        }

        return false;
    }

    private IActionResult RedirectToGeneralPool(Donation donation, string warning)
    {
        TempData["Warning"] = warning;
        return RedirectToAction(nameof(GeneralCreate), new
        {
            financialAccountId = donation.FinancialAccountId,
            aidTypeId = donation.AidTypeId,
            sourceScopeCode = ResolveTargetingScopeCode(donation),
            purposeName = ResolvePurposeName(donation)
        });
    }

    public async Task<IActionResult> Index(Guid donationId)
    {
        var donation = await _donationRepository.GetByIdAsync(donationId);
        if (donation == null)
            return NotFound();

        if (!await PopulateDonorAsync(donation.DonorId))
            return NotFound();

        ViewBag.Donation = donation;
        ViewBag.AllocatedAmount = await _repository.GetAllocatedAmountAsync(donationId);
        ViewBag.SupportsDirectAllocation = SupportsDirectRequestAllocation(donation);
        ViewBag.TargetingScopeCode = ResolveTargetingScopeCode(donation);
        ViewBag.PurposeName = ResolvePurposeName(donation);

        var items = await _repository.GetByDonationIdAsync(donationId);
        var model = items.Select(x => new DonationAllocationListItemVm
        {
            Id = x.Id,
            AllocatedDate = x.AllocatedDate,
            BeneficiaryName = x.Beneficiary?.FullName,
            AidRequestSummary = x.AidRequest == null
                ? null
                : $"{x.AidRequest.RequestDate:yyyy-MM-dd} - {x.AidRequest.AidType?.NameAr ?? "بدون نوع"} - {x.AidRequest.Status}",
            DonationItemName = x.DonationInKindItem?.Item?.ItemNameAr,
            AllocatedQuantity = x.AllocatedQuantity,
            Amount = x.Amount,
            Notes = x.Notes
        }).ToList();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid donationId)
    {
        var donation = await _donationRepository.GetByIdAsync(donationId);
        if (donation == null)
            return NotFound();

        if (!await PopulateDonorAsync(donation.DonorId))
            return NotFound();

        if (!SupportsDirectRequestAllocation(donation))
            return RedirectToGeneralPool(donation, "هذا التبرع لا يدعم التخصيص المباشر. استخدم شاشة التخصيص العام من الرصيد.");

        ViewBag.Donation = donation;

        var vm = new CreateDonationAllocationVm
        {
            DonationId = donationId,
            AllocatedDate = DateTime.Today
        };

        await FillLookupsAsync(vm, donation);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDonationAllocationVm vm)
    {
        var donation = await _donationRepository.GetByIdAsync(vm.DonationId);
        if (donation == null)
            return NotFound();

        if (!await PopulateDonorAsync(donation.DonorId))
            return NotFound();

        if (!SupportsDirectRequestAllocation(donation))
            return RedirectToGeneralPool(donation, "هذا التبرع لا يدعم التوزيع الجماعي المباشر. استخدم شاشة التخصيص العام من الرصيد.");

        ViewBag.Donation = donation;
        await ValidateAllocationRequestAsync(vm, donation, null);
        await FillLookupsAsync(vm, donation, vm.AidRequestId);

        if (!ModelState.IsValid)
            return View(vm);

        var aidRequest = await _db.Set<BeneficiaryAidRequest>()
            .AsNoTracking()
            .FirstAsync(x => x.Id == vm.AidRequestId!.Value);

        var entity = new DonationAllocation
        {
            Id = Guid.NewGuid(),
            DonationId = vm.DonationId,
            AidRequestLineId = vm.AidRequestLineId,
            AllocatedDate = vm.AllocatedDate,
            AidRequestId = aidRequest.Id,
            BeneficiaryId = aidRequest.BeneficiaryId,
            DonationInKindItemId = vm.DonationInKindItemId,
            AllocatedQuantity = vm.AllocatedQuantity,
            Amount = vm.Amount,
            ApprovalStatus = AllocationApprovedStatus,
            ApprovedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            ApprovedAtUtc = DateTime.UtcNow,
            Notes = vm.Notes?.Trim(),
            CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            CreatedAtUtc = DateTime.UtcNow
        };

        await _repository.AddAsync(entity);
        var pendingDisbursementCount = await EnsurePendingAidDisbursementsForAllocationsAsync(new[] { entity });
        
        TempData["Success"] = BuildAllocationWorkflowSuccessMessage(
            "تم حفظ تخصيص التبرع بنجاح",
            pendingDisbursementCount);
        return RedirectToAction(nameof(Index), new { donationId = vm.DonationId });
    }

    [HttpGet]
    public async Task<IActionResult> BulkCreate(Guid donationId)
    {
        var donation = await _donationRepository.GetByIdAsync(donationId);
        if (donation == null)
            return NotFound();

        if (!await PopulateDonorAsync(donation.DonorId))
            return NotFound();

        if (!SupportsDirectRequestAllocation(donation))
            return RedirectToGeneralPool(donation, "هذا التبرع لا يدعم التوزيع الجماعي المباشر. استخدم شاشة التخصيص العام من الرصيد.");

        ViewBag.Donation = donation;
        var vm = await BuildBulkAllocationVmAsync(donation, DateTime.Today, null, null);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BulkCreate(BulkDonationAllocationVm vm)
    {
        var donation = await _donationRepository.GetByIdAsync(vm.DonationId);
        if (donation == null)
            return NotFound();

        if (!await PopulateDonorAsync(donation.DonorId))
            return NotFound();

        if (!SupportsDirectRequestAllocation(donation))
            return RedirectToGeneralPool(donation, "هذا التبرع لا يدعم التوزيع الجماعي المباشر. استخدم شاشة التخصيص العام من الرصيد.");

        ViewBag.Donation = donation;

        var entered = vm.Rows?.ToDictionary(x => x.AidRequestId, x => x.AllocateAmount) ?? new Dictionary<Guid, decimal?>();
        var hydratedVm = await BuildBulkAllocationVmAsync(donation, vm.AllocatedDate, vm.Notes, entered);

        if (!string.Equals(donation.DonationType, "نقدي", StringComparison.OrdinalIgnoreCase))
            ModelState.AddModelError(string.Empty, "التوزيع الجماعي متاح حاليًا للتبرعات النقدية فقط.");

        if (!donation.AidTypeId.HasValue || donation.AidTypeId == Guid.Empty)
            ModelState.AddModelError(string.Empty, "يجب تحديد نوع المساعدة المستهدف داخل التبرع أولًا.");

        if (hydratedVm.Rows.Count == 0)
            ModelState.AddModelError(string.Empty, "لا توجد طلبات معتمدة ومفتوحة مؤهلة للتوزيع لهذا التبرع.");

        var selectedRows = hydratedVm.Rows
            .Select((row, index) => new { Row = row, Index = index })
            .Where(x => x.Row.AllocateAmount.HasValue && x.Row.AllocateAmount.Value > 0)
            .ToList();

        if (selectedRows.Count == 0)
            ModelState.AddModelError(string.Empty, "أدخل مبلغًا واحدًا على الأقل داخل سطور التوزيع قبل الحفظ.");

        foreach (var item in hydratedVm.Rows.Select((row, index) => new { Row = row, Index = index }))
        {
            if (item.Row.AllocateAmount.HasValue && item.Row.AllocateAmount.Value < 0)
                ModelState.AddModelError($"Rows[{item.Index}].AllocateAmount", "لا يمكن إدخال مبلغ سالب.");

            if (item.Row.AllocateAmount.HasValue && item.Row.AllocateAmount.Value > item.Row.RemainingNeedAmount)
                ModelState.AddModelError($"Rows[{item.Index}].AllocateAmount", $"المبلغ المدخل أكبر من الاحتياج المتبقي للطلب ({item.Row.RemainingNeedAmount:N2}).");
        }

        var totalSelected = selectedRows.Sum(x => x.Row.AllocateAmount ?? 0m);
        if (totalSelected > hydratedVm.DonationRemainingAmount)
            ModelState.AddModelError(string.Empty, $"إجمالي التوزيع المطلوب ({totalSelected:N2}) أكبر من المتبقي من التبرع ({hydratedVm.DonationRemainingAmount:N2}).");

        if (!ModelState.IsValid)
            return View(hydratedVm);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var now = DateTime.UtcNow;

        var allocations = selectedRows.Select(x => new DonationAllocation
        {
            Id = Guid.NewGuid(),
            DonationId = hydratedVm.DonationId,
            AllocatedDate = hydratedVm.AllocatedDate,
            AidRequestId = x.Row.AidRequestId,
            BeneficiaryId = x.Row.BeneficiaryId,
            Amount = x.Row.AllocateAmount!.Value,
            ApprovalStatus = AllocationApprovedStatus,
            ApprovedByUserId = userId,
            ApprovedAtUtc = now,
            Notes = string.IsNullOrWhiteSpace(hydratedVm.Notes)
                ? $"توزيع جماعي على الطلب {x.Row.RequestNumberOrDate}"
                : hydratedVm.Notes!.Trim(),
            CreatedByUserId = userId,
            CreatedAtUtc = now
        }).ToList();

        await _db.Set<DonationAllocation>().AddRangeAsync(allocations);
        await _db.SaveChangesAsync();

        var pendingDisbursementCount = await EnsurePendingAidDisbursementsForAllocationsAsync(allocations);

        TempData["Success"] = BuildAllocationWorkflowSuccessMessage(
            $"تم حفظ {allocations.Count} تخصيص/توزيع بإجمالي {totalSelected:N2}.",
            pendingDisbursementCount);
        return RedirectToAction(nameof(Index), new { donationId = hydratedVm.DonationId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null || entity.Donation == null)
            return NotFound();

        if (!await PopulateDonorAsync(entity.Donation.DonorId))
            return NotFound();

        ViewBag.Donation = entity.Donation;

        var vm = new EditDonationAllocationVm
        {
            Id = entity.Id,
            DonationId = entity.DonationId,
            AllocatedDate = entity.AllocatedDate,
            AidRequestId = entity.AidRequestId,
            BeneficiaryId = entity.BeneficiaryId,
            DonationInKindItemId = entity.DonationInKindItemId,
            AllocatedQuantity = entity.AllocatedQuantity,
            Amount = entity.Amount,
            Notes = entity.Notes
        };

        await FillLookupsAsync(vm, entity.Donation, entity.AidRequestId, entity.Id);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditDonationAllocationVm vm)
    {
        var entity = await _repository.GetByIdAsync(vm.Id);
        if (entity == null || entity.Donation == null)
            return NotFound();

        if (!await PopulateDonorAsync(entity.Donation.DonorId))
            return NotFound();

        ViewBag.Donation = entity.Donation;
        await ValidateAllocationRequestAsync(vm, entity.Donation, vm.Id);
        await FillLookupsAsync(vm, entity.Donation, vm.AidRequestId, vm.Id);

        if (!ModelState.IsValid)
            return View(vm);

        var aidRequest = await _db.Set<BeneficiaryAidRequest>()
            .AsNoTracking()
            .FirstAsync(x => x.Id == vm.AidRequestId!.Value);

        entity.AllocatedDate = vm.AllocatedDate;
        entity.AidRequestLineId = vm.AidRequestLineId;
        entity.AidRequestId = aidRequest.Id;
        entity.BeneficiaryId = aidRequest.BeneficiaryId;
        entity.DonationInKindItemId = vm.DonationInKindItemId;
        entity.AllocatedQuantity = vm.AllocatedQuantity;
        entity.Amount = vm.Amount;
        entity.Notes = vm.Notes?.Trim();
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.UpdateAsync(entity);
        TempData["Success"] = "تم تعديل تخصيص التبرع بنجاح";
        return RedirectToAction(nameof(Index), new { donationId = entity.DonationId });
    }

    private async Task ValidateAllocationRequestAsync(CreateDonationAllocationVm vm, Donation donation, Guid? currentAllocationId)
    {
        if (!donation.AidTypeId.HasValue || donation.AidTypeId == Guid.Empty)
            ModelState.AddModelError(nameof(vm.AidRequestId), "يجب تحديد نوع المساعدة داخل التبرع أولًا قبل التخصيص.");

        if (!vm.AidRequestId.HasValue || vm.AidRequestId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(vm.AidRequestId), "طلب المساعدة مطلوب.");
            return;
        }

        var aidRequest = await _db.Set<BeneficiaryAidRequest>()
            .AsNoTracking()
            .Include(x => x.Beneficiary)
            .Include(x => x.AidType)
            .FirstOrDefaultAsync(x => x.Id == vm.AidRequestId.Value);

        if (aidRequest == null)
        {
            ModelState.AddModelError(nameof(vm.AidRequestId), "طلب المساعدة المختار غير موجود.");
            return;
        }

        vm.BeneficiaryId = aidRequest.BeneficiaryId;
        var requestLines = await _db.Set<BeneficiaryAidRequestLine>()
    .AsNoTracking()
    .Where(x => x.AidRequestId == aidRequest.Id)
    .ToListAsync();

        var requestHasLines = requestLines.Count > 0;
        vm.SelectedRequestHasLines = requestHasLines;

        BeneficiaryAidRequestLine? selectedLine = null;
        if (requestHasLines)
        {
            if (!vm.AidRequestLineId.HasValue || vm.AidRequestLineId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(vm.AidRequestLineId), "يجب اختيار بند طلب المساعدة قبل التخصيص لأن هذا الطلب يحتوي على بنود.");
            }
            else
            {
                selectedLine = requestLines.FirstOrDefault(x => x.Id == vm.AidRequestLineId.Value);
                if (selectedLine == null)
                    ModelState.AddModelError(nameof(vm.AidRequestLineId), "بند طلب المساعدة المختار لا يخص هذا الطلب.");
            }
        }
        else
        {
            vm.AidRequestLineId = null;
        }
        var isExistingSelectedRequest = currentAllocationId.HasValue
            && await _db.Set<DonationAllocation>().AsNoTracking().AnyAsync(x => x.Id == currentAllocationId.Value && x.AidRequestId == aidRequest.Id);

        if (!string.Equals(aidRequest.Status, "Approved", StringComparison.OrdinalIgnoreCase) && !isExistingSelectedRequest)
            ModelState.AddModelError(nameof(vm.AidRequestId), "لا يمكن التخصيص إلا لطلب مساعدة معتمد.");

        if (donation.AidTypeId.HasValue && donation.AidTypeId.Value != aidRequest.AidTypeId)
            ModelState.AddModelError(nameof(vm.AidRequestId), "لا يمكن تخصيص التبرع إلا لنفس نوع طلب المساعدة تمامًا.");

        if (string.Equals(donation.DonationType, "نقدي", StringComparison.OrdinalIgnoreCase))
        {
            if (selectedLine != null)
            {
                var allocatedForLine = await _db.Set<DonationAllocation>()
                    .AsNoTracking()
                    .Where(x => x.AidRequestLineId == selectedLine.Id
                        && (!currentAllocationId.HasValue || x.Id != currentAllocationId.Value))
                    .SumAsync(x => x.Amount ?? 0m);

                var lineTotal = selectedLine.EstimatedTotalValue
                    ?? ((selectedLine.ApprovedQuantity ?? selectedLine.RequestedQuantity ?? 0m) * (selectedLine.EstimatedUnitValue ?? 0m));

                var lineRemaining = Math.Max(0m, lineTotal - allocatedForLine);
                vm.SelectedLineEstimatedTotal = lineTotal;
                vm.SelectedLineAllocatedAmount = allocatedForLine;
                vm.SelectedLineRemainingAmount = lineRemaining;

                if (lineTotal > 0m && vm.Amount.HasValue && vm.Amount.Value > lineRemaining)
                    ModelState.AddModelError(nameof(vm.Amount), $"المبلغ المخصص أكبر من المتبقي على بند الطلب ({lineRemaining:N2}).");
            }
            if (!vm.Amount.HasValue || vm.Amount.Value <= 0)
                ModelState.AddModelError(nameof(vm.Amount), "التبرع النقدي يجب تخصيصه بمبلغ.");

            if (vm.DonationInKindItemId.HasValue)
                ModelState.AddModelError(nameof(vm.DonationInKindItemId), "لا يمكن اختيار صنف عيني مع تبرع نقدي.");

            var allocatedAmount = await _repository.GetAllocatedAmountAsync(vm.DonationId, currentAllocationId);
            var donationAmount = donation.Amount ?? 0m;
            var donationRemaining = Math.Max(0m, donationAmount - allocatedAmount);
            if (vm.Amount.HasValue && vm.Amount.Value > donationRemaining)
                ModelState.AddModelError(nameof(vm.Amount), $"المبلغ المخصص أكبر من المتبقي من التبرع ({donationRemaining:N2}).");

            var allocatedForRequest = await _db.Set<DonationAllocation>()
                .AsNoTracking()
                .Where(x => x.AidRequestId == aidRequest.Id && (!currentAllocationId.HasValue || x.Id != currentAllocationId.Value))
                .SumAsync(x => x.Amount ?? 0m);

            var requestedAmount = aidRequest.RequestedAmount ?? 0m;
            if (requestedAmount > 0m)
            {
                var requestRemaining = Math.Max(0m, requestedAmount - allocatedForRequest);
                if (vm.Amount.HasValue && vm.Amount.Value > requestRemaining)
                    ModelState.AddModelError(nameof(vm.Amount), $"المبلغ المطلوب يتجاوز الاحتياج المتبقي للطلب ({requestRemaining:N2}).");
            }
        }
        else if (string.Equals(donation.DonationType, "عيني", StringComparison.OrdinalIgnoreCase))
        {
            if (selectedLine != null)
            {
                var allocatedQtyForLine = await _db.Set<DonationAllocation>()
                    .AsNoTracking()
                    .Where(x => x.AidRequestLineId == selectedLine.Id
                        && (!currentAllocationId.HasValue || x.Id != currentAllocationId.Value))
                    .SumAsync(x => x.AllocatedQuantity ?? 0m);

                var approvedQty = selectedLine.ApprovedQuantity ?? selectedLine.RequestedQuantity ?? 0m;
                var lineRemainingQty = Math.Max(0m, approvedQty - allocatedQtyForLine);

                vm.SelectedLineRequestedQuantity = selectedLine.RequestedQuantity ?? 0m;
                vm.SelectedLineApprovedQuantity = approvedQty;
                vm.SelectedLineAllocatedQuantity = allocatedQtyForLine;
                vm.SelectedLineRemainingQuantity = lineRemainingQty;

                if (approvedQty > 0m && vm.AllocatedQuantity.HasValue && vm.AllocatedQuantity.Value > lineRemainingQty)
                    ModelState.AddModelError(nameof(vm.AllocatedQuantity), $"الكمية المخصصة أكبر من الكمية المتبقية على بند الطلب ({lineRemainingQty:N2}).");
            }
            if (!vm.DonationInKindItemId.HasValue)
                ModelState.AddModelError(nameof(vm.DonationInKindItemId), "اختر الصنف العيني المراد تخصيصه.");

            if (!vm.AllocatedQuantity.HasValue || vm.AllocatedQuantity.Value <= 0)
                ModelState.AddModelError(nameof(vm.AllocatedQuantity), "الكمية المخصصة مطلوبة للتبرع العيني.");

            if (vm.Amount.HasValue && vm.Amount.Value > 0)
                ModelState.AddModelError(nameof(vm.Amount), "لا يتم تخصيص التبرع العيني بمبلغ نقدي.");

            if (vm.DonationInKindItemId.HasValue && vm.AllocatedQuantity.HasValue && vm.AllocatedQuantity.Value > 0)
            {
                var inKindItem = await _donationInKindItemRepository.GetByIdAsync(vm.DonationInKindItemId.Value);
                if (inKindItem == null)
                {
                    ModelState.AddModelError(nameof(vm.DonationInKindItemId), "الصنف العيني غير موجود.");
                }
                else if (inKindItem.DonationId != vm.DonationId)
                {
                    ModelState.AddModelError(nameof(vm.DonationInKindItemId), "الصنف العيني المختار لا يخص نفس التبرع.");
                }
                else
                {
                    var allocatedQty = await _donationInKindItemRepository.GetAllocatedQuantityAsync(vm.DonationInKindItemId.Value, currentAllocationId);
                    var remainingQty = inKindItem.Quantity - allocatedQty;
                    if (vm.AllocatedQuantity.Value > remainingQty)
                        ModelState.AddModelError(nameof(vm.AllocatedQuantity), $"الكمية المخصصة أكبر من المتبقي ({remainingQty:N2}).");
                }
            }
        }
    }

    private async Task<bool> PopulateDonorAsync(Guid donorId)
    {
        var donor = await _donorRepository.GetByIdAsync(donorId);
        if (donor == null)
            return false;

        ViewBag.DonorHeader = new DonorHeaderVm
        {
            Id = donor.Id,
            Code = donor.Code,
            DonorType = donor.DonorType,
            FullName = donor.FullName,
            PhoneNumber = donor.PhoneNumber,
            ContactPerson = donor.ContactPerson,
            IsActive = donor.IsActive
        };

        return true;
    }

    private async Task FillLookupsAsync(CreateDonationAllocationVm vm, Donation donation, Guid? selectedAidRequestId = null, Guid? currentAllocationId = null)
    {
        var donationItems = await _db.Set<DonationInKindItem>()
            .AsNoTracking()
            .Include(x => x.Item)
            .Where(x => x.DonationId == vm.DonationId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync();

        var donationRemaining = Math.Max(0m, (donation.Amount ?? 0m) - await _repository.GetAllocatedAmountAsync(vm.DonationId, currentAllocationId));
        vm.DonationRemainingAmount = donationRemaining;
        vm.DonationInKindItems = donationItems
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = $"{x.Item?.ItemNameAr ?? "صنف"} - كمية {x.Quantity:N2}" })
            .ToList();

        if (!donation.AidTypeId.HasValue || donation.AidTypeId == Guid.Empty)
        {
            vm.AidRequests = new List<SelectListItem>();
            vm.RequestedAmount = 0m;
            vm.AlreadyAllocatedAmount = 0m;
            vm.AlreadyDisbursedAmount = 0m;
            vm.RemainingNeedAmount = 0m;
            return;
        }

        var approvedRequests = await _db.Set<BeneficiaryAidRequest>()
            .AsNoTracking()
            .Include(x => x.Beneficiary)
            .Include(x => x.AidType)
            .Where(x => x.Status == "Approved" && x.AidTypeId == donation.AidTypeId.Value)
            .ToListAsync();

        if (selectedAidRequestId.HasValue && !approvedRequests.Any(x => x.Id == selectedAidRequestId.Value))
        {
            var selected = await _db.Set<BeneficiaryAidRequest>()
                .AsNoTracking()
                .Include(x => x.Beneficiary)
                .Include(x => x.AidType)
                .FirstOrDefaultAsync(x => x.Id == selectedAidRequestId.Value);
            if (selected != null)
                approvedRequests.Add(selected);
        }

        var requestIds = approvedRequests.Select(x => x.Id).Distinct().ToList();
        var allocatedByRequest = requestIds.Count == 0
            ? new Dictionary<Guid, decimal>()
            : await _db.Set<DonationAllocation>()
                .AsNoTracking()
                .Where(x => x.AidRequestId.HasValue && requestIds.Contains(x.AidRequestId.Value) && (!currentAllocationId.HasValue || x.Id != currentAllocationId.Value))
                .GroupBy(x => x.AidRequestId!.Value)
                .Select(g => new { AidRequestId = g.Key, Amount = g.Sum(x => x.Amount ?? 0m) })
                .ToDictionaryAsync(x => x.AidRequestId, x => x.Amount);

        var disbursedByRequest = requestIds.Count == 0
            ? new Dictionary<Guid, decimal>()
            : await _db.Set<BeneficiaryAidDisbursement>()
                .AsNoTracking()
                .Where(x => x.AidRequestId.HasValue && requestIds.Contains(x.AidRequestId.Value))
                .GroupBy(x => x.AidRequestId!.Value)
                .Select(g => new { AidRequestId = g.Key, Amount = g.Sum(x => x.Amount ?? 0m) })
                .ToDictionaryAsync(x => x.AidRequestId, x => x.Amount);

        var beneficiaryIds = approvedRequests.Select(x => x.BeneficiaryId).Distinct().ToList();
        var disbursedBeneficiaryIds = beneficiaryIds.Count == 0
            ? new HashSet<Guid>()
            : (await _db.Set<BeneficiaryAidDisbursement>()
                .AsNoTracking()
                .Where(x => beneficiaryIds.Contains(x.BeneficiaryId))
                .Select(x => x.BeneficiaryId)
                .Distinct()
                .ToListAsync()).ToHashSet();

        var orderedRequests = approvedRequests
            .Select(x => new
            {
                Request = x,
                Allocated = allocatedByRequest.TryGetValue(x.Id, out var allocated) ? allocated : 0m,
                Disbursed = disbursedByRequest.TryGetValue(x.Id, out var disbursed) ? disbursed : 0m,
                Requested = x.RequestedAmount ?? 0m
            })
            .Where(x => x.Request.Id == selectedAidRequestId || x.Requested <= 0m || x.Allocated < x.Requested)
            .OrderBy(x => disbursedBeneficiaryIds.Contains(x.Request.BeneficiaryId) ? 1 : 0)
            .ThenBy(x => GetUrgencyRank(x.Request.UrgencyLevel))
            .ThenBy(x => x.Request.RequestDate)
            .ThenBy(x => x.Request.Beneficiary?.FullName)
            .ToList();

        if (selectedAidRequestId.HasValue)
        {
            var selectedRequest = orderedRequests.FirstOrDefault(x => x.Request.Id == selectedAidRequestId.Value);
            if (selectedRequest != null)
            {
                vm.BeneficiaryId = selectedRequest.Request.BeneficiaryId;
                vm.RequestedAmount = selectedRequest.Requested;
                vm.AlreadyAllocatedAmount = selectedRequest.Allocated;
                vm.AlreadyDisbursedAmount = selectedRequest.Disbursed;
                vm.RemainingNeedAmount = selectedRequest.Requested > 0m
                    ? Math.Max(0m, selectedRequest.Requested - selectedRequest.Allocated)
                    : 0m;
            }
        }

        vm.AidRequests = orderedRequests
            .Select(x => new SelectListItem
            {
                Value = x.Request.Id.ToString(),
                Text = $"{(disbursedBeneficiaryIds.Contains(x.Request.BeneficiaryId) ? "[له صرف سابق]" : "[أولوية أولى]")} {x.Request.Beneficiary?.FullName} - {x.Request.RequestDate:yyyy-MM-dd} - {x.Request.AidType?.NameAr ?? "بدون نوع"} - المطلوب {x.Requested:N2} - الممول {x.Allocated:N2} - المتبقي {Math.Max(0m, x.Requested - x.Allocated):N2}"
            })
            .ToList();
        vm.AidRequestLines = new List<SelectListItem>();

        var requestIdForLines = selectedAidRequestId ?? vm.AidRequestId;
        if (requestIdForLines.HasValue && requestIdForLines.Value != Guid.Empty)
        {
            var lines = await _db.Set<BeneficiaryAidRequestLine>()
                .AsNoTracking()
                .Include(x => x.Item)
                .Where(x => x.AidRequestId == requestIdForLines.Value)
                .OrderBy(x => x.CreatedAtUtc)
                .ToListAsync();

            if (lines.Count > 0)
            {
                var lineIds = lines.Select(x => x.Id).ToList();

                var allocatedAmountByLine = await _db.Set<DonationAllocation>()
                    .AsNoTracking()
                    .Where(x => x.AidRequestLineId.HasValue
                        && lineIds.Contains(x.AidRequestLineId.Value)
                        && (!currentAllocationId.HasValue || x.Id != currentAllocationId.Value))
                    .GroupBy(x => x.AidRequestLineId!.Value)
                    .Select(g => new
                    {
                        LineId = g.Key,
                        Amount = g.Sum(x => x.Amount ?? 0m),
                        Quantity = g.Sum(x => x.AllocatedQuantity ?? 0m)
                    })
                    .ToDictionaryAsync(x => x.LineId);

                vm.SelectedRequestHasLines = true;
                vm.AidRequestLines = lines.Select(line =>
                {
                    allocatedAmountByLine.TryGetValue(line.Id, out var used);
                    var itemName = !string.IsNullOrWhiteSpace(line.ItemNameSnapshot)
                        ? line.ItemNameSnapshot
                        : line.Item?.ItemNameAr;

                    itemName = string.IsNullOrWhiteSpace(itemName) ? "بند طلب" : itemName;

                    var approvedQty = line.ApprovedQuantity ?? line.RequestedQuantity ?? 0m;
                    var remainingQty = Math.Max(0m, approvedQty - (used?.Quantity ?? 0m));

                    var lineTotal = line.EstimatedTotalValue
                        ?? (approvedQty * (line.EstimatedUnitValue ?? 0m));
                    var remainingAmount = Math.Max(0m, lineTotal - (used?.Amount ?? 0m));

                    return new SelectListItem
                    {
                        Value = line.Id.ToString(),
                        Text = $"{itemName} - كمية {approvedQty:N2} - متبقي كمية {remainingQty:N2} - قيمة {lineTotal:N2} - متبقي قيمة {remainingAmount:N2}",
                        Selected = vm.AidRequestLineId.HasValue && vm.AidRequestLineId.Value == line.Id
                    };
                }).ToList();
            }
        }

    }


    private async Task<List<SelectListItem>> GetAidRequestLineOptionsAsync(Guid aidRequestId)
    {
        var lines = await GetAidRequestLineInfosAsync(aidRequestId);
        return lines.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = BuildAidRequestLineDisplay(x)
        }).ToList();
    }

    private async Task<Dictionary<Guid, string>> GetAidRequestLineSummariesAsync(IEnumerable<Guid?> ids)
    {
        var lineIds = ids.Where(x => x.HasValue && x.Value != Guid.Empty)
            .Select(x => x!.Value)
            .Distinct()
            .ToList();

        if (lineIds.Count == 0)
            return new Dictionary<Guid, string>();

        var connection = _db.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;
        if (shouldClose)
            await connection.OpenAsync();

        try
        {
            var result = new Dictionary<Guid, string>();
            foreach (var id in lineIds)
            {
                var line = await GetAidRequestLineInfoAsync(id);
                if (line != null)
                    result[line.Id] = BuildAidRequestLineDisplay(line);
            }

            return result;
        }
        finally
        {
            if (shouldClose)
                await connection.CloseAsync();
        }
    }

    private async Task<bool> AidRequestHasLinesAsync(Guid aidRequestId)
    {
        var connection = _db.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;
        if (shouldClose)
            await connection.OpenAsync();

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT TOP (1) 1 FROM dbo.BeneficiaryAidRequestLines WHERE AidRequestId = @AidRequestId";
            AddParameter(command, "@AidRequestId", aidRequestId);
            var value = await command.ExecuteScalarAsync();
            return value != null && value != DBNull.Value;
        }
        finally
        {
            if (shouldClose)
                await connection.CloseAsync();
        }
    }

    private async Task<AidRequestLineInfo?> GetAidRequestLineInfoAsync(Guid? aidRequestLineId)
    {
        if (!aidRequestLineId.HasValue || aidRequestLineId.Value == Guid.Empty)
            return null;

        var connection = _db.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;
        if (shouldClose)
            await connection.OpenAsync();

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = @"
SELECT TOP (1)
       Id,
       AidRequestId,
       ItemNameSnapshot,
       Description,
       RequestedQuantity,
       ApprovedQuantity,
       EstimatedUnitValue,
       EstimatedTotalValue,
       FulfillmentMethod,
       WarehouseId
FROM dbo.BeneficiaryAidRequestLines
WHERE Id = @Id";
            AddParameter(command, "@Id", aidRequestLineId.Value);

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null;

            return new AidRequestLineInfo
            {
                Id = reader.GetGuid(0),
                AidRequestId = reader.GetGuid(1),
                ItemNameSnapshot = reader.IsDBNull(2) ? null : reader.GetString(2),
                Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                RequestedQuantity = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                ApprovedQuantity = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                EstimatedUnitValue = reader.IsDBNull(6) ? null : reader.GetDecimal(6),
                EstimatedTotalValue = reader.IsDBNull(7) ? null : reader.GetDecimal(7),
                FulfillmentMethod = reader.IsDBNull(8) ? null : reader.GetString(8),
                WarehouseId = reader.IsDBNull(9) ? null : reader.GetGuid(9)
            };
        }
        finally
        {
            if (shouldClose)
                await connection.CloseAsync();
        }
    }

    private async Task<List<AidRequestLineInfo>> GetAidRequestLineInfosAsync(Guid aidRequestId)
    {
        var result = new List<AidRequestLineInfo>();
        if (aidRequestId == Guid.Empty)
            return result;

        var connection = _db.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;
        if (shouldClose)
            await connection.OpenAsync();

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = @"
SELECT Id,
       AidRequestId,
       ItemNameSnapshot,
       Description,
       RequestedQuantity,
       ApprovedQuantity,
       EstimatedUnitValue,
       EstimatedTotalValue,
       FulfillmentMethod,
       WarehouseId
FROM dbo.BeneficiaryAidRequestLines
WHERE AidRequestId = @AidRequestId
ORDER BY CreatedAtUtc, Id";
            AddParameter(command, "@AidRequestId", aidRequestId);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new AidRequestLineInfo
                {
                    Id = reader.GetGuid(0),
                    AidRequestId = reader.GetGuid(1),
                    ItemNameSnapshot = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                    RequestedQuantity = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                    ApprovedQuantity = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                    EstimatedUnitValue = reader.IsDBNull(6) ? null : reader.GetDecimal(6),
                    EstimatedTotalValue = reader.IsDBNull(7) ? null : reader.GetDecimal(7),
                    FulfillmentMethod = reader.IsDBNull(8) ? null : reader.GetString(8),
                    WarehouseId = reader.IsDBNull(9) ? null : reader.GetGuid(9)
                });
            }
        }
        finally
        {
            if (shouldClose)
                await connection.CloseAsync();
        }

        return result;
    }

    private async Task<decimal> GetAllocatedAmountForAidRequestLineAsync(Guid aidRequestLineId, Guid? currentAllocationId)
    {
        return await _db.Set<DonationAllocation>()
            .AsNoTracking()
            .Where(x => x.AidRequestLineId == aidRequestLineId && (!currentAllocationId.HasValue || x.Id != currentAllocationId.Value))
            .SumAsync(x => x.Amount ?? 0m);
    }

    private async Task<decimal> GetAllocatedQuantityForAidRequestLineAsync(Guid aidRequestLineId, Guid? currentAllocationId)
    {
        return await _db.Set<DonationAllocation>()
            .AsNoTracking()
            .Where(x => x.AidRequestLineId == aidRequestLineId && (!currentAllocationId.HasValue || x.Id != currentAllocationId.Value))
            .SumAsync(x => x.AllocatedQuantity ?? 0m);
    }

    private static string BuildAidRequestLineDisplay(AidRequestLineInfo line)
    {
        var title = !string.IsNullOrWhiteSpace(line.ItemNameSnapshot)
            ? line.ItemNameSnapshot
            : !string.IsNullOrWhiteSpace(line.Description)
                ? line.Description
                : "بند طلب مساعدة";

        var qty = line.ApprovedQuantity ?? line.RequestedQuantity;
        var qtyText = qty.HasValue && qty.Value > 0m ? $" | كمية {qty.Value:N2}" : string.Empty;
        var valueText = line.EstimatedTotalValue.HasValue && line.EstimatedTotalValue.Value > 0m
            ? $" | قيمة {line.EstimatedTotalValue.Value:N2}"
            : string.Empty;
        var methodText = string.IsNullOrWhiteSpace(line.FulfillmentMethod)
            ? string.Empty
            : $" | {ResolveFulfillmentMethodName(line.FulfillmentMethod)}";

        return $"{title}{qtyText}{valueText}{methodText}";
    }

    private static string ResolveFulfillmentMethodName(string? code)
        => code switch
        {
            "InKindFromStock" => "صرف عيني من المخزن",
            "CashEquivalent" => "صرف نقدي بالقيمة",
            "PurchaseNeedRequest" => "طلب احتياج للشراء",
            "VendorPayment" => "دفع لمورد / جهة خدمة",
            _ => "غير محدد"
        };

    private static void AddParameter(System.Data.Common.DbCommand command, string name, object? value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }

    private sealed class AidRequestLineInfo
    {
        public Guid Id { get; set; }
        public Guid AidRequestId { get; set; }
        public string? ItemNameSnapshot { get; set; }
        public string? Description { get; set; }
        public decimal? RequestedQuantity { get; set; }
        public decimal? ApprovedQuantity { get; set; }
        public decimal? EstimatedUnitValue { get; set; }
        public decimal? EstimatedTotalValue { get; set; }
        public string? FulfillmentMethod { get; set; }
        public Guid? WarehouseId { get; set; }
    }

    private async Task<BulkDonationAllocationVm> BuildBulkAllocationVmAsync(Donation donation, DateTime? allocatedDate, string? notes, Dictionary<Guid, decimal?>? enteredAmounts)
    {
        var donationRemaining = Math.Max(0m, (donation.Amount ?? 0m) - await _repository.GetAllocatedAmountAsync(donation.Id));
        var vm = new BulkDonationAllocationVm
        {
            DonationId = donation.Id,
            AllocatedDate = allocatedDate ?? DateTime.Today,
            Notes = notes,
            DonationAmount = donation.Amount ?? 0m,
            DonationRemainingAmount = donationRemaining,
            DonationType = donation.DonationType,
            AidTypeName = await ResolveAidTypeNameAsync(donation.AidTypeId)
        };

        if (!string.Equals(donation.DonationType, "نقدي", StringComparison.OrdinalIgnoreCase))
            return vm;

        if (!donation.AidTypeId.HasValue || donation.AidTypeId == Guid.Empty)
            return vm;

        var approvedRequests = await _db.Set<BeneficiaryAidRequest>()
            .AsNoTracking()
            .Include(x => x.Beneficiary)
            .Include(x => x.AidType)
            .Where(x => x.Status == "Approved" && x.AidTypeId == donation.AidTypeId.Value)
            .ToListAsync();

        var requestIds = approvedRequests.Select(x => x.Id).Distinct().ToList();
        var allocatedByRequest = requestIds.Count == 0
            ? new Dictionary<Guid, decimal>()
            : await _db.Set<DonationAllocation>()
                .AsNoTracking()
                .Where(x => x.AidRequestId.HasValue && requestIds.Contains(x.AidRequestId.Value))
                .GroupBy(x => x.AidRequestId!.Value)
                .Select(g => new { AidRequestId = g.Key, Amount = g.Sum(x => x.Amount ?? 0m) })
                .ToDictionaryAsync(x => x.AidRequestId, x => x.Amount);

        var disbursedByRequest = requestIds.Count == 0
            ? new Dictionary<Guid, decimal>()
            : await _db.Set<BeneficiaryAidDisbursement>()
                .AsNoTracking()
                .Where(x => x.AidRequestId.HasValue && requestIds.Contains(x.AidRequestId.Value))
                .GroupBy(x => x.AidRequestId!.Value)
                .Select(g => new { AidRequestId = g.Key, Amount = g.Sum(x => x.Amount ?? 0m) })
                .ToDictionaryAsync(x => x.AidRequestId, x => x.Amount);

        var beneficiaryIds = approvedRequests.Select(x => x.BeneficiaryId).Distinct().ToList();
        var disbursedBeneficiaryIds = beneficiaryIds.Count == 0
            ? new HashSet<Guid>()
            : (await _db.Set<BeneficiaryAidDisbursement>()
                .AsNoTracking()
                .Where(x => beneficiaryIds.Contains(x.BeneficiaryId))
                .Select(x => x.BeneficiaryId)
                .Distinct()
                .ToListAsync()).ToHashSet();

        var orderedRequests = approvedRequests
            .Select(x => new
            {
                Request = x,
                Requested = x.RequestedAmount ?? 0m,
                Funded = allocatedByRequest.TryGetValue(x.Id, out var funded) ? funded : 0m,
                Disbursed = disbursedByRequest.TryGetValue(x.Id, out var disbursed) ? disbursed : 0m,
                HasPreviousDisbursement = disbursedBeneficiaryIds.Contains(x.BeneficiaryId)
            })
            .Select(x => new
            {
                x.Request,
                x.Requested,
                x.Funded,
                x.Disbursed,
                x.HasPreviousDisbursement,
                RemainingNeed = x.Requested > 0m ? Math.Max(0m, x.Requested - x.Funded) : 0m
            })
            .Where(x => x.RemainingNeed > 0m)
            .OrderBy(x => x.HasPreviousDisbursement ? 1 : 0)
            .ThenBy(x => GetUrgencyRank(x.Request.UrgencyLevel))
            .ThenBy(x => x.Request.RequestDate)
            .ThenBy(x => x.Request.Beneficiary?.FullName ?? string.Empty)
            .ToList();

        vm.Rows = orderedRequests.Select(x => new BulkDonationAllocationRowVm
        {
            AidRequestId = x.Request.Id,
            BeneficiaryId = x.Request.BeneficiaryId,
            BeneficiaryName = x.Request.Beneficiary?.FullName ?? "مستفيد",
            RequestNumberOrDate = x.Request.RequestDate.ToString("yyyy-MM-dd"),
            AidTypeName = x.Request.AidType?.NameAr ?? "بدون نوع",
            UrgencyLevel = x.Request.UrgencyLevel,
            HasPreviousDisbursement = x.HasPreviousDisbursement,
            RequestedAmount = x.Requested,
            FundedAmount = x.Funded,
            DisbursedAmount = x.Disbursed,
            RemainingNeedAmount = x.RemainingNeed,
            AllocateAmount = enteredAmounts != null && enteredAmounts.TryGetValue(x.Request.Id, out var entered) ? entered : null
        }).ToList();

        return vm;
    }
    [HttpGet]
    public async Task<IActionResult> InKindGeneralCreate(
        Guid? aidTypeId = null, Guid? itemId = null, Guid? warehouseId = null)
    {
        var vm = new InKindGeneralAllocationVm
        {
            AidTypeId = aidTypeId,
            ItemId = itemId,
            WarehouseId = warehouseId,
            AllocatedDate = DateTime.Today
        };

        // ── Dropdowns ────────────────────────────────────────
        vm.AidTypes = (await _db.Set<InfrastrfuctureManagmentCore.Domains.Charity.Lookups.AidTypeLookup>()
            .AsNoTracking().OrderBy(x => x.DisplayOrder).ThenBy(x => x.NameAr).ToListAsync())
            .Select(x => new SelectListItem(x.NameAr, x.Id.ToString())).ToList();

        vm.Items = (await _db.Set<InfrastrfuctureManagmentCore.Domains.Item.Item>()
            .AsNoTracking().OrderBy(x => x.ItemNameAr).ToListAsync())
            .Select(x => new SelectListItem(x.ItemNameAr, x.Id.ToString())).ToList();

        vm.Warehouses = (await _db.Set<InfrastrfuctureManagmentCore.Domains.Warehouses.Warehouse>()
            .AsNoTracking().OrderBy(x => x.WarehouseNameAr).ToListAsync())
            .Select(x => new SelectListItem(x.WarehouseNameAr, x.Id.ToString())).ToList();

        if (!aidTypeId.HasValue) return View(vm);

        // ── Source Items — تبرعات عينية متاحة ──────────────
        var inKindItemsQuery = _db.Set<DonationInKindItem>()
            .AsNoTracking()
            .Include(x => x.Item)
            .Include(x => x.Warehouse)
            .Include(x => x.Donation).ThenInclude(d => d!.Donor)
            .Where(x => x.Donation != null && x.Donation.DonationType == "عيني");

        if (itemId.HasValue)
            inKindItemsQuery = inKindItemsQuery.Where(x => x.ItemId == itemId.Value);
        if (warehouseId.HasValue)
            inKindItemsQuery = inKindItemsQuery.Where(x => x.WarehouseId == warehouseId.Value);

        var inKindItems = await inKindItemsQuery.ToListAsync();

        foreach (var iki in inKindItems)
        {
            var allocated = await _donationInKindItemRepository.GetAllocatedQuantityAsync(iki.Id);
            var remaining = iki.Quantity - allocated;
            if (remaining <= 0) continue;
            vm.SourceItems.Add(new InKindSourceItemVm
            {
                DonationInKindItemId = iki.Id,
                DonationNumber = iki.Donation?.DonationNumber ?? "—",
                DonorName = iki.Donation?.Donor?.FullName ?? "—",
                ItemName = iki.Item?.ItemNameAr ?? iki.ItemId.ToString()[..8],
                TotalQuantity = iki.Quantity,
                AllocatedQuantity = allocated,
                RemainingQuantity = remaining,
                WarehouseName = iki.Warehouse?.WarehouseNameAr,
                ExpiryDate = iki.ExpiryDate
            });
        }

        if (!vm.SourceItems.Any()) return View(vm);

        // ── Eligible Aid Requests ────────────────────────────
        var eligibleLines = await _db.Set<BeneficiaryAidRequestLine>()
            .AsNoTracking()
            .Include(x => x.AidRequest).ThenInclude(r => r!.Beneficiary)
            .Include(x => x.Item)
            .Where(x =>
                x.AidRequest != null &&
                x.AidRequest.Status == "Approved" &&
                (x.FulfillmentMethod == "InKindFromDonation" || x.FulfillmentMethod == "InKindFromStock") &&
                x.ApprovedQuantity > 0 &&
                (itemId == null || x.ItemId == itemId))
            .OrderBy(x => x.AidRequest!.CreatedAtUtc)
            .ToListAsync();

        var today = DateTime.UtcNow.Date;
        foreach (var line in eligibleLines)
        {
            var alreadyAllocated = await _db.Set<DonationAllocation>()
                .Where(a => a.AidRequestLineId == line.Id && a.AllocatedQuantity > 0)
                .SumAsync(a => (decimal?)a.AllocatedQuantity) ?? 0;

            var remaining = (line.ApprovedQuantity ?? 0) - alreadyAllocated;
            if (remaining <= 0) continue;

            var hasDisbursement = await _db.Set<BeneficiaryAidDisbursement>()
                .AnyAsync(d => d.BeneficiaryId == line.AidRequest!.BeneficiaryId);

            vm.Rows.Add(new InKindAllocationRowVm
            {
                AidRequestId = line.AidRequestId,
                BeneficiaryId = line.AidRequest!.BeneficiaryId,
                AidRequestLineId = line.Id,
                BeneficiaryName = line.AidRequest.Beneficiary?.FullName ?? "—",
                RequestSummary = $"طلب {line.AidRequest.CreatedAtUtc:yyyy/MM/dd}",
                ItemName = line.Item?.ItemNameAr ?? line.ItemNameSnapshot ?? "صنف عيني",
                RequestedQuantity = line.RequestedQuantity ?? 0,
                AllocatedQuantity = alreadyAllocated,
                RemainingNeedQuantity = remaining,
                HasPreviousDisbursement = hasDisbursement
            });
        }

        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> InKindGeneralCreate(InKindGeneralAllocationVm vm)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(InKindGeneralCreate),
                new { aidTypeId = vm.AidTypeId, itemId = vm.ItemId, warehouseId = vm.WarehouseId });

        var userId = _userManager?.GetUserId(User) ?? "";
        var toCreate = vm.Rows
            .Where(r => r.AllocateQuantity > 0 && r.SelectedDonationInKindItemId.HasValue)
            .ToList();

        if (!toCreate.Any())
        {
            TempData["Error"] = "لم يتم إدخال أي كميات للتخصيص.";
            return RedirectToAction(nameof(InKindGeneralCreate),
                new { aidTypeId = vm.AidTypeId, itemId = vm.ItemId, warehouseId = vm.WarehouseId });
        }

        // Find first donation for each in-kind item (to link allocation to donation)
        var itemToDonation = await _db.Set<DonationInKindItem>()
            .Where(x => toCreate.Select(r => r.SelectedDonationInKindItemId!.Value).Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.DonationId);

        foreach (var row in toCreate)
        {
            if (!itemToDonation.TryGetValue(row.SelectedDonationInKindItemId!.Value, out var donationId))
                continue;

            _db.Set<DonationAllocation>().Add(new DonationAllocation
            {
                Id = Guid.NewGuid(),
                DonationId = donationId,
                DonationInKindItemId = row.SelectedDonationInKindItemId,
                AidRequestId = row.AidRequestId,
                BeneficiaryId = row.BeneficiaryId,
                AidRequestLineId = row.AidRequestLineId,
                AllocatedQuantity = row.AllocateQuantity,
                AllocatedDate = vm.AllocatedDate,
                Notes = vm.Notes,
                ApprovalStatus = "Approved",
                CreatedByUserId = userId,
                CreatedAtUtc = DateTime.UtcNow
            });
        }

        await _db.SaveChangesAsync();
        TempData["Success"] = $"تم التخصيص العيني بنجاح — {toCreate} طلب.";
        return RedirectToAction(nameof(InKindGeneralCreate),
            new { aidTypeId = vm.AidTypeId, itemId = vm.ItemId, warehouseId = vm.WarehouseId });
    }
    [HttpGet]
    public async Task<IActionResult> GeneralCreate(Guid? financialAccountId = null, Guid? aidTypeId = null, string? sourceScopeCode = null, string? purposeName = null)
    {
        var vm = await BuildGeneralFundingAllocationVmAsync(
            financialAccountId,
            aidTypeId,
            sourceScopeCode,
            purposeName,
            DateTime.Today,
            null,
            null);

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GeneralCreate(GeneralFundingAllocationVm vm)
    {
        var entered = vm.Rows?.ToDictionary(x => x.AidRequestId, x => x.AllocateAmount) ?? new Dictionary<Guid, decimal?>();
        var hydratedVm = await BuildGeneralFundingAllocationVmAsync(
            vm.FinancialAccountId,
            vm.AidTypeId,
            vm.SourceScopeCode,
            vm.PurposeName,
            vm.AllocatedDate,
            vm.Notes,
            entered);

        if (!vm.FinancialAccountId.HasValue || vm.FinancialAccountId == Guid.Empty)
            ModelState.AddModelError(nameof(vm.FinancialAccountId), "الحساب المالي المصدر مطلوب.");

        if (!vm.AidTypeId.HasValue || vm.AidTypeId == Guid.Empty)
            ModelState.AddModelError(nameof(vm.AidTypeId), "نوع المساعدة مطلوب.");

        if (string.Equals(hydratedVm.SourceScopeCode, DonationTargetingScopeOption.GeneralPurpose, StringComparison.OrdinalIgnoreCase)
            && string.IsNullOrWhiteSpace(hydratedVm.PurposeName))
            ModelState.AddModelError(nameof(vm.PurposeName), "اسم الغرض / الباب العام مطلوب.");

        if (hydratedVm.Rows.Count == 0)
            ModelState.AddModelError(string.Empty, "لا توجد طلبات مؤهلة للتخصيص من هذا الرصيد.");

        var selectedRows = hydratedVm.Rows
            .Select((row, index) => new { Row = row, Index = index })
            .Where(x => x.Row.AllocateAmount.HasValue && x.Row.AllocateAmount.Value > 0)
            .ToList();

        if (selectedRows.Count == 0)
            ModelState.AddModelError(string.Empty, "أدخل مبلغًا واحدًا على الأقل قبل الحفظ.");

        foreach (var item in hydratedVm.Rows.Select((row, index) => new { Row = row, Index = index }))
        {
            if (item.Row.AllocateAmount.HasValue && item.Row.AllocateAmount.Value < 0)
                ModelState.AddModelError($"Rows[{item.Index}].AllocateAmount", "لا يمكن إدخال مبلغ سالب.");

            if (item.Row.AllocateAmount.HasValue && item.Row.AllocateAmount.Value > item.Row.RemainingNeedAmount)
                ModelState.AddModelError($"Rows[{item.Index}].AllocateAmount", $"المبلغ المدخل أكبر من الاحتياج المتبقي للطلب ({item.Row.RemainingNeedAmount:N2}).");
        }

        var totalSelected = selectedRows.Sum(x => x.Row.AllocateAmount ?? 0m);
        if (totalSelected > hydratedVm.AvailablePoolAmount)
            ModelState.AddModelError(string.Empty, $"إجمالي التخصيص المطلوب ({totalSelected:N2}) أكبر من الرصيد المتاح ({hydratedVm.AvailablePoolAmount:N2}).");

        if (!ModelState.IsValid)
            return View(hydratedVm);

        var sourceDonations = await GetEligibleGeneralPoolDonationsAsync(hydratedVm.FinancialAccountId, hydratedVm.AidTypeId, hydratedVm.SourceScopeCode, hydratedVm.PurposeName);
        var sourceDonationIds = sourceDonations.Select(x => x.Id).ToList();
        var allocatedByDonation = sourceDonationIds.Count == 0
            ? new Dictionary<Guid, decimal>()
            : await _db.Set<DonationAllocation>()
                .AsNoTracking()
                .Where(x => sourceDonationIds.Contains(x.DonationId))
                .GroupBy(x => x.DonationId)
                .Select(g => new { DonationId = g.Key, Amount = g.Sum(x => x.Amount ?? 0m) })
                .ToDictionaryAsync(x => x.DonationId, x => x.Amount);

        var fundingQueue = sourceDonations
            .Select(x => new FundingSourceState
            {
                Donation = x,
                RemainingAmount = Math.Max(0m, (x.Amount ?? 0m) - (allocatedByDonation.TryGetValue(x.Id, out var allocated) ? allocated : 0m))
            })
            .Where(x => x.RemainingAmount > 0m)
            .OrderBy(x => x.Donation.DonationDate)
            .ThenBy(x => x.Donation.CreatedAtUtc)
            .ThenBy(x => x.Donation.DonationNumber)
            .ToList();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var now = DateTime.UtcNow;
        var allocations = new List<DonationAllocation>();

        foreach (var selected in selectedRows)
        {
            var remainingToAllocate = selected.Row.AllocateAmount ?? 0m;
            while (remainingToAllocate > 0m)
            {
                var source = fundingQueue.FirstOrDefault(x => x.RemainingAmount > 0m);
                if (source == null)
                {
                    ModelState.AddModelError(string.Empty, "نفد الرصيد المتاح أثناء توزيع السطور. أعد التحميل ثم حاول مرة أخرى.");
                    return View(hydratedVm);
                }

                var take = Math.Min(source.RemainingAmount, remainingToAllocate);
                var entity = new DonationAllocation
                {
                    Id = Guid.NewGuid(),
                    DonationId = source.Donation.Id,
                    AllocatedDate = hydratedVm.AllocatedDate,
                    AidRequestId = selected.Row.AidRequestId,
                    BeneficiaryId = selected.Row.BeneficiaryId,
                    Amount = take,
                    ApprovalStatus = AllocationApprovedStatus,
                    ApprovedByUserId = userId,
                    ApprovedAtUtc = now,
                    Notes = string.IsNullOrWhiteSpace(hydratedVm.Notes)
                        ? $"تخصيص عام من الرصيد {source.Donation.DonationNumber}"
                        : hydratedVm.Notes!.Trim(),
                    CreatedByUserId = userId,
                    CreatedAtUtc = now
                };

                allocations.Add(entity);
                source.RemainingAmount -= take;
                remainingToAllocate -= take;
            }
        }

        await _db.Set<DonationAllocation>().AddRangeAsync(allocations);
        await _db.SaveChangesAsync();

        var pendingDisbursementCount = await EnsurePendingAidDisbursementsForAllocationsAsync(allocations);
        TempData["Success"] = BuildAllocationWorkflowSuccessMessage(
            $"تم حفظ {allocations.Count} سطر تخصيص عام بإجمالي {totalSelected:N2}.",
            pendingDisbursementCount);
        return RedirectToAction(nameof(GeneralCreate), new
        {
            financialAccountId = hydratedVm.FinancialAccountId,
            aidTypeId = hydratedVm.AidTypeId,
            sourceScopeCode = hydratedVm.SourceScopeCode,
            purposeName = hydratedVm.PurposeName
        });
    }

    private async Task<GeneralFundingAllocationVm> BuildGeneralFundingAllocationVmAsync(
        Guid? financialAccountId,
        Guid? aidTypeId,
        string scopeCode,
        string? purposeName,
        DateTime? allocatedDate,
        string? notes,
        Dictionary<Guid, decimal?>? enteredAmounts)
    {
        var vm = new GeneralFundingAllocationVm
        {
            FinancialAccountId = financialAccountId,
            AidTypeId = aidTypeId,
            SourceScopeCode = NormalizeSourceScopeCode(scopeCode),
            PurposeName = purposeName,
            AllocatedDate = allocatedDate ?? DateTime.Today,
            Notes = notes
        };

        await FillGeneralFundingLookupsAsync(vm);
        vm.FinancialAccountName = vm.FinancialAccounts.FirstOrDefault(x => x.Value == financialAccountId?.ToString())?.Text;
        vm.AidTypeName = vm.AidTypes.FirstOrDefault(x => x.Value == aidTypeId?.ToString())?.Text;

        if (!financialAccountId.HasValue || financialAccountId == Guid.Empty || !aidTypeId.HasValue || aidTypeId == Guid.Empty)
            return vm;

        var sourceDonations = await GetEligibleGeneralPoolDonationsAsync(financialAccountId, aidTypeId, vm.SourceScopeCode, purposeName);
        var donationIds = sourceDonations.Select(x => x.Id).ToList();
        var allocatedByDonation = donationIds.Count == 0
            ? new Dictionary<Guid, decimal>()
            : await _db.Set<DonationAllocation>()
                .AsNoTracking()
                .Where(x => donationIds.Contains(x.DonationId))
                .GroupBy(x => x.DonationId)
                .Select(g => new { DonationId = g.Key, Amount = g.Sum(x => x.Amount ?? 0m) })
                .ToDictionaryAsync(x => x.DonationId, x => x.Amount);

        vm.SourceDonations = sourceDonations
            .Select(x =>
            {
                var allocated = allocatedByDonation.TryGetValue(x.Id, out var value) ? value : 0m;
                var remaining = Math.Max(0m, (x.Amount ?? 0m) - allocated);
                return new GeneralFundingSourceDonationVm
                {
                    DonationId = x.Id,
                    DonationNumber = x.DonationNumber,
                    DonationDate = x.DonationDate,
                    DonorName = x.Donor?.FullName,
                    DonationAmount = x.Amount ?? 0m,
                    AllocatedAmount = allocated,
                    RemainingAmount = remaining,
                    PurposeName = ResolvePurposeName(x),
                    AidTypeName = x.AidType?.NameAr
                };
            })
            .Where(x => x.RemainingAmount > 0m)
            .OrderBy(x => x.DonationDate)
            .ThenBy(x => x.DonationNumber)
            .ToList();

        vm.AvailablePoolAmount = vm.SourceDonations.Sum(x => x.RemainingAmount);

        var approvedRequests = await _db.Set<BeneficiaryAidRequest>()
            .AsNoTracking()
            .Include(x => x.Beneficiary)
            .Include(x => x.AidType)
            .Where(x => x.Status == "Approved" && x.AidTypeId == aidTypeId.Value)
            .ToListAsync();

        if (string.Equals(vm.SourceScopeCode, DonationTargetingScopeOption.GeneralPurpose, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(vm.PurposeName))
            approvedRequests = approvedRequests.Where(x => AidRequestMatchesPurpose(x, vm.PurposeName)).ToList();

        var requestIds = approvedRequests.Select(x => x.Id).Distinct().ToList();
        var allocatedByRequest = requestIds.Count == 0
            ? new Dictionary<Guid, decimal>()
            : await _db.Set<DonationAllocation>()
                .AsNoTracking()
                .Where(x => x.AidRequestId.HasValue && requestIds.Contains(x.AidRequestId.Value))
                .GroupBy(x => x.AidRequestId!.Value)
                .Select(g => new { AidRequestId = g.Key, Amount = g.Sum(x => x.Amount ?? 0m) })
                .ToDictionaryAsync(x => x.AidRequestId, x => x.Amount);

        var disbursedByRequest = requestIds.Count == 0
            ? new Dictionary<Guid, decimal>()
            : await _db.Set<BeneficiaryAidDisbursement>()
                .AsNoTracking()
                .Where(x => x.AidRequestId.HasValue && requestIds.Contains(x.AidRequestId.Value))
                .GroupBy(x => x.AidRequestId!.Value)
                .Select(g => new { AidRequestId = g.Key, Amount = g.Sum(x => x.Amount ?? 0m) })
                .ToDictionaryAsync(x => x.AidRequestId, x => x.Amount);

        var beneficiaryIds = approvedRequests.Select(x => x.BeneficiaryId).Distinct().ToList();
        var disbursedBeneficiaryIds = beneficiaryIds.Count == 0
            ? new HashSet<Guid>()
            : (await _db.Set<BeneficiaryAidDisbursement>()
                .AsNoTracking()
                .Where(x => beneficiaryIds.Contains(x.BeneficiaryId))
                .Select(x => x.BeneficiaryId)
                .Distinct()
                .ToListAsync()).ToHashSet();

        var orderedRequests = approvedRequests
            .Select(x => new
            {
                Request = x,
                Requested = x.RequestedAmount ?? 0m,
                Funded = allocatedByRequest.TryGetValue(x.Id, out var funded) ? funded : 0m,
                Disbursed = disbursedByRequest.TryGetValue(x.Id, out var disbursed) ? disbursed : 0m,
                HasPreviousDisbursement = disbursedBeneficiaryIds.Contains(x.BeneficiaryId)
            })
            .Select(x => new
            {
                x.Request,
                x.Requested,
                x.Funded,
                x.Disbursed,
                x.HasPreviousDisbursement,
                RemainingNeed = x.Requested > 0m ? Math.Max(0m, x.Requested - x.Funded) : 0m
            })
            .Where(x => x.RemainingNeed > 0m)
            .OrderBy(x => x.HasPreviousDisbursement ? 1 : 0)
            .ThenBy(x => GetUrgencyRank(x.Request.UrgencyLevel))
            .ThenBy(x => x.Request.RequestDate)
            .ThenBy(x => x.Request.Beneficiary?.FullName ?? string.Empty)
            .ToList();

        vm.Rows = orderedRequests.Select(x => new GeneralFundingAllocationRowVm
        {
            AidRequestId = x.Request.Id,
            BeneficiaryId = x.Request.BeneficiaryId,
            BeneficiaryName = x.Request.Beneficiary?.FullName ?? "مستفيد",
            RequestNumberOrDate = x.Request.RequestDate.ToString("yyyy-MM-dd"),
            AidTypeName = x.Request.AidType?.NameAr ?? "بدون نوع",
            UrgencyLevel = x.Request.UrgencyLevel,
            HasPreviousDisbursement = x.HasPreviousDisbursement,
            RequestedAmount = x.Requested,
            FundedAmount = x.Funded,
            DisbursedAmount = x.Disbursed,
            RemainingNeedAmount = x.RemainingNeed,
            AllocateAmount = enteredAmounts != null && enteredAmounts.TryGetValue(x.Request.Id, out var entered) ? entered : null
        }).ToList();

        vm.SelectedTotalAmount = vm.Rows.Sum(x => x.AllocateAmount ?? 0m);
        return vm;
    }

    private async Task FillGeneralFundingLookupsAsync(GeneralFundingAllocationVm vm)
    {
        vm.SourceScopes = new List<SelectListItem>
        {
            new() { Value = DonationTargetingScopeOption.GeneralFund, Text = "رصيد التبرعات العام" },
            new() { Value = DonationTargetingScopeOption.GeneralPurpose, Text = "رصيد غرض / باب عام" }
        };

        // ── تحميل الحسابات المالية مباشرة من جدول FinancialAccount ──
        // (بدلاً من الاعتماد على التبرعات الموجودة فقط)
        var accounts = await _db.Set<InfrastrfuctureManagmentCore.Domains.Financial.FinancialAccount>()
            .AsNoTracking()
            .Where(x => x.IsPosting && x.IsActive &&
                   (x.Category == InfrastrfuctureManagmentCore.Domains.Financial.AccountCategory.Asset ||
                    x.Category == InfrastrfuctureManagmentCore.Domains.Financial.AccountCategory.Revenue))
            .OrderBy(x => x.AccountCode)
            .ThenBy(x => x.AccountNameAr)
            .ToListAsync();

        var aidTypes = await _db.Set<InfrastrfuctureManagmentCore.Domains.Charity.Lookups.AidTypeLookup>()
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.NameAr)
            .ToListAsync();

        vm.FinancialAccounts = accounts
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = $"{x.AccountCode} - {x.AccountNameAr}" })
            .ToList();

        vm.AidTypes = aidTypes
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.NameAr })
            .ToList();
    }

    private async Task<List<Donation>> GetEligibleGeneralPoolDonationsAsync(Guid? financialAccountId, Guid? aidTypeId, string scopeCode, string? purposeName)
    {
        if (!financialAccountId.HasValue || financialAccountId == Guid.Empty)
            return new List<Donation>();

        var donations = await _db.CharityDonations
            .AsNoTracking()
            .Include(x => x.Donor)
            .Include(x => x.AidType)
            .Where(x => x.DonationType == "نقدي" && x.FinancialAccountId == financialAccountId.Value)
            .OrderBy(x => x.DonationDate)
            .ThenBy(x => x.CreatedAtUtc)
            .ToListAsync();

        var sr = donations
           .Where(x => MatchesGeneralPoolDonation(x, NormalizeSourceScopeCode(scopeCode), purposeName))
           .Where(x => !aidTypeId.HasValue || aidTypeId == Guid.Empty || !x.AidTypeId.HasValue || x.AidTypeId == aidTypeId.Value)
           .ToList();
        return sr;
    }


    private async Task<int> EnsurePendingAidDisbursementsForAllocationsAsync(IEnumerable<DonationAllocation> allocations)
    {
        var allocationList = allocations
     .Where(x => x.Id != Guid.Empty
         && x.AidRequestId.HasValue
         && x.BeneficiaryId.HasValue
         && (((x.Amount ?? 0m) > 0m) || ((x.AllocatedQuantity ?? 0m) > 0m && x.DonationInKindItemId.HasValue)))
     .ToList();

        if (allocationList.Count == 0)
            return 0;

        var allocationIds = allocationList.Select(x => x.Id).Distinct().ToList();
        var existingLineAllocationIds = await _db.Set<BeneficiaryAidDisbursementFundingLine>()
            .AsNoTracking()
            .Where(x => allocationIds.Contains(x.DonationAllocationId))
            .Select(x => x.DonationAllocationId)
            .Distinct()
            .ToListAsync();

        var existingLineAllocationSet = existingLineAllocationIds.ToHashSet();
        allocationList = allocationList
            .Where(x => !existingLineAllocationSet.Contains(x.Id))
            .ToList();

        if (allocationList.Count == 0)
            return 0;

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var now = DateTime.UtcNow;

        var allocationIdsToApprove = allocationList.Select(x => x.Id).Distinct().ToList();
        var trackedAllocations = await _db.Set<DonationAllocation>()
            .Where(x => allocationIdsToApprove.Contains(x.Id))
            .ToListAsync();
        var inKindItemIdsForDisbursement = allocationList
    .Where(x => x.DonationInKindItemId.HasValue)
    .Select(x => x.DonationInKindItemId!.Value)
    .Distinct()
    .ToList();

        var inKindItemsForDisbursement = inKindItemIdsForDisbursement.Count == 0
            ? new Dictionary<Guid, DonationInKindItem>()
            : await _db.Set<DonationInKindItem>()
                .AsNoTracking()
                .Include(x => x.Item)
                .Where(x => inKindItemIdsForDisbursement.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id);
        foreach (var allocation in trackedAllocations)
        {
            allocation.ApprovalStatus = AllocationApprovedStatus;
            allocation.ApprovedByUserId = currentUserId;
            allocation.ApprovedAtUtc = now;
            allocation.RejectedByUserId = null;
            allocation.RejectedAtUtc = null;
            allocation.ApprovalNotes ??= "تم اعتماد التخصيص تلقائيًا لإنشاء سجل صرف متاح للصرف.";
        }

        var aidRequestIds = allocationList
            .Select(x => x.AidRequestId!.Value)
            .Distinct()
            .ToList();

        var aidRequests = await _db.Set<BeneficiaryAidRequest>()
            .AsNoTracking()
            .Where(x => aidRequestIds.Contains(x.Id))
            .Select(x => new
            {
                x.Id,
                x.BeneficiaryId,
                x.AidTypeId,
                x.ProjectId,
                x.Status
            })
            .ToDictionaryAsync(x => x.Id);

        var donationIds = allocationList
            .Select(x => x.DonationId)
            .Distinct()
            .ToList();

        var donations = await _db.Set<Donation>()
            .AsNoTracking()
            .Where(x => donationIds.Contains(x.Id))
            .Select(x => new
            {
                x.Id,
                x.FinancialAccountId,
                x.PaymentMethodId
            })
            .ToDictionaryAsync(x => x.Id);

        var inKindItemIds = allocationList
            .Where(x => x.DonationInKindItemId.HasValue)
            .Select(x => x.DonationInKindItemId!.Value)
            .Distinct()
            .ToList();

        var inKindItems = inKindItemIds.Count == 0
            ? new Dictionary<Guid, DonationInKindItem>()
            : await _db.Set<DonationInKindItem>()
                .AsNoTracking()
                .Include(x => x.Item)
                .Where(x => inKindItemIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id);

        var affectedDisbursementIds = new HashSet<Guid>();

        foreach (var requestGroup in allocationList.GroupBy(x => new
        {
            AidRequestId = x.AidRequestId!.Value,
            AidRequestLineId = x.AidRequestLineId,
            IsInKind = x.DonationInKindItemId.HasValue
        }))
        {
            if (!aidRequests.TryGetValue(requestGroup.Key.AidRequestId, out var aidRequest))
                continue;

            if (!string.Equals(aidRequest.Status, "Approved", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(aidRequest.Status, "Disbursed", StringComparison.OrdinalIgnoreCase))
                continue;

            var groupAllocations = requestGroup.ToList();
            var isInKind = requestGroup.Key.IsInKind;
            var amountToReserve = isInKind
                ? groupAllocations.Sum(x =>
                {
                    if (!x.DonationInKindItemId.HasValue || !inKindItems.TryGetValue(x.DonationInKindItemId.Value, out var inKindItem))
                        return 0m;

                    return (x.AllocatedQuantity ?? 0m) * (inKindItem.EstimatedUnitValue ?? 0m);
                })
                : groupAllocations.Sum(x => x.Amount ?? 0m);

            if (!isInKind && amountToReserve <= 0m)
                continue;

            var donationId = ResolveSingleGuid(groupAllocations.Select(x => x.DonationId));
            var financialAccountId = isInKind ? null : ResolveSingleNullableGuid(groupAllocations.Select(x =>
                donations.TryGetValue(x.DonationId, out var donation) ? donation.FinancialAccountId : null));
            var paymentMethodId = isInKind ? null : ResolveSingleNullableGuid(groupAllocations.Select(x =>
                donations.TryGetValue(x.DonationId, out var donation) ? donation.PaymentMethodId : null));
            var sourceType = isInKind ? AutoInKindDisbursementSourceType : AutoDisbursementSourceType;
            var approvalStatus = isInKind ? AllocationApprovedStatus : DisbursementPendingStatus;

            BeneficiaryAidDisbursement? pendingDisbursement = null;
            if (!isInKind)
            {
                pendingDisbursement = await _db.Set<BeneficiaryAidDisbursement>()
     .FirstOrDefaultAsync(x => x.AidRequestId == requestGroup.Key.AidRequestId
         && x.AidRequestLineId == requestGroup.Key.AidRequestLineId
         && x.SourceType == AutoDisbursementSourceType
         && x.ApprovalStatus == DisbursementPendingStatus
         && x.ExecutionStatus != "Cancelled");
            }

            if (pendingDisbursement == null)
            {
                pendingDisbursement = new BeneficiaryAidDisbursement
                {
                    Id = Guid.NewGuid(),
                    BeneficiaryId = aidRequest.BeneficiaryId,
                    AidRequestId = aidRequest.Id,
                    AidRequestLineId = requestGroup.Key.AidRequestLineId,
                    AidTypeId = aidRequest.AidTypeId,
                    DisbursementDate = groupAllocations.Min(x => x.AllocatedDate),
                    Amount = amountToReserve > 0m ? amountToReserve : null,
                    PaymentMethodId = paymentMethodId,
                    FinancialAccountId = financialAccountId,
                    ProjectId = aidRequest.ProjectId,
                    DonationId = donationId,
                    ApprovalStatus = approvalStatus,
                    ApprovedAtUtc = isInKind ? now : null,
                    ApprovedByUserId = isInKind ? currentUserId : null,
                    RejectedAtUtc = null,
                    RejectedByUserId = null,
                    ExecutionStatus = DisbursementAvailableExecutionStatus,
                    ExecutedAmount = 0m,
                    ExecutedAtUtc = null,
                    ExecutedByUserId = null,
                    SourceType = sourceType,
                    SourceId = requestGroup.Key.AidRequestLineId ?? aidRequest.Id,
                    Notes = isInKind
                        ? "تم إنشاء سجل صرف عيني متاح للصرف بناءً على تخصيص تبرع عيني. يتم إنشاء إذن الصرف المخزني من سجل الصرف."
                        : "تم إنشاء سجل صرف نقدي معلق بناءً على تخصيص تبرع لطلب المساعدة.",
                    CreatedByUserId = currentUserId,
                    CreatedAtUtc = now
                };

                await _db.Set<BeneficiaryAidDisbursement>().AddAsync(pendingDisbursement);
            }
            else
            {
                pendingDisbursement.Amount = (pendingDisbursement.Amount ?? 0m) + amountToReserve;
                pendingDisbursement.DisbursementDate = pendingDisbursement.DisbursementDate == default
                    ? groupAllocations.Min(x => x.AllocatedDate)
                    : pendingDisbursement.DisbursementDate;

                pendingDisbursement.FinancialAccountId ??= financialAccountId;
                pendingDisbursement.PaymentMethodId ??= paymentMethodId;

                if (isInKind)
                {
                    pendingDisbursement.ApprovalStatus = AllocationApprovedStatus;
                    pendingDisbursement.ApprovedAtUtc ??= now;
                    pendingDisbursement.ApprovedByUserId ??= currentUserId;
                }

                if (!pendingDisbursement.DonationId.HasValue)
                    pendingDisbursement.DonationId = donationId;
                else if (donationId.HasValue && pendingDisbursement.DonationId.Value != donationId.Value)
                    pendingDisbursement.DonationId = null;

                if (string.IsNullOrWhiteSpace(pendingDisbursement.Notes))
                    pendingDisbursement.Notes = isInKind
                        ? "تم تحديث سجل الصرف العيني المتاح بناءً على تخصيصات عينية إضافية."
                        : "تم تحديث سجل الصرف النقدي المعلق بناءً على تخصيصات تبرعات إضافية.";
            }

            affectedDisbursementIds.Add(pendingDisbursement.Id);

            foreach (var allocation in groupAllocations)
            {
                var consumedAmount = allocation.Amount ?? 0m;
                if (isInKind && allocation.DonationInKindItemId.HasValue && inKindItems.TryGetValue(allocation.DonationInKindItemId.Value, out var inKindItem))
                    consumedAmount = (allocation.AllocatedQuantity ?? 0m) * (inKindItem.EstimatedUnitValue ?? 0m);

                await _db.Set<BeneficiaryAidDisbursementFundingLine>().AddAsync(new BeneficiaryAidDisbursementFundingLine
                {
                    Id = Guid.NewGuid(),
                    DisbursementId = pendingDisbursement.Id,
                    DonationAllocationId = allocation.Id,
                    AmountConsumed = consumedAmount,
                    CreatedByUserId = currentUserId,
                    CreatedAtUtc = now
                });
            }
        }

        await _db.SaveChangesAsync();
        return affectedDisbursementIds.Count;
    }


    private static string BuildAllocationWorkflowSuccessMessage(string baseMessage, int pendingDisbursementCount)
    {
        var parts = new List<string> { baseMessage.TrimEnd('.') };

        if (pendingDisbursementCount > 0)
            parts.Add($"تم إنشاء/تحديث {pendingDisbursementCount} سجل صرف متاح للصرف");

        return string.Join("، ", parts) + ".";
    }

    private static Guid? ResolveSingleGuid(IEnumerable<Guid> values)
    {
        var ids = values.Where(x => x != Guid.Empty).Distinct().ToList();
        return ids.Count == 1 ? ids[0] : null;
    }

    private static Guid? ResolveSingleNullableGuid(IEnumerable<Guid?> values)
    {
        var ids = values
            .Where(x => x.HasValue && x.Value != Guid.Empty)
            .Select(x => x!.Value)
            .Distinct()
            .ToList();

        return ids.Count == 1 ? ids[0] : null;
    }

    private sealed class FundingSourceState
    {
        public Donation Donation { get; set; } = null!;
        public decimal RemainingAmount { get; set; }
    }

    private async Task<string?> ResolveAidTypeNameAsync(Guid? aidTypeId)
    {
        if (!aidTypeId.HasValue || aidTypeId == Guid.Empty)
            return null;

        return await _db.Set<InfrastrfuctureManagmentCore.Domains.Charity.Lookups.AidTypeLookup>()
            .AsNoTracking()
            .Where(x => x.Id == aidTypeId.Value)
            .Select(x => x.NameAr)
            .FirstOrDefaultAsync();
    }

    private static int GetUrgencyRank(string? urgencyLevel)
    {
        var value = (urgencyLevel ?? string.Empty).Trim().ToLowerInvariant();
        return value switch
        {
            "urgent" or "high" or "عاجل" or "مرتفعة" => 0,
            "medium" or "متوسطة" => 1,
            _ => 2
        };
    }
}
