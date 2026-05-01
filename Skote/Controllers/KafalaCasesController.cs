using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Charity.Kafala;
using InfrastructureManagmentServices.Charity.Kafala;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Kafala;
using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastrfuctureManagmentCore.Domains.Payments;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Kafala;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InfrastructureManagmentServices.Charity.Workflow;
using System.Security.Claims;

public class KafalaCasesController : Controller
{
    private readonly IKafalaCaseRepository _repository;
    private readonly IKafalaSponsorRepository _sponsorRepository;
    private readonly IKafalaAidCycleBridgeService _bridgeService;
    private readonly IWorkflowService _workflowService;
    private readonly AppDbContext _db;

    public KafalaCasesController(IKafalaCaseRepository repository, IKafalaSponsorRepository sponsorRepository, IKafalaAidCycleBridgeService bridgeService, AppDbContext db,IWorkflowService workflowService)
    {
        _repository = repository;
        _workflowService = workflowService;
        _sponsorRepository = sponsorRepository;
        _bridgeService = bridgeService;
        _db = db;
    }

    public async Task<IActionResult> Index(Guid? sponsorId)
    {
        var items = await _repository.GetAllAsync(sponsorId);
        ViewBag.SponsorId = sponsorId;
        var model = items.Select(x => new KafalaCaseListItemVm
        {
            Id = x.Id,
            CaseNumber = x.CaseNumber,
            SponsorName = x.Sponsor?.FullName ?? string.Empty,
            BeneficiaryName = x.Beneficiary?.FullName ?? string.Empty,
            SponsorshipType = x.SponsorshipType,
            Frequency = x.Frequency,
            MonthlyAmount = x.MonthlyAmount,
            StartDate = x.StartDate,
            NextDueDate = x.NextDueDate,
            LastCollectionDate = x.LastCollectionDate,
            LastDisbursementDate = x.LastDisbursementDate,
            AutoIncludeInAidCycles = x.AutoIncludeInAidCycles,
            Status = x.Status
        }).ToList();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new CreateKafalaCaseVm();
        await FillLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateKafalaCaseVm vm)
    {
        await FillLookupsAsync(vm);
        if (!ModelState.IsValid) return View(vm);
        if (await _repository.CaseNumberExistsAsync(vm.CaseNumber))
        {
            ModelState.AddModelError(nameof(vm.CaseNumber), "رقم الكفالة موجود بالفعل");
            return View(vm);
        }

        var entity =await _repository.AddAsync(new KafalaCase
        {
            Id = Guid.NewGuid(),
            CaseNumber = vm.CaseNumber.Trim(),
            SponsorId = vm.SponsorId,
            BeneficiaryId = vm.BeneficiaryId,
            SponsorshipType = vm.SponsorshipType,
            Frequency = vm.Frequency,
            MonthlyAmount = vm.MonthlyAmount,
            StartDate = vm.StartDate,
            EndDate = vm.EndDate,
            NextDueDate = vm.NextDueDate ?? vm.StartDate,
            PaymentMethodId = vm.PaymentMethodId,
            FinancialAccountId = vm.FinancialAccountId,
            AutoIncludeInAidCycles = vm.AutoIncludeInAidCycles,
            Status = vm.Status,
            Notes = vm.Notes?.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _workflowService.InitiateAsync(
            entityType: "KafalaCase",
            entityId: entity.Id,
            entityTitle: entity.Beneficiary.FullName,        // النص الظاهر في اللوحة
            submittedByUserId: userId
        );
        TempData["Success"] = "تم إنشاء ملف الكفالة";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return NotFound();
        var vm = new EditKafalaCaseVm
        {
            Id = entity.Id,
            CaseNumber = entity.CaseNumber,
            SponsorId = entity.SponsorId,
            BeneficiaryId = entity.BeneficiaryId,
            SponsorshipType = entity.SponsorshipType,
            Frequency = entity.Frequency,
            MonthlyAmount = entity.MonthlyAmount,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            NextDueDate = entity.NextDueDate,
            PaymentMethodId = entity.PaymentMethodId,
            FinancialAccountId = entity.FinancialAccountId,
            AutoIncludeInAidCycles = entity.AutoIncludeInAidCycles,
            Status = entity.Status,
            Notes = entity.Notes
        };
        await FillLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditKafalaCaseVm vm)
    {
        await FillLookupsAsync(vm);
        if (!ModelState.IsValid) return View(vm);
        var entity = await _repository.GetByIdAsync(vm.Id);
        if (entity == null) return NotFound();
        if (await _repository.CaseNumberExistsAsync(vm.CaseNumber, vm.Id))
        {
            ModelState.AddModelError(nameof(vm.CaseNumber), "رقم الكفالة موجود بالفعل");
            return View(vm);
        }
        entity.CaseNumber = vm.CaseNumber.Trim();
        entity.SponsorId = vm.SponsorId;
        entity.BeneficiaryId = vm.BeneficiaryId;
        entity.SponsorshipType = vm.SponsorshipType;
        entity.Frequency = vm.Frequency;
        entity.MonthlyAmount = vm.MonthlyAmount;
        entity.StartDate = vm.StartDate;
        entity.EndDate = vm.EndDate;
        entity.NextDueDate = vm.NextDueDate;
        entity.PaymentMethodId = vm.PaymentMethodId;
        entity.FinancialAccountId = vm.FinancialAccountId;
        entity.AutoIncludeInAidCycles = vm.AutoIncludeInAidCycles;
        entity.Status = vm.Status;
        entity.Notes = vm.Notes?.Trim();
        entity.UpdatedAtUtc = DateTime.UtcNow;
        await _repository.UpdateAsync(entity);
        TempData["Success"] = "تم تعديل ملف الكفالة";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateCycle(Guid? sponsorId, DateTime? plannedDate)
    {
        var date = plannedDate?.Date ?? DateTime.Today;
        var result = await _bridgeService.CreateDueSponsorshipCycleAsync(date, sponsorId);
        TempData[result.aidCycleId.HasValue ? "Success" : "Warning"] = string.Join(" - ", result.messages);
        if (result.aidCycleId.HasValue)
            return RedirectToAction("Details", "AidCycles", new { id = result.aidCycleId.Value });
        return RedirectToAction(nameof(Index), new { sponsorId });
    }

    private async Task FillLookupsAsync(CreateKafalaCaseVm vm)
    {
        var sponsors = await _sponsorRepository.GetAllAsync();
        vm.Sponsors = sponsors.Where(x => x.IsActive).Select(x => new SelectListItem(x.FullName, x.Id.ToString())).ToList();
        vm.Beneficiaries = await _db.Set<Beneficiary>().AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.FullName)
            .Select(x => new SelectListItem(x.FullName, x.Id.ToString())).ToListAsync();
        vm.PaymentMethods = await _db.Set<PaymentMethod>().AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.MethodNameAr)
            .Select(x => new SelectListItem(x.MethodNameAr, x.Id.ToString())).ToListAsync();
        vm.FinancialAccounts = await _db.Set<FinancialAccount>().AsNoTracking().Where(x => x.IsActive && x.IsPosting).OrderBy(x => x.AccountCode)
            .Select(x => new SelectListItem($"{x.AccountCode} - {x.AccountNameAr}", x.Id.ToString())).ToListAsync();
        vm.SponsorshipTypes = new() {
            new("كفالة يتيم", "Orphan"), new("كفالة شهرية", "Monthly"), new("كفالة صحية", "Health"), new("كفالة تعليم", "Education"), new("كفالة أسرة", "Family") };
        vm.Frequencies = new() { new("شهري", "Monthly"), new("ربع سنوي", "Quarterly"), new("نصف سنوي", "SemiAnnual"), new("سنوي", "Annual") };
        vm.Statuses = new() { new("مسودة", "Draft"), new("نشطة", "Active"), new("موقوفة", "Suspended"), new("مغلقة", "Closed") };
    }
}
