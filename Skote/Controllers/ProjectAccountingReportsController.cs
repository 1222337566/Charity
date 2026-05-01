using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.AccountingProjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class ProjectAccountingReportsController : Controller
{
    private readonly IProjectAccountingReportRepository _repository;
    private readonly AppDbContext _db;

    public ProjectAccountingReportsController(IProjectAccountingReportRepository repository, AppDbContext db)
    {
        _repository = repository;
        _db = db;
    }

    public IActionResult Index() => View();

    public async Task<IActionResult> ProjectCostSummary(Guid? projectId, Guid? costCenterId, DateTime? fromDate, DateTime? toDate)
    {
        var report = await _repository.GetProjectCostSummaryAsync(projectId, costCenterId, fromDate, toDate);
        var vm = new ProjectCostSummaryVm { ProjectId = projectId, CostCenterId = costCenterId, FromDate = fromDate, ToDate = toDate, Rows = report.Rows };
        await FillLookupsAsync(vm);
        return View(vm);
    }

    public async Task<IActionResult> ProjectLedger(Guid? projectId, DateTime? fromDate, DateTime? toDate)
    {
        var vm = new ProjectLedgerVm { ProjectId = projectId, FromDate = fromDate, ToDate = toDate };
        vm.Projects = await _db.Set<CharityProject>().AsNoTracking().OrderBy(x => x.Name)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();
        if (projectId.HasValue) vm.Ledger = await _repository.GetProjectLedgerAsync(projectId.Value, fromDate, toDate);
        return View(vm);
    }

    private async Task FillLookupsAsync(ProjectCostSummaryVm vm)
    {
        vm.Projects = await _db.Set<CharityProject>().AsNoTracking().OrderBy(x => x.Name)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToListAsync();
        vm.CostCenters = await _db.Set<CostCenter>().AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.NameAr)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = (string)x.NameAr }).ToListAsync();
    }
}
