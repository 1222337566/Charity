using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Charity.AidCycles;
using InfrastrfuctureManagmentCore.Domains.Charity.AidCycles;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.AidCycles;
using InfrastructureManagmentServices.Charity.AidCycles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Skote.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize(Policy = CharityPolicies.AidDisbursementView)]
public class AidCycleBeneficiariesController : Controller
{
    private readonly IAidCycleRepository _cycleRepository;
    private readonly IAidCycleBeneficiaryRepository _repository;
    private readonly IAidCycleDisbursementService _disbursementService;
    private readonly AppDbContext _db;
    private readonly IUserActivityService _activityService;

    public AidCycleBeneficiariesController(
        IAidCycleRepository cycleRepository,
        IAidCycleBeneficiaryRepository repository,
        IAidCycleDisbursementService disbursementService,
        AppDbContext db,
        IUserActivityService activityService)
    {
        _cycleRepository = cycleRepository;
        _repository = repository;
        _disbursementService = disbursementService;
        _db = db;
        _activityService = activityService;
    }

    [Authorize(Policy = CharityPolicies.AidDisbursementManage)]
    [HttpGet]
    public async Task<IActionResult> Add(Guid aidCycleId)
    {
        var cycle = await PopulateCycleAsync(aidCycleId);
        if (cycle == null)
            return NotFound();

        var items = await _repository.GetEligibleBeneficiariesForAddAsync(aidCycleId);

        var vm = new AddAidCycleBeneficiariesVm
        {
            AidCycleId = aidCycleId,
            AidCycleTitle = $"{cycle.CycleNumber} - {cycle.Title}",
            Items = items
        };

        return View(vm);
    }

