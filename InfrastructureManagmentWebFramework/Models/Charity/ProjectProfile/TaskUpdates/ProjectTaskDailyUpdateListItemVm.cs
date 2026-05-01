namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.TaskUpdates
{
    public class ProjectTaskDailyUpdateListItemVm
    {
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal ProgressPercent { get; set; }
        public decimal HoursSpent { get; set; }
        public string? Note { get; set; }
        public string? BlockerNote { get; set; }
        public string? NextAction { get; set; }
        public string? CreatedByName { get; set; }
    }
}
