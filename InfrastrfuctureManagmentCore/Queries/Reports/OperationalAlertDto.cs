namespace InfrastrfuctureManagmentCore.Queries.Reports
{
    public class OperationalAlertDto
    {
        public string Title { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string Severity { get; set; } = "info"; // danger / warning / success / info
        public int Count { get; set; }
        public string? Controller { get; set; }
        public string? Action { get; set; }
        public Guid? RouteId { get; set; }
        public string? RouteKey { get; set; }
        public string? BadgeText { get; set; }
    }
}
