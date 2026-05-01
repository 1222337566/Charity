namespace InfrastrfuctureManagmentCore.Domains.HR
{
    public class HrEmployee
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? NationalId { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? AddressLine { get; set; }
        public Guid? DepartmentId { get; set; }
        public HrDepartment? Department { get; set; }
        public Guid? JobTitleId { get; set; }
        public HrJobTitle? JobTitle { get; set; }
        public DateTime HireDate { get; set; } = DateTime.Today;
        public string EmploymentType { get; set; } = "Permanent";
        public decimal BasicSalary { get; set; }
        public decimal? InsuranceSalary { get; set; }
        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }
        public string Status { get; set; } = "Active";
        public string? Notes { get; set; }
        public string? UserId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<HrAttendanceRecord> AttendanceRecords { get; set; } = new List<HrAttendanceRecord>();
    }
}
