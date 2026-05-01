using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Donors;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Charity.Donors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Skote.Helpers;
[Authorize(Policy = CharityPolicies.DonorsView)]
public class DonorsController : Controller
{
    private readonly IDonorRepository _donorRepository;
    private readonly AppDbContext _db;

    public DonorsController(IDonorRepository donorRepository, AppDbContext db)
    {
        _donorRepository = donorRepository;
        _db = db;
    }

    public async Task<IActionResult> Index(string? q, string? donorType, bool? isActive)
    {
        var donors = await _donorRepository.SearchAsync(q, donorType, isActive);

        var rows = donors.Select(x => new DonorListRowVm
        {
            Id = x.Id,
            Code = x.Code,
            DonorType = x.DonorType,
            FullName = x.FullName,
            ContactPerson = x.ContactPerson,
            PhoneNumber = x.PhoneNumber,
            GovernorateName = x.Governorate?.NameAr,
            IsActive = x.IsActive,
            TotalDonations = x.Donations?.Where(d => d.Amount.HasValue).Sum(d => d.Amount!.Value) ?? 0m
        }).ToList();

        var vm = new DonorListPageVm
        {
            Filter = new DonorListFilterVm
            {
                Q = q,
                DonorType = donorType,
                IsActive = isActive,
                DonorTypes = DonorTypeOption.Values.Select(x => new SelectListItem { Value = x, Text = x }).ToList()
            },
            Rows = rows,
            TotalCount = rows.Count,
            ActiveCount = rows.Count(x => x.IsActive),
            TotalDonations = rows.Sum(x => x.TotalDonations)
        };

        return View(vm);
    }
    [Authorize(Policy = CharityPolicies.DonorsManage)]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new CreateDonorVm
        {
            Code = await GenerateDonorCodeAsync(),
            DonorType = DonorTypeOption.Values.First()
        };

