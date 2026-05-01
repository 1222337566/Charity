namespace InfrastrfuctureManagmentCore.Queries.Reports
{
    public class QuickActionDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconClass { get; set; } = "bx bx-right-arrow-alt";
        public string Controller { get; set; } = string.Empty;
        public string Action { get; set; } = "Index";
        public string? Policy { get; set; }
        public string? BadgeText { get; set; }
        public string ColorClass { get; set; } = "primary";
    }
}