    [Authorize(Policy = CharityPolicies.AidDisbursementManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(AddAidCycleBeneficiariesVm vm)
    {
        var cycle = await PopulateCycleAsync(vm.AidCycleId);
        if (cycle == null)
            return NotFound();

        if (vm.SelectedBeneficiaryIds == null || !vm.SelectedBeneficiaryIds.Any())
        {
            ModelState.AddModelError(string.Empty, "اختر مستحقًا واحدًا على الأقل.");
        }

        if (!ModelState.IsValid)
        {
            vm.AidCycleTitle = $"{cycle.CycleNumber} - {cycle.Title}";
            vm.Items = await _repository.GetEligibleBeneficiariesForAddAsync(vm.AidCycleId);
            return View(vm);
        }

        var addedCount = await _repository.AddBeneficiariesAsync(vm.AidCycleId, vm.SelectedBeneficiaryIds, vm.ApprovedAmount, vm.Notes, User);

        if (addedCount == 0)
        {
            TempData["Warning"] = "لم يتم إضافة أي مستحق. تأكد من وجود قرار لجنة معتمد ونوع مساعدة مطابق للدورة.";
        }
        else
        {
            TempData["Success"] = $"تمت إضافة {addedCount} مستحق/مستحقة إلى الدورة.";
            await _cycleRepository.UpdateTotalsAsync(vm.AidCycleId);
        }

        return RedirectToAction(nameof(Index), new { aidCycleId = vm.AidCycleId });
    }

    public async Task<IActionResult> Index(Guid aidCycleId)
    {
        await PopulateCycleAsync(aidCycleId);
        var items = await _repository.GetByCycleIdAsync(aidCycleId);
        var model = items.Select(x => new AidCycleBeneficiaryListItemVm
        {
            Id = x.Id,
            BeneficiaryId = x.BeneficiaryId,
            BeneficiaryCode = x.Beneficiary?.Code ?? string.Empty,
            BeneficiaryName = x.Beneficiary?.FullName ?? string.Empty,
            AidType = x.AidType?.NameAr,
            ApprovedAmount = x.ApprovedAmount ?? x.ScheduledAmount,
            Status = x.Status,
            LastDisbursementDate = x.LastDisbursementDate,
            NextDueDate = x.NextDueDate,
            AddedByUserId = x.CreatedByUserId,
            CreatedAtUtc = x.CreatedAtUtc
        }).ToList();
        return View(model);
    }

    [Authorize(Policy = CharityPolicies.AidDisbursementManage)]
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        await PopulateCycleAsync(entity.AidCycleId);
        var vm = new EditAidCycleBeneficiaryVm
        {
            Id = entity.Id,
            AidCycleId = entity.AidCycleId,
            BeneficiaryId = entity.BeneficiaryId,
            BeneficiaryDisplay = $"{entity.Beneficiary?.Code} - {entity.Beneficiary?.FullName}",
            ScheduledAmount = entity.ScheduledAmount,
            ApprovedAmount = entity.ApprovedAmount,
            Status = entity.Status,
            LastDisbursementDate = entity.LastDisbursementDate,
            NextDueDate = entity.NextDueDate,
            Notes = entity.Notes,
            StopReason = entity.StopReason
        };
        return View(vm);
    }

    [Authorize(Policy = CharityPolicies.AidDisbursementManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditAidCycleBeneficiaryVm vm)
    {
        var entity = await _repository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        await PopulateCycleAsync(entity.AidCycleId);
        if (!ModelState.IsValid)
            return View(vm);

        entity.ScheduledAmount = vm.ScheduledAmount;
        entity.ApprovedAmount = vm.ApprovedAmount;
        entity.Status = vm.Status;
        entity.LastDisbursementDate = vm.LastDisbursementDate;
        entity.NextDueDate = vm.NextDueDate;
        entity.Notes = vm.Notes?.Trim();
        entity.StopReason = vm.StopReason?.Trim();

        await _repository.UpdateAsync(entity);
        await _cycleRepository.UpdateTotalsAsync(entity.AidCycleId);
        TempData["Success"] = "تم تعديل بيانات المستحق داخل الدورة";
        return RedirectToAction(nameof(Index), new { aidCycleId = entity.AidCycleId });
    }

    [Authorize(Policy = CharityPolicies.AidDisbursementManage)]
    [HttpGet]
    public async Task<IActionResult> BatchDisburse(Guid aidCycleId)
    {
        var cycle = await PopulateCycleAsync(aidCycleId);
        if (cycle == null)
            return NotFound();

        var vm = new AidCycleBatchDisbursementVm
        {
            AidCycleId = cycle.Id,
            CycleDisplay = $"{cycle.CycleNumber} - {cycle.Title}",
            DisbursementDate = cycle.PlannedDisbursementDate.Date,
            EligibleLines = await LoadEligibleLinesAsync(aidCycleId)
        };
        vm.SelectedLineIds = vm.EligibleLines.Select(x => x.Id).ToList();
        await FillDisbursementLookupsAsync(vm);
        return View(vm);
    }

    [Authorize(Policy = CharityPolicies.AidDisbursementManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BatchDisburse(AidCycleBatchDisbursementVm vm)
    {
        var cycle = await PopulateCycleAsync(vm.AidCycleId);
        if (cycle == null)
            return NotFound();

        vm.EligibleLines = await LoadEligibleLinesAsync(vm.AidCycleId);
        await FillDisbursementLookupsAsync(vm);

        if (!ModelState.IsValid)
            return View(vm);

        var result = await _disbursementService.DisburseAsync(new AidCycleDisbursementRequest
        {
            AidCycleId = vm.AidCycleId,
            DisbursementDate = vm.DisbursementDate,
            PaymentMethodId = vm.PaymentMethodId,
            FinancialAccountId = vm.FinancialAccountId,
            LineIds = vm.SelectedLineIds,
            Notes = vm.Notes,
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        });

        await _cycleRepository.UpdateTotalsAsync(vm.AidCycleId);

        if (result.DisbursedCount > 0)
        {
            await _activityService.LogBusinessAsync(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                UserActivityBusinessActions.AidCycleBatchDisbursed,
                $"تم تنفيذ صرف جماعي لدورة الصرف {cycle.CycleNumber}",
                entityName: "دورة صرف",
                entityId: vm.AidCycleId.ToString(),
                newValues: new Dictionary<string, string?>
                {
                    ["CycleNumber"] = cycle.CycleNumber,
                    ["DisbursementDate"] = vm.DisbursementDate.ToString("yyyy-MM-dd"),
                    ["DisbursedCount"] = result.DisbursedCount.ToString(),
                    ["TotalAmount"] = result.TotalAmount.ToString("0.##")
                },
                ct: HttpContext.RequestAborted);
        }

        TempData[result.DisbursedCount > 0 ? "Success" : "Warning"] = result.DisbursedCount > 0
            ? $"تم صرف {result.DisbursedCount} حالة بإجمالي {result.TotalAmount:n2}"
            : string.Join(" - ", result.Messages.DefaultIfEmpty("لم يتم تنفيذ أي عملية صرف"));

        return RedirectToAction(nameof(Index), new { aidCycleId = vm.AidCycleId });
    }

    public async Task<IActionResult> DueList(DateTime? dueDate)
    {
        var items = await _repository.GetDueListAsync(dueDate);
        var model = items.Select(x => new AidCycleBeneficiaryListItemVm
        {
            Id = x.Id,
            BeneficiaryId = x.BeneficiaryId,
            BeneficiaryCode = x.Beneficiary?.Code ?? string.Empty,
            BeneficiaryName = x.Beneficiary?.FullName ?? string.Empty,
            AidType = x.AidType?.NameAr,
            ApprovedAmount = x.ApprovedAmount ?? x.ScheduledAmount,
            Status = x.Status,
            LastDisbursementDate = x.LastDisbursementDate,
            NextDueDate = x.NextDueDate,
            AddedByUserId = x.CreatedByUserId,
            CreatedAtUtc = x.CreatedAtUtc
        }).ToList();

        ViewBag.DueDate = (dueDate ?? DateTime.Today).ToString("yyyy-MM-dd");
        return View(model);
    }

    private async Task<AidCycle?> PopulateCycleAsync(Guid aidCycleId)
    {
        if (aidCycleId == Guid.Empty)
            return null;

        var cycle = await _cycleRepository.GetByIdAsync(aidCycleId);
        if (cycle == null)
            return null;

        ViewBag.AidCycleId = cycle.Id;
        ViewBag.AidCycleTitle = cycle.Title;
        ViewBag.AidCycleNumber = cycle.CycleNumber;
        ViewBag.AidCycleStatus = cycle.Status;
        return cycle;
    }

    private async Task<List<AidCycleBeneficiaryListItemVm>> LoadEligibleLinesAsync(Guid aidCycleId)
    {
        var items = await _repository.GetByCycleIdAsync(aidCycleId);
        return items
            .Where(x => x.Status == "Eligible" || x.Status == "Deferred")
            .Select(x => new AidCycleBeneficiaryListItemVm
            {
                Id = x.Id,
                BeneficiaryId = x.BeneficiaryId,
                BeneficiaryCode = x.Beneficiary?.Code ?? string.Empty,
                BeneficiaryName = x.Beneficiary?.FullName ?? string.Empty,
                AidType = x.AidType?.NameAr,
                ApprovedAmount = x.ApprovedAmount ?? x.ScheduledAmount,
                Status = x.Status,
                LastDisbursementDate = x.LastDisbursementDate,
                NextDueDate = x.NextDueDate,
                AddedByUserId = x.CreatedByUserId,
                CreatedAtUtc = x.CreatedAtUtc
            })
            .ToList();
    }

    private async Task FillDisbursementLookupsAsync(AidCycleBatchDisbursementVm vm)
    {
        vm.PaymentMethods = await _db.Set<InfrastrfuctureManagmentCore.Domains.Payments.PaymentMethod>()
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.MethodNameAr)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.MethodNameAr })
            .ToListAsync();

        vm.FinancialAccounts = await _db.Set<InfrastrfuctureManagmentCore.Domains.Financial.FinancialAccount>()
            .AsNoTracking()
            .Where(x => x.IsActive && x.IsPosting)
            .OrderBy(x => x.AccountCode)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.AccountCode + " - " + x.AccountNameAr })
            .ToListAsync();
    }
}
