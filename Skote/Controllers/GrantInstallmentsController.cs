using InfrastrfuctureManagmentCore.Domains.Charity.Funders;
using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastrfuctureManagmentCore.Domains.Payments;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Funders;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Charity.FunderProfile;
using InfrastructureManagmentWebFramework.Models.Charity.FunderProfile.GrantInstallments;
using InfrastructureManagmentWebFramework.Models.Charity.Funders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Skote.Helpers;

[Authorize(Policy = CharityPolicies.FundersView)]
public class GrantInstallmentsController : Controller
{
    private readonly IGrantInstallmentRepository _grantInstallmentRepository;
    private readonly IGrantAgreementRepository _grantAgreementRepository;
    private readonly IOperationalJournalHookService _operationalJournalHookService;
    private readonly AppDbContext _db;

    public GrantInstallmentsController(
        IGrantInstallmentRepository grantInstallmentRepository,
        IGrantAgreementRepository grantAgreementRepository,
        IOperationalJournalHookService operationalJournalHookService,
        AppDbContext db)
    {
        _grantInstallmentRepository = grantInstallmentRepository;
        _grantAgreementRepository = grantAgreementRepository;
        _operationalJournalHookService = operationalJournalHookService;
        _db = db;
    }

    public async Task<IActionResult> Index(Guid? grantAgreementId = null)
    {
        List<GrantInstallment> items;

        if (grantAgreementId.HasValue && grantAgreementId.Value != Guid.Empty)
        {
            if (!await PopulateGrantAgreementAsync(grantAgreementId.Value))
                return NotFound();

            items = await _grantInstallmentRepository.GetByGrantAgreementIdAsync(grantAgreementId.Value);
        }
        else
        {
            ViewBag.PageSubtitle = "عرض عام لكل دفعات التمويل والاستلامات المرتبطة بالاتفاقيات";
            items = await _db.CharityGrantInstallments
                .AsNoTracking()
                .Include(x => x.GrantAgreement)
                    .ThenInclude(x => x.Funder)
                .Include(x => x.PaymentMethod)
                .Include(x => x.FinancialAccount)
                .OrderByDescending(x => x.DueDate)
                .ThenByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }
        var model = items.Select(x => new GrantInstallmentListItemVm
        {
            Id = x.Id,
            InstallmentNumber = x.InstallmentNumber,
            DueDate = x.DueDate,
            Amount = x.Amount,
            ReceivedAmount = x.ReceivedAmount,
            ReceivedDate = x.ReceivedDate,
            Status = x.Status,
            PaymentMethod = x.PaymentMethod?.MethodNameAr,
            FinancialAccount = x.FinancialAccount?.AccountNameAr,
            ReferenceNumber = x.ReferenceNumber,
            Notes = x.Notes,
            AgreementTitle = x.GrantAgreement?.Title,
            FunderName = x.GrantAgreement?.Funder?.Name
        }).ToList();

        return View(model);
    }

    [Authorize(Policy = CharityPolicies.FundersManage)]
    [HttpGet]
    public async Task<IActionResult> Create(Guid grantAgreementId)
    {
        if (!await PopulateGrantAgreementAsync(grantAgreementId))
            return NotFound();

        var installments = await _grantInstallmentRepository.GetByGrantAgreementIdAsync(grantAgreementId);
        var nextInstallmentNumber = installments.Any() ? installments.Max(x => x.InstallmentNumber) + 1 : 1;

        var vm = new CreateGrantInstallmentVm
        {
            GrantAgreementId = grantAgreementId,
            InstallmentNumber = nextInstallmentNumber,
            DueDate = DateTime.Today,
            Status = GrantInstallmentStatusOption.Values.First()
        };

        await FillLookupsAsync(vm);
        return View(vm);
    }

