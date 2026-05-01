namespace InfrastructureManagmentWebFramework.Models.HR.Employees
{
    public class EmployeeDetailsVm
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? NationalId { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? AddressLine { get; set; }
        public string? DepartmentName { get; set; }
        public string? JobTitleName { get; set; }
        public DateTime HireDate { get; set; }
        public string EmploymentType { get; set; } = string.Empty;
        public decimal BasicSalary { get; set; }
        public decimal? InsuranceSalary { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public int AttendanceRecordsCount { get; set; }
    }
}
