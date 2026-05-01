using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Charity.Kafala;
using InfrastrfuctureManagmentCore.Domains.Charity.Kafala;
using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastrfuctureManagmentCore.Domains.Payments;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Kafala;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class KafalaPaymentsController : Controller
{
    private readonly IKafalaPaymentRepository _paymentRepository;
    private readonly IKafalaCaseRepository _caseRepository;
    private readonly AppDbContext _db;

    public KafalaPaymentsController(IKafalaPaymentRepository paymentRepository, IKafalaCaseRepository caseRepository, AppDbContext db)
    {
        _paymentRepository = paymentRepository;
        _caseRepository = caseRepository;
        _db = db;
    }

    public async Task<IActionResult> Index(Guid? kafalaCaseId, Guid? sponsorId, Guid? aidCycleId)
    {
        var items = await _paymentRepository.GetAllAsync(kafalaCaseId, sponsorId, aidCycleId);
        var model = items.Select(x => new KafalaPaymentListItemVm
        {
            Id = x.Id,
            PaymentDate = x.PaymentDate,
            SponsorName = x.Sponsor?.FullName ?? string.Empty,
            BeneficiaryName = x.KafalaCase?.Beneficiary?.FullName ?? string.Empty,
            PeriodLabel = x.PeriodLabel,
            Direction = x.Direction,
            Amount = x.Amount,
            PaymentMethodName = x.PaymentMethod?.MethodNameAr,
            Status = x.Status,
            AidCycleId = x.AidCycleId,
            ReferenceNumber = x.ReferenceNumber
        }).ToList();
        ViewBag.KafalaCaseId = kafalaCaseId;
        ViewBag.SponsorId = sponsorId;
        ViewBag.AidCycleId = aidCycleId;
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid? kafalaCaseId)
    {
        var vm = new CreateKafalaPaymentVm { KafalaCaseId = kafalaCaseId ?? Guid.Empty, PeriodLabel = DateTime.Today.ToString("yyyy/MM") };
        await FillLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateKafalaPaymentVm vm)
    {
        await FillLookupsAsync(vm);
        if (!ModelState.IsValid) return View(vm);

        var kafalaCase = await _caseRepository.GetByIdAsync(vm.KafalaCaseId);
        if (kafalaCase == null)
        {
            ModelState.AddModelError(nameof(vm.KafalaCaseId), "ملف الكفالة غير موجود");
            return View(vm);
        }

        await _paymentRepository.AddAsync(new KafalaPayment
        {
            Id = Guid.NewGuid(),
            KafalaCaseId = kafalaCase.Id,
            SponsorId = kafalaCase.SponsorId,
            PaymentDate = vm.PaymentDate,
            Amount = vm.Amount,
            PeriodLabel = vm.PeriodLabel.Trim(),
            Direction = vm.Direction,
            PaymentMethodId = vm.PaymentMethodId,
            FinancialAccountId = vm.FinancialAccountId,
            ReferenceNumber = vm.ReferenceNumber?.Trim(),
            Status = vm.Status,
            Notes = vm.Notes?.Trim(),
            CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            CreatedAtUtc = DateTime.UtcNow
        });

        if (vm.Direction == "Received")
        {
            kafalaCase.LastCollectionDate = vm.PaymentDate;
            kafalaCase.NextDueDate = CalculateNextDueDate(kafalaCase.Frequency, vm.PaymentDate);
        }
        else if (vm.Direction == "Disbursed")
        {
            kafalaCase.LastDisbursementDate = vm.PaymentDate;
        }
        await _caseRepository.UpdateAsync(kafalaCase);

        TempData["Success"] = "تم تسجيل حركة الكفالة بنجاح";
        return RedirectToAction(nameof(Index), new { kafalaCaseId = vm.KafalaCaseId });
    }

    private static DateTime CalculateNextDueDate(string frequency, DateTime baseDate) => frequency switch
    {
        "Quarterly" => baseDate.AddMonths(3),
        "SemiAnnual" => baseDate.AddMonths(6),
        "Annual" => baseDate.AddYears(1),
        _ => baseDate.AddMonths(1)
    };

    private async Task FillLookupsAsync(CreateKafalaPaymentVm vm)
    {
        vm.KafalaCases = (await _caseRepository.GetAllAsync()).Where(x => x.Status == "Active")
            .Select(x => new SelectListItem($"{x.CaseNumber} - {x.Beneficiary?.FullName}", x.Id.ToString())).ToList();
        vm.PaymentMethods = await _db.Set<PaymentMethod>().AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.MethodNameAr)
            .Select(x => new SelectListItem(x.MethodNameAr, x.Id.ToString())).ToListAsync();
        vm.FinancialAccounts = await _db.Set<FinancialAccount>().AsNoTracking().Where(x => x.IsActive && x.IsPosting).OrderBy(x => x.AccountCode)
            .Select(x => new SelectListItem($"{x.AccountCode} - {x.AccountNameAr}", x.Id.ToString())).ToListAsync();
        vm.Directions = new() { new("تحصيل من الكفيل", "Received"), new("صرف للمكفول", "Disbursed"), new("تسوية", "Adjustment") };
        vm.Statuses = new() { new("مؤكد", "Confirmed"), new("مسودة", "Draft"), new("معكوس", "Reversed") };
    }
}