    [Authorize(Policy = CharityPolicies.FundersManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateGrantInstallmentVm vm)
    {
        if (!await PopulateGrantAgreementAsync(vm.GrantAgreementId))
            return NotFound();

        await FillLookupsAsync(vm);
        ValidateReceivedFields(vm);

        if (!ModelState.IsValid)
            return View(vm);

        if (await _grantInstallmentRepository.InstallmentNumberExistsAsync(vm.GrantAgreementId, vm.InstallmentNumber))
        {
            ModelState.AddModelError(nameof(vm.InstallmentNumber), "رقم الدفعة موجود بالفعل داخل نفس الاتفاقية");
            return View(vm);
        }

        var entity = new GrantInstallment
        {
            Id = Guid.NewGuid(),
            GrantAgreementId = vm.GrantAgreementId,
            InstallmentNumber = vm.InstallmentNumber,
            DueDate = vm.DueDate,
            Amount = vm.Amount,
            ReceivedAmount = vm.ReceivedAmount,
            ReceivedDate = vm.ReceivedDate,
            PaymentMethodId = vm.PaymentMethodId,
            FinancialAccountId = vm.FinancialAccountId,
            Status = vm.Status,
            ReferenceNumber = vm.ReferenceNumber?.Trim(),
            Notes = vm.Notes?.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        await _grantInstallmentRepository.AddAsync(entity);

        await TryCreateJournalAfterSaveAsync(entity.Id, entity.ReceivedAmount, entity.ReceivedDate, "تم حفظ دفعة التمويل بنجاح");
        if (!TempData.ContainsKey("Success") && !TempData.ContainsKey("Warning"))
            TempData["Success"] = "تم حفظ دفعة التمويل بنجاح";

        return RedirectToAction(nameof(Index), new { grantAgreementId = vm.GrantAgreementId });
    }

    [Authorize(Policy = CharityPolicies.FundersManage)]
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _grantInstallmentRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        if (!await PopulateGrantAgreementAsync(entity.GrantAgreementId))
            return NotFound();

        var vm = new EditGrantInstallmentVm
        {
            Id = entity.Id,
            GrantAgreementId = entity.GrantAgreementId,
            InstallmentNumber = entity.InstallmentNumber,
            DueDate = entity.DueDate,
            Amount = entity.Amount,
            ReceivedAmount = entity.ReceivedAmount,
            ReceivedDate = entity.ReceivedDate,
            PaymentMethodId = entity.PaymentMethodId,
            FinancialAccountId = entity.FinancialAccountId,
            Status = entity.Status,
            ReferenceNumber = entity.ReferenceNumber,
            Notes = entity.Notes
        };

        await FillLookupsAsync(vm);
        return View(vm);
    }

    [Authorize(Policy = CharityPolicies.FundersManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditGrantInstallmentVm vm)
    {
        var entity = await _grantInstallmentRepository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        if (!await PopulateGrantAgreementAsync(entity.GrantAgreementId))
            return NotFound();

        await FillLookupsAsync(vm);
        ValidateReceivedFields(vm);

        if (!ModelState.IsValid)
            return View(vm);

        if (await _grantInstallmentRepository.InstallmentNumberExistsAsync(entity.GrantAgreementId, vm.InstallmentNumber, vm.Id))
        {
            ModelState.AddModelError(nameof(vm.InstallmentNumber), "رقم الدفعة موجود بالفعل داخل نفس الاتفاقية");
            return View(vm);
        }

        entity.InstallmentNumber = vm.InstallmentNumber;
        entity.DueDate = vm.DueDate;
        entity.Amount = vm.Amount;
        entity.ReceivedAmount = vm.ReceivedAmount;
        entity.ReceivedDate = vm.ReceivedDate;
        entity.PaymentMethodId = vm.PaymentMethodId;
        entity.FinancialAccountId = vm.FinancialAccountId;
        entity.Status = vm.Status;
        entity.ReferenceNumber = vm.ReferenceNumber?.Trim();
        entity.Notes = vm.Notes?.Trim();
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _grantInstallmentRepository.UpdateAsync(entity);

        await TryCreateJournalAfterSaveAsync(entity.Id, entity.ReceivedAmount, entity.ReceivedDate, "تم تعديل دفعة التمويل بنجاح");
        if (!TempData.ContainsKey("Success") && !TempData.ContainsKey("Warning"))
            TempData["Success"] = "تم تعديل دفعة التمويل بنجاح";

        return RedirectToAction(nameof(Index), new { grantAgreementId = entity.GrantAgreementId });
    }

    private async Task FillLookupsAsync(CreateGrantInstallmentVm vm)
    {
        var paymentMethods = await _db.Set<PaymentMethod>()
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.MethodNameAr)
            .ToListAsync();

        var accounts = await _db.Set<FinancialAccount>()
            .AsNoTracking()
            .Where(x => x.IsActive && x.IsPosting)
            .OrderBy(x => x.AccountCode)
            .ThenBy(x => x.AccountNameAr)
            .ToListAsync();

        vm.Statuses = GrantInstallmentStatusOption.Values.Select(x => new SelectListItem { Value = x, Text = x }).ToList();
        vm.PaymentMethods = paymentMethods.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.MethodNameAr }).ToList();
        vm.FinancialAccounts = accounts.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = $"{x.AccountCode} - {x.AccountNameAr}" }).ToList();
    }

    private void ValidateReceivedFields(CreateGrantInstallmentVm vm)
    {
        if (vm.ReceivedAmount.HasValue && vm.ReceivedAmount.Value < 0)
            ModelState.AddModelError(nameof(vm.ReceivedAmount), "قيمة الاستلام لا يمكن أن تكون سالبة");

        if (vm.ReceivedAmount.HasValue && vm.ReceivedAmount.Value > 0 && !vm.ReceivedDate.HasValue)
            ModelState.AddModelError(nameof(vm.ReceivedDate), "يجب تحديد تاريخ الاستلام عند إدخال قيمة مستلمة");
    }

    private async Task<bool> PopulateGrantAgreementAsync(Guid grantAgreementId)
    {
        var agreement = await _grantAgreementRepository.GetByIdAsync(grantAgreementId);
        if (agreement == null)
            return false;

        ViewBag.GrantAgreementHeader = new GrantAgreementHeaderVm
        {
            Id = agreement.Id,
            FunderId = agreement.FunderId,
            AgreementNumber = agreement.AgreementNumber,
            Title = agreement.Title,
            FunderName = agreement.Funder?.Name ?? string.Empty,
            Status = agreement.Status,
            TotalAmount = agreement.TotalAmount,
            Currency = agreement.Currency
        };

        return true;
    }

    private async Task TryCreateJournalAfterSaveAsync(Guid grantInstallmentId, decimal? receivedAmount, DateTime? receivedDate, string baseSuccessMessage)
    {
        if (!receivedAmount.HasValue || receivedAmount.Value <= 0 || !receivedDate.HasValue)
            return;

        var hook = await _operationalJournalHookService.TryCreateGrantInstallmentEntryAsync(grantInstallmentId);
        TempData[hook.IsSuccess ? "Success" : "Warning"] = hook.IsSuccess
            ? $"{baseSuccessMessage}. {hook.Message}"
            : hook.Message;
    }
}
