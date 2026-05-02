namespace InfrastructureManagmentWebFramework.Models.Payroll.Employees
{
    public class PayrollEmployeeDetailsVm
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string MonthName { get; set; } = string.Empty;
        public decimal BasicSalary { get; set; }
        public decimal Additions { get; set; }
        public decimal AttendanceDeduction { get; set; }
        public decimal OtherDeductions { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetAmount { get; set; }
        public List<PayrollEmployeeDetailsItemVm> Items { get; set; } = new();
    }

    public class PayrollEmployeeDetailsItemVm
    {
        public string? Name { get; set; }
        public string ItemType { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string? Notes { get; set; }
    }
}
