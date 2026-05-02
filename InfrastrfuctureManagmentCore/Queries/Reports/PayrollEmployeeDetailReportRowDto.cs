namespace InfrastrfuctureManagmentCore.Queries.Reports
{
    public class PayrollEmployeeDetailReportRowDto
    {
        public Guid PayrollMonthId { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal Additions { get; set; }
        public decimal AttendanceDeduction { get; set; }
        public decimal OtherDeductions { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetAmount { get; set; }
    }
}
