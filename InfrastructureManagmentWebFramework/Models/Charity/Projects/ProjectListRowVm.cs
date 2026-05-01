namespace InfrastructureManagmentWebFramework.Models.Charity.Projects
{
    public class ProjectListRowVm
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Budget { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
