using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Charity.AidCycles;
using InfrastrfuctureManagmentCore.Domains.Charity.AidCycles;
using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.AidCycles;
using InfrastructureManagmentServices.Charity.AidCycles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Skote.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize(Policy = CharityPolicies.AidDisbursementView)]
public class AidCyclesController : Controller
{
    private readonly IAidCycleRepository _repository;
    private readonly IAidCycleGenerationService _generationService;
    private readonly AppDbContext _db;
    private readonly IUserActivityService _activityService;

    public AidCyclesController(
        IAidCycleRepository repository,
        IAidCycleGenerationService generationService,
        AppDbContext db,
        IUserActivityService activityService)
    {
        _repository = repository;
        _generationService = generationService;
        _db = db;
        _activityService = activityService;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _repository.GetAllAsync();
        var model = items.Select(x => new AidCycleListItemVm
        {
            Id = x.Id,
            CycleNumber = x.CycleNumber,
            Title = x.Title,
            CycleType = x.CycleType,
            AidType = x.AidType?.NameAr,
            PeriodYear = x.PeriodYear,
            PeriodMonth = x.PeriodMonth,
            PlannedDisbursementDate = x.PlannedDisbursementDate,
            Status = x.Status,
            BeneficiariesCount = x.BeneficiariesCount,
            TotalPlannedAmount = x.TotalPlannedAmount,
            TotalDisbursedAmount = x.TotalDisbursedAmount
        }).ToList();
        return View(model);
    }

