using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentServices.Charity.Beneficiaries;
using InfrastructureManagmentServices.Charity.Funding;
using InfrastructureManagmentServices.Charity.Workflow;
using InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.AidRequests;
using InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.Shared;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Lookups;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Stores;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class BeneficiaryAidRequestsController : Controller
{
    private readonly IBeneficiaryAidRequestRepository _repository;
    private readonly IBeneficiaryRepository _beneficiaryRepository;
    private readonly ICharityLookupRepository _lookupRepository;
    private readonly ICharityProjectRepository _projectRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IWorkflowService _workflowService;
    private readonly IAidRequestFundingService _aidRequestFundingService;
    private readonly ICharityOperationalStatusService _operationalStatusService;
    private readonly AppDbContext _db;

    public BeneficiaryAidRequestsController(
        IBeneficiaryAidRequestRepository repository,
        IBeneficiaryRepository beneficiaryRepository,
        ICharityLookupRepository lookupRepository,
        ICharityProjectRepository projectRepository,
        IWarehouseRepository warehouseRepository,
        IItemRepository itemRepository,
        IWorkflowService workflowService,
        IAidRequestFundingService aidRequestFundingService,
        ICharityOperationalStatusService operationalStatusService,
        AppDbContext db)
    {
        _repository = repository;
        _beneficiaryRepository = beneficiaryRepository;
        _lookupRepository = lookupRepository;
        _projectRepository = projectRepository;
        _warehouseRepository = warehouseRepository;
        _itemRepository = itemRepository;
        _workflowService = workflowService;
        _aidRequestFundingService = aidRequestFundingService;
        _operationalStatusService = operationalStatusService;
        _db = db;
    }

    public async Task<IActionResult> Index(Guid? aidTypeId =null, Guid? beneficiaryId = null)
    {
        var items = new List<BeneficiaryAidRequest>() { };
        if (beneficiaryId.HasValue && beneficiaryId.Value != Guid.Empty)
        {
            if (!await PopulateBeneficiaryAsync(beneficiaryId.Value))
                return NotFound();
            items = await _repository.GetByBeneficiaryIdAsync(beneficiaryId.Value);
        }
        else if (aidTypeId.HasValue && aidTypeId.Value != Guid.Empty)
        {
            items = await _repository.GetAidTypeAsync((Guid)aidTypeId);
        }
        else 
        {
            ClearBeneficiaryContext();
            items = await _repository.GetAllAsync();
        }
         

        var requestIds = items.Select(x => x.Id).ToList();

        var snapshots = await _aidRequestFundingService.GetSnapshotsAsync(requestIds);
        var operationalSnapshots = await _operationalStatusService.GetAidRequestStatusesAsync(requestIds);

        var model = items.Select(x =>
        {
            snapshots.TryGetValue(x.Id, out var snapshot);
            operationalSnapshots.TryGetValue(x.Id, out var operational);

            return new BeneficiaryAidRequestListItemVm
            {
                Id = x.Id,
                BeneficiaryId = x.BeneficiaryId,
                BeneficiaryCode = x.Beneficiary?.Code,
                BeneficiaryName = x.Beneficiary?.FullName,
                RequestDate = x.RequestDate,
                AidType = x.AidType?.NameAr,
                RequestedAmount = x.RequestedAmount,
                FundedAmount = snapshot?.FundedAmount ?? 0m,
                DisbursedAmount = snapshot?.DisbursedAmount ?? 0m,
                RemainingToFundAmount = snapshot?.RemainingToFundAmount ?? 0m,
                RemainingToDisburseAmount = snapshot?.RemainingToDisburseAmount ?? 0m,
                RemainingOnRequestAmount = snapshot?.RemainingOnRequestAmount ?? 0m,
                UrgencyLevel = x.UrgencyLevel,
                Status = x.Status,
                Reason = x.Reason,
                FundingStatusCode = snapshot?.FundingStatusCode ?? AidRequestFundingStatusCodes.NotFunded,
                FundingStatusName = snapshot?.FundingStatusName ?? "غير ممول",
                DisbursementStatusCode = snapshot?.DisbursementStatusCode ?? AidRequestFundingStatusCodes.NotDisbursed,
                DisbursementStatusName = snapshot?.DisbursementStatusName ?? "غير مصروف",
                OperationalStatusCode = operational?.OperationalStatusCode ?? CharityOperationalStatusCodes.Open,
                OperationalStatusName = operational?.OperationalStatusName ?? "مفتوح",
                IsOperationallyClosed = operational?.IsClosed ?? false
            };
        }).ToList();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid? beneficiaryId = null)
    {
        if (beneficiaryId.HasValue && beneficiaryId.Value != Guid.Empty)
        {
            if (!await PopulateBeneficiaryAsync(beneficiaryId.Value))
                return NotFound();

            var committeeValidation = await BeneficiaryCommitteeDecisionGuard
                .ValidateHasApprovedDecisionAsync(_db, beneficiaryId.Value, "إضافة طلب مساعدة");

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

        var vm = new CreateBeneficiaryAidRequestVm
        {
            BeneficiaryId = beneficiaryId ?? Guid.Empty,
            RequestDate = DateTime.Today,
            Status = "Pending",
            Lines = { new BeneficiaryAidRequestLineVm() }
        };

        await FillLookupsAsync(vm);
        return View(vm);
    }
    [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBeneficiaryAidRequestVm vm)
        {
            if (vm.BeneficiaryId == Guid.Empty)
                ModelState.AddModelError(nameof(vm.BeneficiaryId), "المستفيد مطلوب");

            if (vm.BeneficiaryId != Guid.Empty && !await PopulateBeneficiaryAsync(vm.BeneficiaryId))
                return NotFound();

            await FillLookupsAsync(vm);

            var normalizedLines = NormalizeAidRequestLines(vm.Lines);
            ApplyAidRequestLinesTotals(vm, normalizedLines);
            ValidateAidRequestLines(normalizedLines);

            await ValidateCommitteeDecisionLimitAsync(vm.BeneficiaryId, vm.AidTypeId, vm.RequestedAmount, nameof(vm.RequestedAmount));

            if (!ModelState.IsValid)
                return View(vm);

            var entity = new BeneficiaryAidRequest
            {
                Id = Guid.NewGuid(),
                BeneficiaryId = vm.BeneficiaryId,
                RequestDate = vm.RequestDate,
                AidTypeId = vm.AidTypeId,
                RequestedAmount = vm.RequestedAmount,
                ProjectId = vm.ProjectId,
                Reason = vm.Reason?.Trim(),
                UrgencyLevel = vm.UrgencyLevel?.Trim(),
                Status = string.IsNullOrWhiteSpace(vm.Status) ? "Pending" : vm.Status.Trim(),
                CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CreatedAtUtc = DateTime.UtcNow
            };

            await _repository.AddAsync(entity);
            await SaveAidRequestLinesAsync(entity.Id, normalizedLines, entity.CreatedByUserId);
            TempData["Success"] = "تم حفظ طلب المساعدة بنجاح";
            await _workflowService.InitiateAsync(
                "AidRequest",
                entity.Id,
                $"طلب مساعدة — {entity.Beneficiary?.FullName ?? "مستفيد"}",
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);
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

            var vm = new EditBeneficiaryAidRequestVm
            {
                Id = entity.Id,
                BeneficiaryId = entity.BeneficiaryId,
                RequestDate = entity.RequestDate,
                AidTypeId = entity.AidTypeId,
                RequestedAmount = entity.RequestedAmount,
                ProjectId = entity.ProjectId,
                Reason = entity.Reason,
                UrgencyLevel = entity.UrgencyLevel,
                Status = entity.Status,
                Lines = await GetAidRequestLineVmsAsync(entity.Id)
            };

            await FillLookupsAsync(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditBeneficiaryAidRequestVm vm)
        {
            var entity = await _repository.GetByIdAsync(vm.Id);
            if (entity == null)
                return NotFound();

            if (vm.BeneficiaryId == Guid.Empty)
                ModelState.AddModelError(nameof(vm.BeneficiaryId), "المستفيد مطلوب");

            if (vm.BeneficiaryId != Guid.Empty && !await PopulateBeneficiaryAsync(vm.BeneficiaryId))
                return NotFound();

            await FillLookupsAsync(vm);

            var normalizedLines = NormalizeAidRequestLines(vm.Lines);
            ApplyAidRequestLinesTotals(vm, normalizedLines);
            ValidateAidRequestLines(normalizedLines);

            await ValidateCommitteeDecisionLimitAsync(vm.BeneficiaryId, vm.AidTypeId, vm.RequestedAmount, nameof(vm.RequestedAmount), vm.Id);

            if (!ModelState.IsValid)
                return View(vm);

            entity.BeneficiaryId = vm.BeneficiaryId;
            entity.RequestDate = vm.RequestDate;
            entity.AidTypeId = vm.AidTypeId;
            entity.RequestedAmount = vm.RequestedAmount;
            entity.ProjectId = vm.ProjectId;
            entity.Reason = vm.Reason?.Trim();
            entity.UrgencyLevel = vm.UrgencyLevel?.Trim();
            entity.Status = string.IsNullOrWhiteSpace(vm.Status) ? entity.Status : vm.Status.Trim();

            await _repository.UpdateAsync(entity);
            await SaveAidRequestLinesAsync(entity.Id, normalizedLines, User.FindFirstValue(ClaimTypes.NameIdentifier), replaceExisting: true);
            TempData["Success"] = "تم تعديل طلب المساعدة بنجاح";
            return RedirectToAction(nameof(Index), new { beneficiaryId = entity.BeneficiaryId });
        }
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        if (!await PopulateBeneficiaryAsync(entity.BeneficiaryId))
            return NotFound();

        var snapshots = await _aidRequestFundingService.GetSnapshotsAsync(new[] { entity.Id });
        snapshots.TryGetValue(entity.Id, out var snapshot);

        var operationalSnapshots = await _operationalStatusService.GetAidRequestStatusesAsync(new[] { entity.Id });
        operationalSnapshots.TryGetValue(entity.Id, out var operational);
        var needRequestsByLine = await _db.Set<InfrastrfuctureManagmentCore.Domains.Charity.StockAdvanced.StockNeedRequest>()
            .AsNoTracking()
            .Where(x => x.BeneficiaryAidRequestId == entity.Id && x.BeneficiaryAidRequestLineId.HasValue)
            .ToDictionaryAsync(x => x.BeneficiaryAidRequestLineId!.Value, x => x.Id.ToString() + "|" + x.RequestNumber + "|" + x.Status);

        ViewBag.NeedRequestsByLine = needRequestsByLine;


        var vm = new BeneficiaryAidRequestListItemVm
        {
            Id = entity.Id,
            BeneficiaryId = entity.BeneficiaryId,
            BeneficiaryCode = entity.Beneficiary?.Code,
            BeneficiaryName = entity.Beneficiary?.FullName,
            RequestDate = entity.RequestDate,
            AidType = entity.AidType?.NameAr,
            RequestedAmount = entity.RequestedAmount,
            FundedAmount = snapshot?.FundedAmount ?? 0m,
            DisbursedAmount = snapshot?.DisbursedAmount ?? 0m,
            RemainingToFundAmount = snapshot?.RemainingToFundAmount ?? 0m,
            RemainingToDisburseAmount = snapshot?.RemainingToDisburseAmount ?? 0m,
            RemainingOnRequestAmount = snapshot?.RemainingOnRequestAmount ?? 0m,
            UrgencyLevel = entity.UrgencyLevel,
            Status = entity.Status,
            Reason = entity.Reason,
            FundingStatusCode = snapshot?.FundingStatusCode ?? AidRequestFundingStatusCodes.NotFunded,
            FundingStatusName = snapshot?.FundingStatusName ?? "غير ممول",
            DisbursementStatusCode = snapshot?.DisbursementStatusCode ?? AidRequestFundingStatusCodes.NotDisbursed,
            DisbursementStatusName = snapshot?.DisbursementStatusName ?? "غير مصروف",
            OperationalStatusCode = operational?.OperationalStatusCode ?? CharityOperationalStatusCodes.Open,
            OperationalStatusName = operational?.OperationalStatusName ?? "مفتوح",
            IsOperationallyClosed = operational?.IsClosed ?? false,
            Lines = await GetAidRequestLineVmsAsync(entity.Id)
        };

        return View(vm);
    }
    private async Task ValidateCommitteeDecisionLimitAsync(Guid beneficiaryId, Guid aidTypeId, decimal? requestedAmount, string modelStateKey, Guid? currentAidRequestId = null)
    {
        if (beneficiaryId == Guid.Empty || aidTypeId == Guid.Empty)
            return;

        var validation = await BeneficiaryCommitteeDecisionGuard.ValidateAidRequestAmountAsync(
            _db,
            beneficiaryId,
            aidTypeId,
            requestedAmount,
            currentAidRequestId);

        if (!validation.IsValid)
            ModelState.AddModelError(modelStateKey, validation.Message);
    }

    private async Task FillLookupsAsync(CreateBeneficiaryAidRequestVm vm)
    {
        var aidTypes = await _lookupRepository.GetAidTypesAsync();
        var beneficiaries = await _beneficiaryRepository.SearchAsync(null, null, true);
        var projects = await _projectRepository.SearchAsync(null, null, true);
        var items = await _itemRepository.GetActiveAsync();
        var warehouses = await _warehouseRepository.GetActiveAsync();

        vm.AidTypes = aidTypes.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.NameAr
        }).ToList();

        vm.Beneficiaries = beneficiaries.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = string.IsNullOrWhiteSpace(x.Code) ? x.FullName : $"{x.Code} - {x.FullName}"
        }).ToList();

        vm.Projects = projects.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.Code} - {x.Name}"
        }).ToList();

        vm.Items = items.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.ItemCode} - {x.ItemNameAr}"
        }).ToList();

        vm.Warehouses = warehouses.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.WarehouseCode} - {x.WarehouseNameAr}"
        }).ToList();

        vm.FulfillmentMethods = BeneficiaryAidRequestFulfillmentMethodOption.Items();
        if (vm.Lines == null || vm.Lines.Count == 0)
            vm.Lines = new List<BeneficiaryAidRequestLineVm> { new() };

        vm.Statuses = BuildStatuses();
    }


    private List<BeneficiaryAidRequestLineVm> NormalizeAidRequestLines(IEnumerable<BeneficiaryAidRequestLineVm>? lines)
    {
        var normalized = new List<BeneficiaryAidRequestLineVm>();
        if (lines == null)
            return normalized;

        foreach (var line in lines)
        {
            if (!HasAidRequestLineData(line))
                continue;

            var requestedQuantity = line.RequestedQuantity;
            var estimatedUnitValue = line.EstimatedUnitValue;
            var estimatedTotalValue = line.EstimatedTotalValue;

            if ((!estimatedTotalValue.HasValue || estimatedTotalValue.Value <= 0m) && estimatedUnitValue.HasValue && estimatedUnitValue.Value > 0m)
            {
                var quantity = requestedQuantity.HasValue && requestedQuantity.Value > 0m ? requestedQuantity.Value : 1m;
                estimatedTotalValue = quantity * estimatedUnitValue.Value;
            }

            normalized.Add(new BeneficiaryAidRequestLineVm
            {
                Id = line.Id,
                ItemId = line.ItemId,
                ItemNameSnapshot = line.ItemNameSnapshot?.Trim(),
                Description = line.Description?.Trim(),
                RequestedQuantity = requestedQuantity,
                ApprovedQuantity = line.ApprovedQuantity,
                EstimatedUnitValue = estimatedUnitValue,
                EstimatedTotalValue = estimatedTotalValue,
                FulfillmentMethod = string.IsNullOrWhiteSpace(line.FulfillmentMethod)
                    ? BeneficiaryAidRequestFulfillmentMethodOption.CashEquivalent
                    : line.FulfillmentMethod.Trim(),
                WarehouseId = line.WarehouseId,
                Status = string.IsNullOrWhiteSpace(line.Status) ? "Pending" : line.Status.Trim(),
                Notes = line.Notes?.Trim()
            });
        }

        return normalized;
    }

    private void ApplyAidRequestLinesTotals(CreateBeneficiaryAidRequestVm vm, List<BeneficiaryAidRequestLineVm> normalizedLines)
    {
        if (normalizedLines.Count == 0)
            return;

        foreach (var line in normalizedLines)
        {
            if (line.ItemId.HasValue && string.IsNullOrWhiteSpace(line.ItemNameSnapshot))
            {
                line.ItemNameSnapshot = vm.Items.FirstOrDefault(x => x.Value == line.ItemId.Value.ToString())?.Text;
            }
        }

        var total = normalizedLines.Sum(x => x.EstimatedTotalValue ?? 0m);
        if (total > 0m)
        {
            vm.RequestedAmount = total;
            ModelState.Remove(nameof(vm.RequestedAmount));
        }

        vm.Lines = normalizedLines;
    }

    private void ValidateAidRequestLines(List<BeneficiaryAidRequestLineVm> normalizedLines)
    {
        for (var i = 0; i < normalizedLines.Count; i++)
        {
            var line = normalizedLines[i];
            var hasDescription = !string.IsNullOrWhiteSpace(line.Description) || !string.IsNullOrWhiteSpace(line.ItemNameSnapshot) || line.ItemId.HasValue;
            if (!hasDescription)
                ModelState.AddModelError($"Lines[{i}].Description", "أدخل وصف البند أو اختر صنفًا مخزنيًا.");

            if ((!line.RequestedQuantity.HasValue || line.RequestedQuantity.Value <= 0m) && (!line.EstimatedTotalValue.HasValue || line.EstimatedTotalValue.Value <= 0m))
                ModelState.AddModelError($"Lines[{i}].RequestedQuantity", "أدخل كمية مطلوبة أو قيمة تقديرية للبند.");

            if (string.Equals(line.FulfillmentMethod, BeneficiaryAidRequestFulfillmentMethodOption.InKindFromStock, StringComparison.OrdinalIgnoreCase))
            {
                if (!line.ItemId.HasValue || line.ItemId.Value == Guid.Empty)
                    ModelState.AddModelError($"Lines[{i}].ItemId", "الصرف العيني من المخزن يحتاج اختيار صنف مخزني.");

                if (!line.WarehouseId.HasValue || line.WarehouseId.Value == Guid.Empty)
                    ModelState.AddModelError($"Lines[{i}].WarehouseId", "الصرف العيني من المخزن يحتاج اختيار مخزن مقترح.");
            }
        }
    }

    private static bool HasAidRequestLineData(BeneficiaryAidRequestLineVm line)
    {
        return line.ItemId.HasValue
            || line.WarehouseId.HasValue
            || !string.IsNullOrWhiteSpace(line.ItemNameSnapshot)
            || !string.IsNullOrWhiteSpace(line.Description)
            || !string.IsNullOrWhiteSpace(line.Notes)
            || (line.RequestedQuantity ?? 0m) > 0m
            || (line.ApprovedQuantity ?? 0m) > 0m
            || (line.EstimatedUnitValue ?? 0m) > 0m
            || (line.EstimatedTotalValue ?? 0m) > 0m;
    }

    private async Task<List<BeneficiaryAidRequestLineVm>> GetAidRequestLineVmsAsync(Guid aidRequestId)
    {
        return await _db.BeneficiaryAidRequestLines
            .AsNoTracking()
            .Where(x => x.AidRequestId == aidRequestId)
            .OrderBy(x => x.CreatedAtUtc)
            .ThenBy(x => x.Id)
            .Select(x => new BeneficiaryAidRequestLineVm
            {
                Id = x.Id,
                ItemId = x.ItemId,
                ItemNameSnapshot = x.ItemNameSnapshot,
                Description = x.Description,
                RequestedQuantity = x.RequestedQuantity,
                ApprovedQuantity = x.ApprovedQuantity,
                EstimatedUnitValue = x.EstimatedUnitValue,
                EstimatedTotalValue = x.EstimatedTotalValue,
                FulfillmentMethod = x.FulfillmentMethod,
                WarehouseId = x.WarehouseId,
                Status = x.Status,
                Notes = x.Notes
            })
            .ToListAsync();
    }

    private async Task SaveAidRequestLinesAsync(Guid aidRequestId, List<BeneficiaryAidRequestLineVm> lines, string? userId, bool replaceExisting = false)
    {
        if (replaceExisting)
        {
            var existingLines = await _db.BeneficiaryAidRequestLines
                .Where(x => x.AidRequestId == aidRequestId)
                .ToListAsync();

            if (existingLines.Count > 0)
                _db.BeneficiaryAidRequestLines.RemoveRange(existingLines);
        }

        foreach (var line in lines)
        {
            _db.BeneficiaryAidRequestLines.Add(new BeneficiaryAidRequestLine
            {
                Id = line.Id.HasValue && line.Id.Value != Guid.Empty ? line.Id.Value : Guid.NewGuid(),
                AidRequestId = aidRequestId,
                ItemId = line.ItemId,
                ItemNameSnapshot = line.ItemNameSnapshot,
                Description = line.Description,
                RequestedQuantity = line.RequestedQuantity,
                ApprovedQuantity = line.ApprovedQuantity,
                EstimatedUnitValue = line.EstimatedUnitValue,
                EstimatedTotalValue = line.EstimatedTotalValue,
                FulfillmentMethod = string.IsNullOrWhiteSpace(line.FulfillmentMethod)
                    ? BeneficiaryAidRequestFulfillmentMethodOption.CashEquivalent
                    : line.FulfillmentMethod,
                WarehouseId = line.WarehouseId,
                Status = string.IsNullOrWhiteSpace(line.Status) ? "Pending" : line.Status,
                Notes = line.Notes,
                CreatedByUserId = userId,
                CreatedAtUtc = DateTime.UtcNow
            });
        }

        await _db.SaveChangesAsync();
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

    private List<SelectListItem> BuildStatuses()
    {
        return new List<SelectListItem>
        {
            new() { Value = "Pending", Text = "قيد المراجعة" },
            new() { Value = "Approved", Text = "معتمد" },
            new() { Value = "Rejected", Text = "مرفوض" },
            new() { Value = "Disbursed", Text = "تم الصرف" }
        };
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
}
