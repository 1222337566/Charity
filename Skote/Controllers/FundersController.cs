using InfrastrfuctureManagmentCore.Domains.Charity.Funders;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Funders;
using InfrastructureManagmentWebFramework.Models.Charity.Funders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Skote.Helpers;
[Authorize(Policy = CharityPolicies.FundersView)]
public class FundersController : Controller
{
    private readonly IFunderRepository _funderRepository;

    public FundersController(IFunderRepository funderRepository)
    {
        _funderRepository = funderRepository;
    }

    public async Task<IActionResult> Index(string? q, string? funderType, bool? isActive)
    {
        var funders = await _funderRepository.SearchAsync(q, funderType, isActive);

        var rows = funders.Select(x => new FunderListRowVm
        {
            Id = x.Id,
            Code = x.Code,
            Name = x.Name,
            FunderType = x.FunderType,
            ContactPerson = x.ContactPerson,
            PhoneNumber = x.PhoneNumber,
            IsActive = x.IsActive,
            AgreementsCount = x.GrantAgreements.Count,
            TotalAmount = x.GrantAgreements.Sum(g => g.TotalAmount)
        }).ToList();

        var vm = new FunderListPageVm
        {
            Filter = new FunderListFilterVm
            {
                Q = q,
                FunderType = funderType,
                IsActive = isActive,
                FunderTypes = FunderTypeOption.Values.Select(x => new SelectListItem { Value = x, Text = x }).ToList()
            },
            Rows = rows,
            TotalCount = rows.Count,
            ActiveCount = rows.Count(x => x.IsActive),
            TotalFunding = rows.Sum(x => x.TotalAmount)
        };

        return View(vm);
    }
    [Authorize(Policy = CharityPolicies.FundersManage)]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new CreateFunderVm
        {
            Code = await GenerateFunderCodeAsync(),
            FunderType = FunderTypeOption.Values.First(),
            IsActive = true
        };

        FillLookups(vm);
        return View(vm);
    }
    [Authorize(Policy = CharityPolicies.FundersManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateFunderVm vm)
    {
        FillLookups(vm);

        if (!ModelState.IsValid)
            return View(vm);

        if (await _funderRepository.CodeExistsAsync(vm.Code.Trim()))
        {
            ModelState.AddModelError(nameof(vm.Code), "كود الممول موجود بالفعل");
            return View(vm);
        }

        var entity = new Funder
        {
            Id = Guid.NewGuid(),
            Code = vm.Code.Trim(),
            Name = vm.Name.Trim(),
            FunderType = vm.FunderType.Trim(),
            ContactPerson = vm.ContactPerson?.Trim(),
            PhoneNumber = vm.PhoneNumber?.Trim(),
            Email = vm.Email?.Trim(),
            AddressLine = vm.AddressLine?.Trim(),
            Notes = vm.Notes?.Trim(),
            IsActive = vm.IsActive,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _funderRepository.AddAsync(entity);
        TempData["Success"] = "تم إضافة الممول بنجاح";
        return RedirectToAction(nameof(Index));
    }
    [Authorize(Policy = CharityPolicies.FundersManage)]
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _funderRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var vm = new EditFunderVm
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            FunderType = entity.FunderType,
            ContactPerson = entity.ContactPerson,
            PhoneNumber = entity.PhoneNumber,
            Email = entity.Email,
            AddressLine = entity.AddressLine,
            Notes = entity.Notes,
            IsActive = entity.IsActive
        };

        FillLookups(vm);
        return View(vm);
    }
    [Authorize(Policy = CharityPolicies.FundersManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditFunderVm vm)
    {
        var entity = await _funderRepository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        FillLookups(vm);

        if (!ModelState.IsValid)
            return View(vm);

        if (await _funderRepository.CodeExistsAsync(vm.Code.Trim(), vm.Id))
        {
            ModelState.AddModelError(nameof(vm.Code), "كود الممول موجود بالفعل");
            return View(vm);
        }

        entity.Code = vm.Code.Trim();
        entity.Name = vm.Name.Trim();
        entity.FunderType = vm.FunderType.Trim();
        entity.ContactPerson = vm.ContactPerson?.Trim();
        entity.PhoneNumber = vm.PhoneNumber?.Trim();
        entity.Email = vm.Email?.Trim();
        entity.AddressLine = vm.AddressLine?.Trim();
        entity.Notes = vm.Notes?.Trim();
        entity.IsActive = vm.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _funderRepository.UpdateAsync(entity);
        TempData["Success"] = "تم تعديل بيانات الممول بنجاح";
        return RedirectToAction(nameof(Details), new { id = vm.Id });
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var entity = await _funderRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var recent = entity.GrantAgreements
            .OrderByDescending(x => x.AgreementDate)
            .ThenByDescending(x => x.CreatedAtUtc)
            .Take(10)
            .Select(x => new FunderAgreementSummaryVm
            {
                Id = x.Id,
                AgreementNumber = x.AgreementNumber,
                Title = x.Title,
                AgreementDate = x.AgreementDate,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                TotalAmount = x.TotalAmount,
                Currency = x.Currency,
                Status = x.Status
            }).ToList();

        var vm = new FunderDetailsVm
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            FunderType = entity.FunderType,
            ContactPerson = entity.ContactPerson,
            PhoneNumber = entity.PhoneNumber,
            Email = entity.Email,
            AddressLine = entity.AddressLine,
            Notes = entity.Notes,
            IsActive = entity.IsActive,
            CreatedAtUtc = entity.CreatedAtUtc,
            AgreementsCount = entity.GrantAgreements.Count,
            TotalFunding = entity.GrantAgreements.Sum(x => x.TotalAmount),
            RecentAgreements = recent
        };

        return View(vm);
    }

    private static void FillLookups(CreateFunderVm vm)
    {
        vm.FunderTypes = FunderTypeOption.Values.Select(x => new SelectListItem { Value = x, Text = x }).ToList();
    }

    private async Task<string> GenerateFunderCodeAsync()
    {
        var items = await _funderRepository.SearchAsync(null, null, null);
        var latestCode = items.OrderByDescending(x => x.CreatedAtUtc).Select(x => x.Code).FirstOrDefault();
        var next = 1;
        if (!string.IsNullOrWhiteSpace(latestCode))
        {
            var digits = new string(latestCode.Where(char.IsDigit).ToArray());
            if (int.TryParse(digits, out var parsed))
                next = parsed + 1;
        }
        return $"FUND-{next:00000}";
    }
}
