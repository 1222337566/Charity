namespace InfrastructureManagmentWebFramework.Models.HR.EmployeeProfile
{
    public class EmployeeHeaderVm
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
        public string? JobTitleName { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime HireDate { get; set; }
        public decimal BasicSalary { get; set; }
        public bool IsActive { get; set; }
    }
}
