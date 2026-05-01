namespace InfrastrfuctureManagmentCore.Domains.Charity.Reports
{
    public class HrEmployeeReportRow
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
        public string? JobTitleName { get; set; }
        public DateTime? HireDate { get; set; }
        public string? EmploymentType { get; set; }
        public decimal BasicSalary { get; set; }
        public string? Status { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
