using InfrastrfuctureManagmentCore.Persistence.Repositories.Reports;
using InfrastructureManagmentWebFramework.Models.Reports.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skote.Helpers;

[Authorize(Policy = CharityPolicies.DashboardView)]
public class CharityDashboardController : Controller
{
    private readonly ICharityDashboardRepository _dashboardRepository;

    public CharityDashboardController(ICharityDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var snapshot = await _dashboardRepository.GetSnapshotAsync();
        return View(new CharityDashboardVm
        {
            BeneficiariesCount = snapshot.BeneficiariesCount,
            DonorsCount = snapshot.DonorsCount,
            FundersCount = snapshot.FundersCount,
            ProjectsCount = snapshot.ProjectsCount,
            EmployeesCount = snapshot.EmployeesCount,
            TotalDonations = snapshot.TotalDonations,
            TotalReceivedGrants = snapshot.TotalReceivedGrants,
            TotalAidDisbursed = snapshot.TotalAidDisbursed,
            TotalPayrollNet = snapshot.TotalPayrollNet,
            BeneficiaryStatuses = snapshot.BeneficiaryStatuses,
            MonthlyDonations = snapshot.MonthlyDonations,
            TopProjects = snapshot.TopProjects,
            RecentPayrollMonths = snapshot.RecentPayrollMonths,
            StoreMovements = snapshot.StoreMovements
        });
    }
}
