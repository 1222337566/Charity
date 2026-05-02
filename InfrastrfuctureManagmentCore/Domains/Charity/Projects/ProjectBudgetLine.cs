namespace InfrastrfuctureManagmentCore.Domains.Charity.Projects
{
    public class ProjectBudgetLine
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string LineName { get; set; } = string.Empty;
        public string LineType { get; set; } = string.Empty;
        public decimal PlannedAmount { get; set; }
        public decimal ActualAmount { get; set; }
        public string? Notes { get; set; }

        public CharityProject? Project { get; set; }
    }
}
