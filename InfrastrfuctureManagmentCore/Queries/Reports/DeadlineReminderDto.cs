namespace InfrastrfuctureManagmentCore.Queries.Reports
{
    public class DeadlineReminderDto
    {
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public int DaysRemaining { get; set; }
        public string? Notes { get; set; }
        public string? Controller { get; set; }
        public string? Action { get; set; }
        public Guid? RouteId { get; set; }
        public string? RouteKey { get; set; }
    }
}
