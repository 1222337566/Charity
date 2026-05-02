using InfrastrfuctureManagmentCore.Queries.Reports;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Reports
{
    public interface ICharityDashboardRepository
    {
        Task<CharityDashboardSnapshotDto> GetSnapshotAsync(int donationMonths = 6, int topProjects = 5, int payrollTake = 6);
    }
}
