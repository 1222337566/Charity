using System;
using System.Linq;
using System.Threading.Tasks;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    [Authorize]
    public class BeneficiaryWaitingListsController : Controller
    {
        private readonly AppDbContext _db;

        public BeneficiaryWaitingListsController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(Guid? categoryId = null)
        {
            var categories = await _db.Set<BeneficiaryCategory>()
                .AsNoTracking()
                .Where(x => x.IsWaitingListCategory && x.IsActive)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.NameAr)
                .ToListAsync();

            ViewBag.WaitingCategories = categories;
            ViewBag.SelectedCategoryId = categoryId;

            var query = _db.Set<BeneficiaryCategoryAssignment>()
                .AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.Beneficiary)
                .Where(x => x.Category.IsWaitingListCategory);

            if (categoryId.HasValue)
                query = query.Where(x => x.CategoryId == categoryId.Value);

            var items = await query
                .OrderByDescending(x => x.AssignedAtUtc)
                .ToListAsync();

            return View(items);
        }
    }
}
