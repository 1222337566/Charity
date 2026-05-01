using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    [Authorize]
    public class BeneficiaryCategoriesController : Controller
    {
        private readonly AppDbContext _db;

        public BeneficiaryCategoriesController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _db.Set<BeneficiaryCategory>()
                .AsNoTracking()
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.NameAr)
                .ToListAsync();

            return View(items);
        }

        public IActionResult Create()
        {
            return View(new BeneficiaryCategory
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                SortOrder = 100
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BeneficiaryCategory model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!string.IsNullOrWhiteSpace(model.Code))
            {
                var exists = await _db.Set<BeneficiaryCategory>()
                    .AnyAsync(x => x.Code == model.Code);

                if (exists)
                {
                    ModelState.AddModelError(nameof(model.Code), "كود التصنيف مستخدم من قبل.");
                    return View(model);
                }
            }

            model.Id = model.Id == Guid.Empty ? Guid.NewGuid() : model.Id;
            model.CreatedAtUtc = DateTime.UtcNow;
            model.CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _db.Set<BeneficiaryCategory>().Add(model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "تم إنشاء التصنيف بنجاح.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _db.Set<BeneficiaryCategory>().FindAsync(id);
            if (entity == null)
                return NotFound();

            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, BeneficiaryCategory model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            var entity = await _db.Set<BeneficiaryCategory>().FindAsync(id);
            if (entity == null)
                return NotFound();

            if (!string.IsNullOrWhiteSpace(model.Code))
            {
                var duplicate = await _db.Set<BeneficiaryCategory>()
                    .AnyAsync(x => x.Id != id && x.Code == model.Code);

                if (duplicate)
                {
                    ModelState.AddModelError(nameof(model.Code), "كود التصنيف مستخدم من قبل.");
                    return View(model);
                }
            }

            entity.NameAr = model.NameAr;
            entity.Code = model.Code;
            entity.Description = model.Description;
            entity.IsWaitingListCategory = model.IsWaitingListCategory;
            entity.RequiresDisabilityType = model.RequiresDisabilityType;
            entity.IsProjectRelated = model.IsProjectRelated;
            entity.IsActivityRelated = model.IsActivityRelated;
            entity.IsActive = model.IsActive;
            entity.SortOrder = model.SortOrder;

            await _db.SaveChangesAsync();

            TempData["Success"] = "تم تعديل التصنيف بنجاح.";
            return RedirectToAction(nameof(Index));
        }
    }
}
