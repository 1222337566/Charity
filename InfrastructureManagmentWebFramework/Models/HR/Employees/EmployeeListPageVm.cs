namespace InfrastructureManagmentWebFramework.Models.HR.Employees
{
    public class EmployeeListPageVm
    {
        public EmployeeListFilterVm Filter { get; set; } = new();
        public List<EmployeeListRowVm> Items { get; set; } = new();
    }
}
