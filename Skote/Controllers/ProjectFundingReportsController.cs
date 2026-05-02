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
    public class ProjectFundingReportsController : Controller
    {
        private readonly AppDbContext _db;

        public ProjectFundingReportsController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(Guid? projectId, Guid? grantorId, CancellationToken ct)
        {
            var vm = new ProjectFundingReportVm
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

            var agreements = await query.ToListAsync(ct);

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

        private async Task FillLookupsAsync(ProjectFundingReportVm vm, CancellationToken ct)
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
    }
}
