using InfrastrfuctureManagmentCore.Domains.Charity.Funders;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Funders;
using InfrastructureManagmentWebFramework.Models.Charity.FunderProfile;
using InfrastructureManagmentWebFramework.Models.Charity.FunderProfile.GrantAgreements;
using InfrastructureManagmentWebFramework.Models.Charity.Funders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InfrastructureManagmentDataAccess.EntityFramework;
using Skote.Helpers;
[Authorize(Policy = CharityPolicies.FundersView)]
public class GrantAgreementsController : Controller
{
    private readonly IGrantAgreementRepository _grantAgreementRepository;
    private readonly IFunderRepository _funderRepository;
    private readonly AppDbContext _db;

    public GrantAgreementsController(IGrantAgreementRepository grantAgreementRepository, IFunderRepository funderRepository, AppDbContext db)
    {
        _grantAgreementRepository = grantAgreementRepository;
        _funderRepository = funderRepository;
        _db = db;
    }

    public async Task<IActionResult> Index(Guid? funderId = null)
    {
        List<GrantAgreement> items;

        if (funderId.HasValue && funderId.Value != Guid.Empty)
        {
            if (!await PopulateFunderAsync(funderId.Value))
                return NotFound();

            items = await _grantAgreementRepository.GetByFunderIdAsync(funderId.Value);
        }
        else
        {
            ViewBag.PageSubtitle = "عرض عام لكل اتفاقيات التمويل على مستوى الجمعية";
            items = await _db.CharityGrantAgreements
                .AsNoTracking()
                .Include(x => x.Funder)
                .Include(x => x.Installments)
                .Include(x => x.Conditions)
                .OrderByDescending(x => x.AgreementDate)
                .ThenByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }
        var model = items.Select(x => new GrantAgreementListItemVm
        {
            Id = x.Id,
            AgreementNumber = x.AgreementNumber,
            Title = x.Title,
            AgreementDate = x.AgreementDate,
            StartDate = x.StartDate,
            EndDate = x.EndDate,
            TotalAmount = x.TotalAmount,
            Currency = x.Currency,
            Status = x.Status,
            InstallmentsCount = x.Installments.Count,
            ConditionsCount = x.Conditions.Count,
            ReceivedAmount = x.Installments.Where(i => i.ReceivedAmount.HasValue).Sum(i => i.ReceivedAmount ?? 0),
            FunderName = x.Funder?.Name
        }).ToList();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid funderId)
    {
        if (!await PopulateFunderAsync(funderId))
            return NotFound();

        var vm = new CreateGrantAgreementVm
        {
            FunderId = funderId,
            AgreementDate = DateTime.Today,
            AgreementNumber = await GenerateAgreementNumberAsync(),
            Status = GrantAgreementStatusOption.Values.First()
        };

        FillLookups(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateGrantAgreementVm vm)
    {
        if (!await PopulateFunderAsync(vm.FunderId))
            return NotFound();

        FillLookups(vm);

        if (!ModelState.IsValid)
            return View(vm);

        if (await _grantAgreementRepository.AgreementNumberExistsAsync(vm.AgreementNumber.Trim()))
        {
            ModelState.AddModelError(nameof(vm.AgreementNumber), "رقم الاتفاقية موجود بالفعل");
            return View(vm);
        }

        var entity = new GrantAgreement
        {
            Id = Guid.NewGuid(),
            FunderId = vm.FunderId,
            AgreementNumber = vm.AgreementNumber.Trim(),
            Title = vm.Title.Trim(),
            Description = vm.Description?.Trim(),
            AgreementDate = vm.AgreementDate,
            StartDate = vm.StartDate,
            EndDate = vm.EndDate,
            TotalAmount = vm.TotalAmount,
            Currency = vm.Currency?.Trim() ?? "EGP",
            PaymentTerms = vm.PaymentTerms?.Trim(),
            ReportingRequirements = vm.ReportingRequirements?.Trim(),
            Status = vm.Status,
            Notes = vm.Notes?.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        await _grantAgreementRepository.AddAsync(entity);
        TempData["Success"] = "تم إضافة اتفاقية التمويل بنجاح";
        return RedirectToAction(nameof(Index), new { funderId = vm.FunderId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _grantAgreementRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        if (!await PopulateFunderAsync(entity.FunderId))
            return NotFound();

        var vm = new EditGrantAgreementVm
        {
            Id = entity.Id,
            FunderId = entity.FunderId,
            AgreementNumber = entity.AgreementNumber,
            Title = entity.Title,
            Description = entity.Description,
            AgreementDate = entity.AgreementDate,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            TotalAmount = entity.TotalAmount,
            Currency = entity.Currency,
            PaymentTerms = entity.PaymentTerms,
            ReportingRequirements = entity.ReportingRequirements,
            Status = entity.Status,
            Notes = entity.Notes
        };

        FillLookups(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditGrantAgreementVm vm)
    {
        var entity = await _grantAgreementRepository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        if (!await PopulateFunderAsync(entity.FunderId))
            return NotFound();

        FillLookups(vm);

        if (!ModelState.IsValid)
            return View(vm);

        if (await _grantAgreementRepository.AgreementNumberExistsAsync(vm.AgreementNumber.Trim(), vm.Id))
        {
            ModelState.AddModelError(nameof(vm.AgreementNumber), "رقم الاتفاقية موجود بالفعل");
            return View(vm);
        }

        entity.AgreementNumber = vm.AgreementNumber.Trim();
        entity.Title = vm.Title.Trim();
        entity.Description = vm.Description?.Trim();
        entity.AgreementDate = vm.AgreementDate;
        entity.StartDate = vm.StartDate;
        entity.EndDate = vm.EndDate;
        entity.TotalAmount = vm.TotalAmount;
        entity.Currency = vm.Currency?.Trim() ?? "EGP";
        entity.PaymentTerms = vm.PaymentTerms?.Trim();
        entity.ReportingRequirements = vm.ReportingRequirements?.Trim();
        entity.Status = vm.Status;
        entity.Notes = vm.Notes?.Trim();
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _grantAgreementRepository.UpdateAsync(entity);
        TempData["Success"] = "تم تعديل اتفاقية التمويل بنجاح";
        return RedirectToAction(nameof(Index), new { funderId = entity.FunderId });
    }

    private async Task<bool> PopulateFunderAsync(Guid funderId)
    {
        var funder = await _funderRepository.GetByIdAsync(funderId);
        if (funder == null)
            return false;

        ViewBag.FunderHeader = new FunderHeaderVm
        {
            Id = funder.Id,
            Code = funder.Code,
            Name = funder.Name,
            FunderType = funder.FunderType,
            ContactPerson = funder.ContactPerson,
            PhoneNumber = funder.PhoneNumber,
            IsActive = funder.IsActive
        };

        return true;
    }

    private static void FillLookups(CreateGrantAgreementVm vm)
    {
        vm.Statuses = GrantAgreementStatusOption.Values.Select(x => new SelectListItem { Value = x, Text = x }).ToList();
    }

    private async Task<string> GenerateAgreementNumberAsync()
    {
        var seed = Guid.NewGuid().ToString("N")[..6].ToUpper();
        var candidate = $"GR-{DateTime.Today:yyyyMMdd}-{seed}";
        while (await _grantAgreementRepository.AgreementNumberExistsAsync(candidate))
        {
            seed = Guid.NewGuid().ToString("N")[..6].ToUpper();
            candidate = $"GR-{DateTime.Today:yyyyMMdd}-{seed}";
        }
        return candidate;
    }
}
