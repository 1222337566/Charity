using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.AccountingProjects.Phases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    public class ProjectPhaseAccountingReportsController : Controller
    {
        private readonly IProjectPhaseAccountingReportRepository _repository;
        private readonly AppDbContext _db;

        public ProjectPhaseAccountingReportsController(IProjectPhaseAccountingReportRepository repository, AppDbContext db)
        {
            _repository = repository;
            _db = db;
        }

        public IActionResult Index() => View();

        public async Task<IActionResult> PhaseCostSummary(Guid? projectId, Guid? phaseId, DateTime? fromDate, DateTime? toDate)
        {
            var report = await _repository.GetPhaseCostSummaryAsync(projectId, phaseId, fromDate, toDate);
            var vm = new ProjectPhaseCostSummaryVm { ProjectId = projectId, PhaseId = phaseId, FromDate = fromDate, ToDate = toDate, Rows = report.Rows };
            await FillLookupsAsync(vm, projectId);
            return View(vm);
        }

        public async Task<IActionResult> PhaseLedger(Guid? projectId, Guid? phaseId, DateTime? fromDate, DateTime? toDate)
        {
            var vm = new ProjectPhaseLedgerVm { ProjectId = projectId, PhaseId = phaseId, FromDate = fromDate, ToDate = toDate };
            await FillLookupsAsync(vm, projectId);
            if (phaseId.HasValue) vm.Ledger = await _repository.GetPhaseLedgerAsync(phaseId.Value, fromDate, toDate);
            return View(vm);
        }

        public async Task<IActionResult> Alerts(Guid? projectId, DateTime? asOfDate)
        {
            var report = await _repository.GetAlertsAsync(projectId, asOfDate);
            var vm = new ProjectPhaseAlertsVm { ProjectId = projectId, AsOfDate = report.AsOfDate, Rows = report.Rows };
            vm.Projects = await _db.Set<CharityProject>().AsNoTracking().OrderBy(x => x.Name)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();
            return View(vm);
        }

        private async Task FillLookupsAsync(ProjectPhaseCostSummaryVm vm, Guid? projectId)
        {
            vm.Projects = await _db.Set<CharityProject>().AsNoTracking().OrderBy(x => x.Name)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();
            vm.Phases = await _db.Set<ProjectPhase>().AsNoTracking()
                .Where(x => !projectId.HasValue || x.ProjectId == projectId.Value)
                .OrderBy(x => x.SortOrder).ThenBy(x => x.Name)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();
        }

        private async Task FillLookupsAsync(ProjectPhaseLedgerVm vm, Guid? projectId)
        {
            vm.Projects = await _db.Set<CharityProject>().AsNoTracking().OrderBy(x => x.Name)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();
            vm.Phases = await _db.Set<ProjectPhase>().AsNoTracking()
                .Where(x => !projectId.HasValue || x.ProjectId == projectId.Value)
                .OrderBy(x => x.SortOrder).ThenBy(x => x.Name)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();
        }
    }
}