        await FillLookupsAsync(vm);
        return View(vm);
    }
    [Authorize(Policy = CharityPolicies.DonorsManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDonorVm vm)
    {
        await FillLookupsAsync(vm);

        if (!ModelState.IsValid)
            return View(vm);

        if (await _donorRepository.CodeExistsAsync(vm.Code.Trim()))
        {
            ModelState.AddModelError(nameof(vm.Code), "كود المتبرع موجود بالفعل");
            return View(vm);
        }

        if (!string.IsNullOrWhiteSpace(vm.NationalIdOrTaxNo) && await _donorRepository.NationalIdOrTaxNoExistsAsync(vm.NationalIdOrTaxNo.Trim()))
        {
            ModelState.AddModelError(nameof(vm.NationalIdOrTaxNo), "الرقم القومي / الضريبي مسجل بالفعل");
            return View(vm);
        }

        var entity = new Donor
        {
            Id = Guid.NewGuid(),
            Code = vm.Code.Trim(),
            DonorType = vm.DonorType.Trim(),
            FullName = vm.FullName.Trim(),
            ContactPerson = vm.ContactPerson?.Trim(),
            NationalIdOrTaxNo = vm.NationalIdOrTaxNo?.Trim(),
            PhoneNumber = vm.PhoneNumber?.Trim(),
            Email = vm.Email?.Trim(),
            AddressLine = vm.AddressLine?.Trim(),
            GovernorateId = vm.GovernorateId,
            CityId = vm.CityId,
            AreaId = vm.AreaId,
            PreferredCommunicationMethod = vm.PreferredCommunicationMethod,
            Notes = vm.Notes?.Trim(),
            IsActive = vm.IsActive,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _donorRepository.AddAsync(entity);

        TempData["Success"] = "تم إضافة المتبرع بنجاح";
        return RedirectToAction(nameof(Index));
    }
    [Authorize(Policy = CharityPolicies.DonorsManage)]
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _donorRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var vm = new EditDonorVm
        {
            Id = entity.Id,
            Code = entity.Code,
            DonorType = entity.DonorType,
            FullName = entity.FullName,
            ContactPerson = entity.ContactPerson,
            NationalIdOrTaxNo = entity.NationalIdOrTaxNo,
            PhoneNumber = entity.PhoneNumber,
            Email = entity.Email,
            AddressLine = entity.AddressLine,
            GovernorateId = entity.GovernorateId,
            CityId = entity.CityId,
            AreaId = entity.AreaId,
            PreferredCommunicationMethod = entity.PreferredCommunicationMethod,
            Notes = entity.Notes,
            IsActive = entity.IsActive
        };

        await FillLookupsAsync(vm);
        return View(vm);
    }
    [Authorize(Policy = CharityPolicies.DonorsManage)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditDonorVm vm)
    {
        var entity = await _donorRepository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        await FillLookupsAsync(vm);

        if (!ModelState.IsValid)
            return View(vm);

        if (await _donorRepository.CodeExistsAsync(vm.Code.Trim(), vm.Id))
        {
            ModelState.AddModelError(nameof(vm.Code), "كود المتبرع موجود بالفعل");
            return View(vm);
        }

        if (!string.IsNullOrWhiteSpace(vm.NationalIdOrTaxNo) && await _donorRepository.NationalIdOrTaxNoExistsAsync(vm.NationalIdOrTaxNo.Trim(), vm.Id))
        {
            ModelState.AddModelError(nameof(vm.NationalIdOrTaxNo), "الرقم القومي / الضريبي مسجل بالفعل");
            return View(vm);
        }

        entity.Code = vm.Code.Trim();
        entity.DonorType = vm.DonorType.Trim();
        entity.FullName = vm.FullName.Trim();
        entity.ContactPerson = vm.ContactPerson?.Trim();
        entity.NationalIdOrTaxNo = vm.NationalIdOrTaxNo?.Trim();
        entity.PhoneNumber = vm.PhoneNumber?.Trim();
        entity.Email = vm.Email?.Trim();
        entity.AddressLine = vm.AddressLine?.Trim();
        entity.GovernorateId = vm.GovernorateId;
        entity.CityId = vm.CityId;
        entity.AreaId = vm.AreaId;
        entity.PreferredCommunicationMethod = vm.PreferredCommunicationMethod;
        entity.Notes = vm.Notes?.Trim();
        entity.IsActive = vm.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _donorRepository.UpdateAsync(entity);

        TempData["Success"] = "تم تعديل بيانات المتبرع بنجاح";
        return RedirectToAction(nameof(Details), new { id = vm.Id });
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var entity = await _donorRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var recent = entity.Donations
            .OrderByDescending(x => x.DonationDate)
            .ThenByDescending(x => x.CreatedAtUtc)
            .Take(10)
            .Select(x => new DonorDonationSummaryVm
            {
                Id = x.Id,
                DonationNumber = x.DonationNumber,
                DonationDate = x.DonationDate,
                DonationType = x.DonationType,
                Amount = x.Amount,
                ReceiptNumber = x.ReceiptNumber,
                PaymentMethod = x.PaymentMethod?.MethodNameAr
            }).ToList();

        var vm = new DonorDetailsVm
        {
            Id = entity.Id,
            Code = entity.Code,
            DonorType = entity.DonorType,
            FullName = entity.FullName,
            ContactPerson = entity.ContactPerson,
            NationalIdOrTaxNo = entity.NationalIdOrTaxNo,
            PhoneNumber = entity.PhoneNumber,
            Email = entity.Email,
            AddressLine = entity.AddressLine,
            GovernorateName = entity.Governorate?.NameAr,
            CityName = entity.City?.NameAr,
            AreaName = entity.Area?.NameAr,
            PreferredCommunicationMethod = entity.PreferredCommunicationMethod,
            Notes = entity.Notes,
            IsActive = entity.IsActive,
            CreatedAtUtc = entity.CreatedAtUtc,
            DonationsCount = entity.Donations.Count,
            TotalDonationsAmount = entity.Donations.Where(x => x.Amount.HasValue).Sum(x => x.Amount!.Value),
            RecentDonations = recent
        };

        return View(vm);
    }

    private async Task FillLookupsAsync(CreateDonorVm vm)
    {
        var governorates = await _db.Set<Governorate>()
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.NameAr)
            .ToListAsync();

        var cities = await _db.Set<City>()
            .AsNoTracking()
            .Where(x => !vm.GovernorateId.HasValue || x.GovernorateId == vm.GovernorateId)
            .OrderBy(x => x.NameAr)
            .ToListAsync();

        var areas = await _db.Set<Area>()
            .AsNoTracking()
            .Where(x => !vm.CityId.HasValue || x.CityId == vm.CityId)
            .OrderBy(x => x.NameAr)
            .ToListAsync();

        vm.DonorTypes = DonorTypeOption.Values.Select(x => new SelectListItem { Value = x, Text = x }).ToList();
        vm.CommunicationMethods = CommunicationMethodOption.Values.Select(x => new SelectListItem { Value = x, Text = x }).ToList();
        vm.Governorates = governorates.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.NameAr }).ToList();
        vm.Cities = cities.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.NameAr }).ToList();
        vm.Areas = areas.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.NameAr }).ToList();
    }

    private async Task<string> GenerateDonorCodeAsync()
    {
        var latestCode = await _db.Set<Donor>()
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => x.Code)
            .FirstOrDefaultAsync();

        var next = 1;
        if (!string.IsNullOrWhiteSpace(latestCode))
        {
            var digits = new string(latestCode.Where(char.IsDigit).ToArray());
            if (int.TryParse(digits, out var parsed))
                next = parsed + 1;
        }

        return $"DON-{next:00000}";
    }
}
