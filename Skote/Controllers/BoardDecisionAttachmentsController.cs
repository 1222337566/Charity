using InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.MinutesAndDecisions;
using Microsoft.AspNetCore.Mvc;
using Skote.Models.Charity.MinutesAndDecisions;
using System.Security.Claims;

namespace Skote.Controllers
{
    public class BoardDecisionAttachmentsController : Controller
    {
        private readonly IBoardDecisionAttachmentRepository _attachmentRepository;
        private readonly IBoardDecisionRepository _decisionRepository;

        private static readonly string[] AllowedExtensions =
        {
            ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".jpeg", ".png"
        };

        private const long MaxFileSizeBytes = 10 * 1024 * 1024;

        public BoardDecisionAttachmentsController(
            IBoardDecisionAttachmentRepository attachmentRepository,
            IBoardDecisionRepository decisionRepository)
        {
            _attachmentRepository = attachmentRepository;
            _decisionRepository = decisionRepository;
        }

        public async Task<IActionResult> Index(Guid boardDecisionId)
        {
            var decision = await _decisionRepository.GetByIdAsync(boardDecisionId);
            if (decision == null) return NotFound();

            ViewBag.Decision = decision;
            var items = await _attachmentRepository.GetByDecisionIdAsync(boardDecisionId);
            return View(items);
        }

        public async Task<IActionResult> Create(Guid boardDecisionId)
        {
            var decision = await _decisionRepository.GetByIdAsync(boardDecisionId);
            if (decision == null) return NotFound();

            ViewBag.Decision = decision;
            return View(new BoardDecisionAttachmentVm { BoardDecisionId = boardDecisionId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BoardDecisionAttachmentVm vm)
        {
            var decision = await _decisionRepository.GetByIdAsync(vm.BoardDecisionId);
            if (decision == null) return NotFound();
            ViewBag.Decision = decision;

            if (!ModelState.IsValid)
                return View(vm);

            if (vm.File == null || vm.File.Length <= 0)
            {
                ModelState.AddModelError(nameof(vm.File), "اختر ملفًا صالحًا للرفع.");
                return View(vm);
            }

            if (vm.File.Length > MaxFileSizeBytes)
            {
                ModelState.AddModelError(nameof(vm.File), "حجم الملف أكبر من الحد المسموح به 10 MB.");
                return View(vm);
            }

            var extension = Path.GetExtension(vm.File.FileName).ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
            {
                ModelState.AddModelError(nameof(vm.File), "نوع الملف غير مسموح.");
                return View(vm);
            }

            byte[] content;
            await using (var ms = new MemoryStream())
            {
                await vm.File.CopyToAsync(ms);
                content = ms.ToArray();
            }

            var entity = new BoardDecisionAttachment
            {
                Id = Guid.NewGuid(),
                BoardDecisionId = vm.BoardDecisionId,
                OriginalFileName = Path.GetFileName(vm.File.FileName),
                StoredFileName = $"{Guid.NewGuid():N}{extension}",
                ContentType = string.IsNullOrWhiteSpace(vm.File.ContentType) ? "application/octet-stream" : vm.File.ContentType,
                FileExtension = extension,
                FileSizeBytes = vm.File.Length,
                FileContent = content,
                AttachmentType = vm.AttachmentType,
                Notes = vm.Notes,
                CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            await _attachmentRepository.AddAsync(entity);
            TempData["Success"] = "تم رفع الملف بنجاح.";
            return RedirectToAction(nameof(Index), new { boardDecisionId = vm.BoardDecisionId });
        }

        public async Task<IActionResult> Download(Guid id)
        {
            var item = await _attachmentRepository.GetByIdAsync(id);
            if (item == null || item.FileContent == null || item.FileContent.Length == 0)
                return NotFound();

            var contentType = string.IsNullOrWhiteSpace(item.ContentType)
                ? "application/octet-stream"
                : item.ContentType;

            var fileName = string.IsNullOrWhiteSpace(item.OriginalFileName)
                ? $"attachment{item.FileExtension}"
                : item.OriginalFileName;

            return File(item.FileContent, contentType, fileName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _attachmentRepository.GetByIdAsync(id);
            if (item == null) return NotFound();

            var decisionId = item.BoardDecisionId;
            await _attachmentRepository.DeleteAsync(item);
            TempData["Success"] = "تم حذف الملف بنجاح.";
            return RedirectToAction(nameof(Index), new { boardDecisionId = decisionId });
        }
    }
}
