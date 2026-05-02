using InfrastrfuctureManagmentCore.Domains.Charity.Funding;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Funding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    [Authorize]
    public class ProjectFundingAgreementsController : Controller
    {
        private readonly AppDbContext _db;

        public ProjectFundingAgreementsController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(Guid? projectId, Guid? grantorId, CancellationToken ct)
        {
            var vm = new ProjectFundingAgreementIndexVm
            {
                ProjectId = projectId,
                GrantorId = grantorId
            };

            await FillLookupsAsync(vm, ct);

            var query = _db.Set<ProjectFundingAgreement>()
                .AsNoTracking()
                .Include(x => x.Project)
                .Include(x => x.Grantor)
                .Include(x => x.Installments)
                .AsQueryable();

            if (projectId.HasValue)
                query = query.Where(x => x.ProjectId == projectId.Value);

            if (grantorId.HasValue)
                query = query.Where(x => x.GrantorId == grantorId.Value);

            var agreements = await query
                .OrderByDescending(x => x.StartDateUtc)
                .ToListAsync(ct);

            vm.Rows = agreements.Select(x => new ProjectFundingAgreementRowVm
            {
                Id = x.Id,
                AgreementNumber = x.AgreementNumber,
                ProjectName = x.Project?.Name ?? string.Empty,
                GrantorName = x.Grantor?.NameAr ?? string.Empty,
                FundingAmount = x.FundingAmount,
                ReceivedAmount = x.Installments.Where(i => i.Status == "Received").Sum(i => i.Amount),
                StartDateUtc = x.StartDateUtc,
                EndDateUtc = x.EndDateUtc,
                Status = x.Status,
                ContactPerson = x.ContactPerson,
                ContactEmail = x.ContactEmail
            }).ToList();

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            var vm = new CreateProjectFundingAgreementVm
            {
                AgreementNumber = $"FA-{DateTime.Now:yyyyMMddHHmm}",
                StartDateUtc = DateTime.UtcNow
            };

            await FillCreateLookupsAsync(vm, ct);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProjectFundingAgreementVm vm, CancellationToken ct)
        {
            await FillCreateLookupsAsync(vm, ct);

            if (!ModelState.IsValid)
                return View(vm);

            var agreement = new ProjectFundingAgreement
            {
                Id = Guid.NewGuid(),
                AgreementNumber = vm.AgreementNumber,
                GrantorId = vm.GrantorId,
                ProjectId = vm.ProjectId,
                FundingAmount = vm.FundingAmount,
                StartDateUtc = vm.StartDateUtc,
                EndDateUtc = vm.EndDateUtc,
                ContactPerson = vm.ContactPerson,
                ContactEmail = vm.ContactEmail,
                Notes = vm.Notes,
                Status = "Active",
                CreatedAtUtc = DateTime.UtcNow
            };

            _db.Set<ProjectFundingAgreement>().Add(agreement);
            await _db.SaveChangesAsync(ct);

            TempData["Success"] = "تم إنشاء اتفاقية التمويل بنجاح";
            return RedirectToAction(nameof(Index));
        }

        private async Task FillLookupsAsync(ProjectFundingAgreementIndexVm vm, CancellationToken ct)
        {
            vm.Projects = await _db.Set<CharityProject>()
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name,
                    Selected = vm.ProjectId.HasValue && vm.ProjectId.Value == x.Id
                })
                .ToListAsync(ct);

            vm.Grantors = await _db.Set<Grantor>()
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.NameAr)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.NameAr,
                    Selected = vm.GrantorId.HasValue && vm.GrantorId.Value == x.Id
                })
                .ToListAsync(ct);
        }

        private async Task FillCreateLookupsAsync(CreateProjectFundingAgreementVm vm, CancellationToken ct)
        {
            vm.Projects = await _db.Set<CharityProject>()
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name,
                    Selected = vm.ProjectId == x.Id
                })
                .ToListAsync(ct);

            vm.Grantors = await _db.Set<Grantor>()
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.NameAr)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.NameAr,
                    Selected = vm.GrantorId == x.Id
                })
                .ToListAsync(ct);
        }
    }
}
