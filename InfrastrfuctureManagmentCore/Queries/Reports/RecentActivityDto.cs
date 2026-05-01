namespace InfrastrfuctureManagmentCore.Queries.Reports
{
    public class RecentActivityDto
    {
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Source { get; set; } = string.Empty;
        public string? Controller { get; set; }
        public string? Action { get; set; }
        public Guid? RouteId { get; set; }
        public string? RouteKey { get; set; }
    }
}
