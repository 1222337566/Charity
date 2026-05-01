using System.Security.Claims;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Skote.ViewModels.BeneficiaryDocuments;

namespace Skote.Controllers;

public class BeneficiaryDocumentsController : Controller
{
    private static readonly string[] AllowedExtensions = [".pdf", ".jpg", ".jpeg", ".png"];
    private readonly IBeneficiaryDocumentRepository _repository;
    private readonly IBeneficiaryRepository _beneficiaryRepository;

    public BeneficiaryDocumentsController(
        IBeneficiaryDocumentRepository repository,
        IBeneficiaryRepository beneficiaryRepository)
    {
        _repository = repository;
        _beneficiaryRepository = beneficiaryRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index(Guid? beneficiaryId)
    {
        var items = beneficiaryId.HasValue
            ? await _repository.GetListByBeneficiaryAsync(beneficiaryId.Value)
            : await _repository.GetListAsync();

        var model = items.Select(x => new BeneficiaryDocumentListItemVm
        {
            Id = x.Id,
            BeneficiaryId = x.BeneficiaryId,
            BeneficiaryCode = x.Beneficiary?.Code ?? string.Empty,
            BeneficiaryName = x.Beneficiary?.FullName ?? string.Empty,
            DocumentType = x.DocumentType,
            OriginalFileName = x.OriginalFileName,
            FileSizeBytes = x.FileSizeBytes,
            CreatedAtUtc = x.CreatedAtUtc
        }).ToList();

        ViewBag.BeneficiaryId = beneficiaryId;

        if (beneficiaryId.HasValue)
        {
            var beneficiary = await _beneficiaryRepository.GetByIdAsync(beneficiaryId.Value);
            ViewBag.BeneficiaryName = beneficiary?.FullName ?? string.Empty;
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid? beneficiaryId)
    {
        var vm = new BeneficiaryDocumentCreateVm();
        await FillBeneficiariesAsync(vm);

        if (beneficiaryId.HasValue)
            vm.BeneficiaryId = beneficiaryId.Value;

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BeneficiaryDocumentCreateVm vm, IFormFile? uploadFile)
    {
        if (!vm.BeneficiaryId.HasValue)
            ModelState.AddModelError(nameof(vm.BeneficiaryId), "يجب اختيار المستفيد.");

        if (uploadFile is null || uploadFile.Length == 0)
            ModelState.AddModelError("uploadFile", "يجب اختيار ملف.");

        if (uploadFile is not null)
        {
            var ext = Path.GetExtension(uploadFile.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                ModelState.AddModelError("uploadFile", "نوع الملف غير مسموح. المسموح: PDF / JPG / JPEG / PNG");
        }

        if (!ModelState.IsValid)
        {
            await FillBeneficiariesAsync(vm);
            return View(vm);
        }

        await using var ms = new MemoryStream();
        await uploadFile!.CopyToAsync(ms);

        var entity = new BeneficiaryDocument
        {
            Id = Guid.NewGuid(),
            BeneficiaryId = vm.BeneficiaryId!.Value,
            DocumentType = vm.DocumentType,
            Notes = vm.Notes,
            OriginalFileName = Path.GetFileName(uploadFile.FileName),
            ContentType = uploadFile.ContentType ?? "application/octet-stream",
            FileExtension = Path.GetExtension(uploadFile.FileName),
            FileSizeBytes = uploadFile.Length,
            FileContent = ms.ToArray(),
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        };

        await _repository.AddAsync(entity);
        return RedirectToAction(nameof(Index), new { beneficiaryId = vm.BeneficiaryId });
    }

    [HttpGet]
    public async Task<IActionResult> Download(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null) return NotFound();

        return File(entity.FileContent, entity.ContentType, entity.OriginalFileName);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, Guid? beneficiaryId)
    {
        await _repository.DeleteAsync(id);
        return RedirectToAction(nameof(Index), new { beneficiaryId });
    }

    private async Task FillBeneficiariesAsync(BeneficiaryDocumentCreateVm vm)
    {
        var beneficiaries = await _beneficiaryRepository.SearchAsync(null, null, true);
        vm.Beneficiaries = beneficiaries
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = string.IsNullOrWhiteSpace(x.Code) ? x.FullName : $"{x.Code} - {x.FullName}",
                Selected = vm.BeneficiaryId.HasValue && x.Id == vm.BeneficiaryId.Value
            })
            .ToList();
    }
}
