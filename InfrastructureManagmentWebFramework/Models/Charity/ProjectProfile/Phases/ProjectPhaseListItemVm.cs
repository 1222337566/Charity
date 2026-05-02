namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Phases
{
    public class ProjectPhaseListItemVm
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal ProgressPercent { get; set; }
        public decimal PlannedCost { get; set; }
        public decimal ActualCost { get; set; }
        public string? ResponsiblePersonName { get; set; }
    }
}
