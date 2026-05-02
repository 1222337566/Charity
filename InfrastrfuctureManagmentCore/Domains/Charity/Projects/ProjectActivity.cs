namespace InfrastrfuctureManagmentCore.Domains.Charity.Projects
{
    public class ProjectActivity
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime PlannedDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal PlannedCost { get; set; }
        public decimal ActualCost { get; set; }
        public string? Notes { get; set; }

        public CharityProject? Project { get; set; }
    }
}
