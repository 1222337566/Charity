namespace InfrastructureManagmentWebFramework.Models.Charity.Projects
{
    public class ProjectListPageVm
    {
        public ProjectListFilterVm Filter { get; set; } = new();
        public List<ProjectListRowVm> Items { get; set; } = new();
    }
}
