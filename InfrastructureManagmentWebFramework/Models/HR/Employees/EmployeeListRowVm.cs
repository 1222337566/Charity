namespace InfrastructureManagmentWebFramework.Models.HR.Employees
{
    public class EmployeeListRowVm
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
        public string? JobTitleName { get; set; }
        public DateTime HireDate { get; set; }
        public decimal BasicSalary { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
