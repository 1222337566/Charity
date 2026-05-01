using InfrastrfuctureManagmentCore.Domains.Company;
using InfrastrfuctureManagmentCore.Persistence.Repositories.company;
using InfrastructureManagmentWebFramework.Models.Company;
using Microsoft.AspNetCore.Mvc;

public class CompanyProfileController : Controller
{
    private readonly ICompanyProfileRepository _companyProfileRepository;

    public CompanyProfileController(ICompanyProfileRepository companyProfileRepository)
    {
        _companyProfileRepository = companyProfileRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var entity = await _companyProfileRepository.GetActiveAsync();

        if (entity == null)
        {
            return View(new CompanyProfileVm());
        }

        var vm = new CompanyProfileVm
        {
            Id = entity.Id,
            CompanyNameAr = entity.CompanyNameAr,
            CompanyNameEn = entity.CompanyNameEn,
            Phone = entity.Phone,
            Address = entity.Address,
            TaxNumber = entity.TaxNumber,
            ReceiptHeaderText = entity.ReceiptHeaderText,
            ReceiptFooterText = entity.ReceiptFooterText
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CompanyProfileVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var current = await _companyProfileRepository.GetActiveAsync();

        if (current == null)
        {
            var entity = new CompanyProfile
            {
                Id = Guid.NewGuid(),
                CompanyNameAr = vm.CompanyNameAr.Trim(),
                CompanyNameEn = vm.CompanyNameEn?.Trim(),
                Phone = vm.Phone?.Trim(),
                Address = vm.Address?.Trim(),
                TaxNumber = vm.TaxNumber?.Trim(),
                ReceiptHeaderText = vm.ReceiptHeaderText?.Trim(),
                ReceiptFooterText = vm.ReceiptFooterText?.Trim(),
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _companyProfileRepository.AddAsync(entity);
        }
        else
        {
            current.CompanyNameAr = vm.CompanyNameAr.Trim();
            current.CompanyNameEn = vm.CompanyNameEn?.Trim();
            current.Phone = vm.Phone?.Trim();
            current.Address = vm.Address?.Trim();
            current.TaxNumber = vm.TaxNumber?.Trim();
            current.ReceiptHeaderText = vm.ReceiptHeaderText?.Trim();
            current.ReceiptFooterText = vm.ReceiptFooterText?.Trim();
            current.UpdatedAtUtc = DateTime.UtcNow;

            await _companyProfileRepository.UpdateAsync(current);
        }

        TempData["Success"] = "تم حفظ بيانات الشركة بنجاح";
        return RedirectToAction(nameof(Index));
    }
}