using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile;

namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectPlanning.Tasks
{
    public class ProjectTaskDailyFollowUpVm
    {
        public ProjectHeaderVm ProjectHeader { get; set; } = new();
        public DateTime Date { get; set; }
        public List<ProjectTaskDailyFollowUpRowVm> Rows { get; set; } = new();
    }

    public class ProjectTaskDailyFollowUpRowVm
    {
        public Guid TaskId { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public string PhaseName { get; set; } = string.Empty;
        public string ActivityTitle { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal PercentComplete { get; set; }
        public string? AssignedToName { get; set; }
        public decimal TodayHours { get; set; }
        public string? TodayNote { get; set; }
        public bool MissingFollowUp { get; set; }
    }
}