    [Authorize(Policy = CharityPolicies.AidDisbursementManage)]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new CreateAidCycleVm();
        await FillLookupsAsync(vm);
        return View(vm);
    }

    [Authorize(Policy = CharityPolicies.AidDisbursementManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateAidCycleVm vm)
    {
        await FillLookupsAsync(vm);
        if (!ModelState.IsValid)
            return View(vm);

        if (await _repository.CycleNumberExistsAsync(vm.CycleNumber))
        {
            ModelState.AddModelError(nameof(vm.CycleNumber), "رقم الدورة موجود بالفعل");
            return View(vm);
        }

        var entity = new AidCycle
        {
            Id = Guid.NewGuid(),
            CycleNumber = vm.CycleNumber.Trim(),
            Title = vm.Title.Trim(),
            CycleType = vm.CycleType,
            AidTypeId = vm.AidTypeId,
            PeriodYear = vm.PeriodYear,
            PeriodMonth = vm.PeriodMonth,
            FromDate = vm.FromDate,
            ToDate = vm.ToDate,
            PlannedDisbursementDate = vm.PlannedDisbursementDate,
            Notes = vm.Notes?.Trim(),
            CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            CreatedAtUtc = DateTime.UtcNow,
            Status = "Draft"
        };

        await _repository.AddAsync(entity);

        await _activityService.LogBusinessAsync(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            UserActivityBusinessActions.AidCycleCreated,
            $"تم إنشاء دورة صرف برقم {entity.CycleNumber}",
            entityName: "دورة صرف",
            entityId: entity.Id.ToString(),
            newValues: new Dictionary<string, string?>
            {
                ["CycleNumber"] = entity.CycleNumber,
                ["Title"] = entity.Title,
                ["CycleType"] = entity.CycleType,
                ["AidTypeId"] = entity.AidTypeId?.ToString(),
                ["PlannedDisbursementDate"] = entity.PlannedDisbursementDate.ToString("yyyy-MM-dd"),
                ["Status"] = entity.Status
            },
            ct: HttpContext.RequestAborted);

        TempData["Success"] = "تم إنشاء دورة الصرف بنجاح";
        return RedirectToAction(nameof(Details), new { id = entity.Id });
    }

    [Authorize(Policy = CharityPolicies.AidDisbursementManage)]
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var vm = new EditAidCycleVm
        {
            Id = entity.Id,
            CycleNumber = entity.CycleNumber,
            Title = entity.Title,
            CycleType = entity.CycleType,
            AidTypeId = entity.AidTypeId,
            PeriodYear = entity.PeriodYear,
            PeriodMonth = entity.PeriodMonth,
            FromDate = entity.FromDate,
            ToDate = entity.ToDate,
            PlannedDisbursementDate = entity.PlannedDisbursementDate,
            Notes = entity.Notes,
            Status = entity.Status
        };

        await FillLookupsAsync(vm);
        return View(vm);
    }

    [Authorize(Policy = CharityPolicies.AidDisbursementManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditAidCycleVm vm)
    {
        var entity = await _repository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        await FillLookupsAsync(vm);
        if (!ModelState.IsValid)
            return View(vm);

        if (await _repository.CycleNumberExistsAsync(vm.CycleNumber, vm.Id))
        {
            ModelState.AddModelError(nameof(vm.CycleNumber), "رقم الدورة موجود بالفعل");
            return View(vm);
        }

        var oldValues = new Dictionary<string, string?>
        {
            ["CycleNumber"] = entity.CycleNumber,
            ["Title"] = entity.Title,
            ["CycleType"] = entity.CycleType,
            ["AidTypeId"] = entity.AidTypeId?.ToString(),
            ["PeriodYear"] = entity.PeriodYear?.ToString(),
            ["PeriodMonth"] = entity.PeriodMonth?.ToString(),
            ["PlannedDisbursementDate"] = entity.PlannedDisbursementDate.ToString("yyyy-MM-dd"),
            ["Status"] = entity.Status
        };

        entity.CycleNumber = vm.CycleNumber.Trim();
        entity.Title = vm.Title.Trim();
        entity.CycleType = vm.CycleType;
        entity.AidTypeId = vm.AidTypeId;
        entity.PeriodYear = vm.PeriodYear;
        entity.PeriodMonth = vm.PeriodMonth;
        entity.FromDate = vm.FromDate;
        entity.ToDate = vm.ToDate;
        entity.PlannedDisbursementDate = vm.PlannedDisbursementDate;
        entity.Notes = vm.Notes?.Trim();

        await _repository.UpdateAsync(entity);

        await _activityService.LogBusinessAsync(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            UserActivityBusinessActions.AidCycleUpdated,
            $"تم تعديل دورة الصرف رقم {entity.CycleNumber}",
            entityName: "دورة صرف",
            entityId: entity.Id.ToString(),
            oldValues: oldValues,
            newValues: new Dictionary<string, string?>
            {
                ["CycleNumber"] = entity.CycleNumber,
                ["Title"] = entity.Title,
                ["CycleType"] = entity.CycleType,
                ["AidTypeId"] = entity.AidTypeId?.ToString(),
                ["PeriodYear"] = entity.PeriodYear?.ToString(),
                ["PeriodMonth"] = entity.PeriodMonth?.ToString(),
                ["PlannedDisbursementDate"] = entity.PlannedDisbursementDate.ToString("yyyy-MM-dd"),
                ["Status"] = entity.Status
            },
            ct: HttpContext.RequestAborted);

        TempData["Success"] = "تم تعديل دورة الصرف بنجاح";
        return RedirectToAction(nameof(Details), new { id = entity.Id });
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var entity = await _repository.GetByIdWithBeneficiariesAsync(id);
        if (entity == null)
            return NotFound();

        return View(entity);
    }

    [Authorize(Policy = CharityPolicies.AidDisbursementManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate(Guid id)
    {
        var cycle = await _repository.GetByIdAsync(id);
        if (cycle == null)
            return NotFound();

        var result = await _generationService.GenerateAsync(id);

        if (result.AddedCount > 0)
        {
            await _activityService.LogBusinessAsync(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                UserActivityBusinessActions.AidCycleGenerated,
                $"تم توليد مستحقين لدورة الصرف رقم {cycle.CycleNumber}",
                entityName: "دورة صرف",
                entityId: cycle.Id.ToString(),
                newValues: new Dictionary<string, string?>
                {
                    ["CycleNumber"] = cycle.CycleNumber,
                    ["AddedCount"] = result.AddedCount.ToString(),
                    ["TotalAmount"] = result.TotalAmount.ToString("0.##")
                },
                ct: HttpContext.RequestAborted);
        }

        TempData[result.AddedCount > 0 ? "Success" : "Warning"] = result.AddedCount > 0
            ? $"تم توليد {result.AddedCount} مستحق/مستحقة بإجمالي {result.TotalAmount:n2}"
            : string.Join(" - ", result.Messages);

        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Policy = CharityPolicies.AidDisbursementManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        entity.Status = "Approved";
        entity.ApprovedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        entity.ApprovedAtUtc = DateTime.UtcNow;
        await _repository.UpdateAsync(entity);

        await _activityService.LogBusinessAsync(
            entity.ApprovedByUserId,
            UserActivityBusinessActions.AidCycleApproved,
            $"تم اعتماد دورة الصرف رقم {entity.CycleNumber}",
            entityName: "دورة صرف",
            entityId: entity.Id.ToString(),
            newValues: new Dictionary<string, string?>
            {
                ["CycleNumber"] = entity.CycleNumber,
                ["Status"] = entity.Status,
                ["ApprovedAtUtc"] = entity.ApprovedAtUtc?.ToString("O")
            },
            ct: HttpContext.RequestAborted);

        TempData["Success"] = "تم اعتماد دورة الصرف";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Policy = CharityPolicies.AidDisbursementManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Close(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        entity.Status = "Closed";
        entity.ClosedAtUtc = DateTime.UtcNow;
        await _repository.UpdateAsync(entity);

        await _activityService.LogBusinessAsync(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            UserActivityBusinessActions.AidCycleClosed,
            $"تم إغلاق دورة الصرف رقم {entity.CycleNumber}",
            entityName: "دورة صرف",
            entityId: entity.Id.ToString(),
            newValues: new Dictionary<string, string?>
            {
                ["CycleNumber"] = entity.CycleNumber,
                ["Status"] = entity.Status,
                ["ClosedAtUtc"] = entity.ClosedAtUtc?.ToString("O")
            },
            ct: HttpContext.RequestAborted);

        TempData["Success"] = "تم إغلاق دورة الصرف";
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task FillLookupsAsync(CreateAidCycleVm vm)
    {
        var aidTypes = await _db.Set<AidTypeLookup>().AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.NameAr)
            .ToListAsync();

        vm.AidTypes = aidTypes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.NameAr }).ToList();
        vm.CycleTypes = BuildCycleTypes();
    }

    private Task FillLookupsAsync(EditAidCycleVm vm)
    {
        return FillLookupsAsync((CreateAidCycleVm)vm);
    }

    private List<SelectListItem> BuildCycleTypes() => new()
    {
        new() { Value = "Monthly", Text = "دورة شهرية" },
        new() { Value = "Seasonal", Text = "دورة موسمية" },
        new() { Value = "Emergency", Text = "حالات عاجلة" },
        new() { Value = "Sponsorship", Text = "كفالات / دوريات" }
    };
}
