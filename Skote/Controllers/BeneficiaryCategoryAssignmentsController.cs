using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Skote.ViewModels.Charity.Beneficiaries;

namespace Skote.Controllers
{
    [Authorize]
    public class BeneficiaryCategoryAssignmentsController : Controller
    {
        private readonly AppDbContext _db;

        public BeneficiaryCategoryAssignmentsController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Create(Guid beneficiaryId)
        {
            var beneficiaryExists = await _db.Set<Beneficiary>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == beneficiaryId);

            if (!beneficiaryExists)
                return NotFound();

            await LoadLookupsAsync();

            return View(new CreateBeneficiaryCategoryAssignmentVm
            {
                BeneficiaryId = beneficiaryId,
                AssignedAtUtc = DateTime.UtcNow,
                Status = BeneficiaryCategoryAssignmentStatuses.Waiting
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBeneficiaryCategoryAssignmentVm model)
        {
            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync();
                return View(model);
            }

            var beneficiaryExists = await _db.Set<Beneficiary>()
                .AnyAsync(x => x.Id == model.BeneficiaryId);

            if (!beneficiaryExists)
                return NotFound();

            var categoryExists = await _db.Set<BeneficiaryCategory>()
                .AnyAsync(x => x.Id == model.CategoryId && x.IsActive);

            if (!categoryExists)
            {
                ModelState.AddModelError(nameof(model.CategoryId), "التصنيف غير صحيح أو غير مفعل.");
                await LoadLookupsAsync();
                return View(model);
            }

            var entity = new BeneficiaryCategoryAssignment
            {
                Id = Guid.NewGuid(),
                BeneficiaryId = model.BeneficiaryId,
                CategoryId = model.CategoryId,
                ProjectId = model.ProjectId,
                ProjectActivityId = model.ProjectActivityId,
                Status = string.IsNullOrWhiteSpace(model.Status)
                    ? BeneficiaryCategoryAssignmentStatuses.Waiting
                    : model.Status,
                AssignedAtUtc = model.AssignedAtUtc == default ? DateTime.UtcNow : model.AssignedAtUtc,
                Notes = model.Notes,
                CreatedAtUtc = DateTime.UtcNow,
                CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            _db.Set<BeneficiaryCategoryAssignment>().Add(entity);
            await _db.SaveChangesAsync();

            TempData["Success"] = "تم إضافة التصنيف / المشاركة للمستفيد.";
            return RedirectToAction("Details", "Beneficiaries", new { id = model.BeneficiaryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _db.Set<BeneficiaryCategoryAssignment>().FindAsync(id);
            if (entity == null)
                return NotFound();

            var beneficiaryId = entity.BeneficiaryId;

            _db.Set<BeneficiaryCategoryAssignment>().Remove(entity);
            await _db.SaveChangesAsync();

            TempData["Success"] = "تم حذف التصنيف من المستفيد.";
            return RedirectToAction("Details", "Beneficiaries", new { id = beneficiaryId });
        }

        private async Task LoadLookupsAsync()
        {
            var categories = await _db.Set<BeneficiaryCategory>()
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.NameAr)
                .ToListAsync();

            ViewBag.CategoryId = new SelectList(categories, "Id", "NameAr");

            ViewBag.Statuses = new SelectList(new[]
            {
                new { Id = BeneficiaryCategoryAssignmentStatuses.Waiting, Name = "قائمة انتظار" },
                new { Id = BeneficiaryCategoryAssignmentStatuses.Accepted, Name = "مقبول" },
                new { Id = BeneficiaryCategoryAssignmentStatuses.Participant, Name = "مشارك" },
                new { Id = BeneficiaryCategoryAssignmentStatuses.Served, Name = "تلقى الخدمة" },
                new { Id = BeneficiaryCategoryAssignmentStatuses.Cancelled, Name = "ملغي" }
            }, "Id", "Name");
        }
    }
}
