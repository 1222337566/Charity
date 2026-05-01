namespace InfrastructureManagmentWebFramework.Models.HR.JobTitles
{
    public class JobTitleListItemVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
