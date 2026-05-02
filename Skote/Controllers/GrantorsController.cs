using InfrastrfuctureManagmentCore.Domains.Charity.Funding;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Funding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    [Authorize]
    public class GrantorsController : Controller
    {
        private readonly AppDbContext _db;

        public GrantorsController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var agreements = await _db.Set<ProjectFundingAgreement>()
                .AsNoTracking()
                .Include(x => x.Installments)
                .ToListAsync(ct);

            var grantors = await _db.Set<Grantor>()
                .AsNoTracking()
                .OrderBy(x => x.NameAr)
                .ToListAsync(ct);

            var vm = new GrantorIndexVm
            {
                Rows = grantors.Select(g =>
                {
                    var gAgreements = agreements.Where(a => a.GrantorId == g.Id).ToList();

                    return new GrantorRowVm
                    {
                        Id = g.Id,
                        GrantorCode = g.GrantorCode,
                        NameAr = g.NameAr,
                        ContactPerson = g.ContactPerson,
                        Email = g.Email,
                        IsActive = g.IsActive,
                        AgreementsCount = gAgreements.Count,
                        TotalFunding = gAgreements.Sum(a => a.FundingAmount)
                    };
                }).ToList()
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateGrantorVm
            {
                GrantorCode = $"GR-{DateTime.Now:yyyyMMddHHmm}"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateGrantorVm vm, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var exists = await _db.Set<Grantor>()
                .AnyAsync(x => x.GrantorCode == vm.GrantorCode, ct);

            if (exists)
            {
                ModelState.AddModelError(nameof(vm.GrantorCode), "كود الجهة موجود من قبل");
                return View(vm);
            }

            var grantor = new Grantor
            {
                Id = Guid.NewGuid(),
                GrantorCode = vm.GrantorCode,
                NameAr = vm.NameAr,
                NameEn = vm.NameEn,
                ContactPerson = vm.ContactPerson,
                Email = vm.Email,
                PhoneNumber = vm.PhoneNumber,
                Address = vm.Address,
                Notes = vm.Notes,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            _db.Set<Grantor>().Add(grantor);
            await _db.SaveChangesAsync(ct);

            TempData["Success"] = "تم إنشاء الجهة المانحة بنجاح";
            return RedirectToAction(nameof(Index));
        }
    }
}
