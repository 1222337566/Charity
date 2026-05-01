using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using InfrastrfuctureManagmentCore.Domains.Charity.Funders;
using InfrastrfuctureManagmentCore.Domains.Charity.Kafala;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/search")]
    public class UnifiedSearchController : ControllerBase
    {
        private readonly AppDbContext _db;
        public UnifiedSearchController(AppDbContext db) => _db = db;

        [HttpGet("")]
        public async Task<IActionResult> Search(string q, int take = 10, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
                return Ok(new { results = Array.Empty<object>(), total = 0 });

            var results = new List<object>();

            // Beneficiaries
            var bens = await _db.Set<Beneficiary>().AsNoTracking()
                .Where(x => x.FullName.Contains(q) || (x.NationalId != null && x.NationalId.Contains(q)))
                .Take(take).Select(x => new { x.Id, Name = x.FullName, Sub = "مستفيد · " + x.NationalId, Type = "Beneficiary", Url = "/Beneficiaries/Details/" + x.Id })
                .ToListAsync(ct);
            results.AddRange(bens);

            // Donors
            var donors = await _db.Set<Donor>().AsNoTracking()
                .Where(x => x.FullName.Contains(q) || (x.PhoneNumber != null && x.PhoneNumber.Contains(q)))
                .Take(take).Select(x => new { x.Id, Name = x.FullName, Sub = "متبرع · " + x.PhoneNumber, Type = "Donor", Url = "/Donors/Details/" + x.Id })
                .ToListAsync(ct);
            results.AddRange(donors);

            // Funders
            var funders = await _db.Set<Funder>().AsNoTracking()
                .Where(x => x.Name.Contains(q))
                .Take(take).Select(x => new { x.Id, Name = x.Name, Sub = "ممول", Type = "Funder", Url = "/Funders/Details/" + x.Id })
                .ToListAsync(ct);
            results.AddRange(funders);

            // Kafala sponsors
            var sponsors = await _db.Set<KafalaSponsor>().AsNoTracking()
                .Where(x => x.FullName.Contains(q) || x.SponsorCode.Contains(q))
                .Take(5).Select(x => new { x.Id, Name = x.FullName, Sub = "كفيل · " + x.SponsorCode, Type = "KafalaSponsor", Url = "/KafalaSponsors/Edit/" + x.Id })
                .ToListAsync(ct);
            results.AddRange(sponsors);

            // Projects
            var projects = await _db.Set<CharityProject>().AsNoTracking()
                .Where(x => x.Name.Contains(q) || x.Code.Contains(q))
                .Take(5).Select(x => new { x.Id, Name = x.Name, Sub = "مشروع · " + x.Status, Type = "Project", Url = "/CharityProjects/Details/" + x.Id })
                .ToListAsync(ct);
            results.AddRange(projects);

            return Ok(new { results, total = results.Count });
        }
    }
}
