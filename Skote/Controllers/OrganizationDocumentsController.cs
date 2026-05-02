using InfrastrfuctureManagmentCore.Domains.OrganizationDocuments;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.OrganizationDocuments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Skote.Controllers
{
    [Authorize]
    public class OrganizationDocumentsController : Controller
    {
        private readonly AppDbContext _db;

        public OrganizationDocumentsController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(
            string? documentType,
            string? searchText,
            DateTime? fromDate,
            DateTime? toDate,
            bool includeArchived = false,
            CancellationToken ct = default)
        {
            var query = _db.Set<OrganizationDocument>()
                .AsNoTracking()
                .AsQueryable();

            if (!includeArchived)
                query = query.Where(x => !x.IsArchived);

            if (!string.IsNullOrWhiteSpace(documentType))
                query = query.Where(x => x.DocumentType == documentType);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var term = searchText.Trim();
                query = query.Where(x =>
                    x.DocumentNumber.Contains(term) ||
                    x.Title.Contains(term) ||
                    x.FileName.Contains(term) ||
                    (x.RelatedEntityName != null && x.RelatedEntityName.Contains(term)) ||
                    (x.Notes != null && x.Notes.Contains(term)));
            }

            if (fromDate.HasValue)
                query = query.Where(x => x.DocumentDateUtc >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(x => x.DocumentDateUtc <= toDate.Value);

            var docs = await query
                .OrderByDescending(x => x.DocumentDateUtc)
                .ThenByDescending(x => x.CreatedAtUtc)
                .ToListAsync(ct);

            var vm = new OrganizationDocumentIndexVm
            {
                DocumentType = documentType,
                SearchText = searchText,
                FromDate = fromDate,
                ToDate = toDate,
                IncludeArchived = includeArchived,
                DocumentTypes = OrganizationDocumentTypeOptions.GetDocumentTypes(documentType),
                Rows = docs.Select(x => new OrganizationDocumentRowVm
                {
                    Id = x.Id,
                    DocumentNumber = x.DocumentNumber,
                    Title = x.Title,
                    DocumentType = x.DocumentType,
                    DocumentTypeAr = OrganizationDocumentTypeOptions.ToArabic(x.DocumentType),
                    DocumentDateUtc = x.DocumentDateUtc,
                    RelatedEntityType = x.RelatedEntityType,
                    RelatedEntityName = x.RelatedEntityName,
                    FileName = x.FileName,
                    FileSize = x.FileSize,
                    IsArchived = x.IsArchived,
                    CreatedAtUtc = x.CreatedAtUtc,
                    Notes = x.Notes
                }).ToList()
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var vm = new CreateOrganizationDocumentVm
            {
                DocumentNumber = $"DOC-{DateTime.Now:yyyyMMddHHmm}",
                DocumentDateUtc = DateTime.UtcNow,
                DocumentTypes = OrganizationDocumentTypeOptions.GetDocumentTypes(),
                RelatedEntityTypes = OrganizationDocumentTypeOptions.GetRelatedEntityTypes()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(30_000_000)]
        public async Task<IActionResult> Create(CreateOrganizationDocumentVm vm, CancellationToken ct)
        {
            vm.DocumentTypes = OrganizationDocumentTypeOptions.GetDocumentTypes(vm.DocumentType);
            vm.RelatedEntityTypes = OrganizationDocumentTypeOptions.GetRelatedEntityTypes(vm.RelatedEntityType);

            if (vm.File == null || vm.File.Length == 0)
                ModelState.AddModelError(nameof(vm.File), "برجاء اختيار ملف المستند");

            if (!ModelState.IsValid)
                return View(vm);

            await using var stream = new MemoryStream();
            await vm.File!.CopyToAsync(stream, ct);

            var document = new OrganizationDocument
            {
                Id = Guid.NewGuid(),
                DocumentNumber = vm.DocumentNumber,
                Title = vm.Title,
                DocumentType = vm.DocumentType,
                DocumentDateUtc = vm.DocumentDateUtc,
                RelatedEntityType = vm.RelatedEntityType,
                RelatedEntityId = vm.RelatedEntityId,
                RelatedEntityName = vm.RelatedEntityName,
                FileName = vm.File.FileName,
                ContentType = string.IsNullOrWhiteSpace(vm.File.ContentType)
                    ? "application/octet-stream"
                    : vm.File.ContentType,
                FileContent = stream.ToArray(),
                FileSize = vm.File.Length,
                Notes = vm.Notes,
                IsArchived = false,
                CreatedAtUtc = DateTime.UtcNow,
                CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            _db.Set<OrganizationDocument>().Add(document);
            await _db.SaveChangesAsync(ct);

            TempData["Success"] = "تم حفظ المستند بنجاح.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Download(Guid id, CancellationToken ct)
        {
            var doc = await _db.Set<OrganizationDocument>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (doc == null)
                return NotFound();

            return File(doc.FileContent, doc.ContentType, doc.FileName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Archive(Guid id, CancellationToken ct)
        {
            var doc = await _db.Set<OrganizationDocument>()
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (doc == null)
                return NotFound();

            doc.IsArchived = true;
            await _db.SaveChangesAsync(ct);

            TempData["Success"] = "تم أرشفة المستند.";
            return RedirectToAction(nameof(Index));
        }
    }
}
