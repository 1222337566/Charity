using InfrastrfuctureManagmentCore.Queries.Reports;

namespace InfrastructureManagmentWebFramework.Models.Reports.Dashboard
{
    public class CharityWorkspaceVm
    {
        public int NewBeneficiariesCount { get; set; }
        public int PendingAidRequestsCount { get; set; }
        public int DueGrantInstallmentsCount { get; set; }
        public int ProjectsEndingSoonCount { get; set; }
        public int MissingAttendanceCount { get; set; }

        public List<QuickActionDto> QuickActions { get; set; } = new();
        public List<OperationalAlertDto> Alerts { get; set; } = new();
        public List<DeadlineReminderDto> Deadlines { get; set; } = new();
        public List<RecentActivityDto> RecentActivities { get; set; } = new();
    }
}
