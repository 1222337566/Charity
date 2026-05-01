using InfrastrfuctureManagmentCore.Domains.Charity.Reports;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Reports
{
    public interface ICharityRfpReportRepository
    {
        Task<RfpDashboardSummary> GetDashboardSummaryAsync(DateTime? fromDate, DateTime? toDate);
        Task<List<HrEmployeeReportRow>> GetHrEmployeesAsync();
        Task<List<BeneficiaryReportRow>> GetBeneficiariesAsync(Guid? statusId = null);
        Task<List<MonthlyAidReportRow>> GetMonthlyAidAsync(DateTime? fromDate, DateTime? toDate);
        Task<List<ProjectActivityReportRow>> GetProjectsActivitiesAsync(DateTime? fromDate, DateTime? toDate);
        Task<List<BoardDecisionReportRow>> GetBoardMeetingsDecisionsAsync(DateTime? fromDate, DateTime? toDate);
    }
}
