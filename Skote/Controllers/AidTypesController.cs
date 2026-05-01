using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Skote.Controllers
{
    [Authorize]
    public class AidTypesController : Controller
    {
        private readonly AppDbContext _db;
        public AidTypesController(AppDbContext db) => _db = db;

        // ── قائمة أنواع المساعدات ──
        public async Task<IActionResult> Index()
        {
            var list = await _db.Set<AidTypeLookup>()
                .AsNoTracking()
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.NameAr)
                .ToListAsync();
            return View(list);
        }

        // ── إضافة ──
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string nameAr, string? nameEn,
            string? description, string? icon, string category, int displayOrder)
        {
            if (string.IsNullOrWhiteSpace(nameAr))
            {
                TempData["Error"] = "اسم النوع مطلوب";
                return RedirectToAction(nameof(Index));
            }

            var exists = await _db.Set<AidTypeLookup>()
                .AnyAsync(x => x.NameAr == nameAr.Trim());
            if (exists)
            {
                TempData["Error"] = $"نوع المساعدة «{nameAr}» موجود مسبقاً";
                return RedirectToAction(nameof(Index));
            }

            _db.Set<AidTypeLookup>().Add(new AidTypeLookup
            {
                Id           = Guid.NewGuid(),
                NameAr       = nameAr.Trim(),
                NameEn       = nameEn?.Trim(),
                DisplayOrder = displayOrder > 0 ? displayOrder : 99,
                IsActive     = true
            });
            await _db.SaveChangesAsync();
            TempData["Success"] = $"تم إضافة نوع المساعدة «{nameAr}» بنجاح";
            return RedirectToAction(nameof(Index));
        }

        // ── تعديل ──
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, string nameAr, string? nameEn,
            int displayOrder, bool isActive)
        {
            var entity = await _db.Set<AidTypeLookup>().FindAsync(id);
            if (entity == null) return NotFound();

            entity.NameAr       = nameAr.Trim();
            entity.NameEn       = nameEn?.Trim();
            entity.DisplayOrder = displayOrder;
            entity.IsActive     = isActive;

            await _db.SaveChangesAsync();
            TempData["Success"] = "تم حفظ التعديلات";
            return RedirectToAction(nameof(Index));
        }

        // ── تفعيل / تعطيل ──
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(Guid id)
        {
            var entity = await _db.Set<AidTypeLookup>().FindAsync(id);
            if (entity == null) return NotFound();
            entity.IsActive = !entity.IsActive;
            await _db.SaveChangesAsync();
            TempData["Success"] = entity.IsActive ? "تم تفعيل النوع" : "تم تعطيل النوع";
            return RedirectToAction(nameof(Index));
        }
    }
}
