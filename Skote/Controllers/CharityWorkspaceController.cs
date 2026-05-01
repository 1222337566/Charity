using InfrastrfuctureManagmentCore.Persistence.Repositories.Reports;
using InfrastructureManagmentWebFramework.Models.Reports.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skote.Helpers;

[Authorize(Policy = CharityPolicies.DashboardView)]
public class CharityWorkspaceController : Controller
{
    private readonly ICharityOperationsRepository _operationsRepository;

    public CharityWorkspaceController(ICharityOperationsRepository operationsRepository)
    {
        _operationsRepository = operationsRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var snapshot = await _operationsRepository.GetWorkspaceSnapshotAsync();
        return View(new CharityWorkspaceVm
        {
            NewBeneficiariesCount = snapshot.NewBeneficiariesCount,
            PendingAidRequestsCount = snapshot.PendingAidRequestsCount,
            DueGrantInstallmentsCount = snapshot.DueGrantInstallmentsCount,
            ProjectsEndingSoonCount = snapshot.ProjectsEndingSoonCount,
            MissingAttendanceCount = snapshot.MissingAttendanceCount,
            QuickActions = snapshot.QuickActions,
            Alerts = snapshot.Alerts,
            Deadlines = snapshot.Deadlines,
            RecentActivities = snapshot.RecentActivities
        });
    }
}
