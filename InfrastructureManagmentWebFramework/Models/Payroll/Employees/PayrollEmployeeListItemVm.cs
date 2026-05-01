namespace InfrastructureManagmentWebFramework.Models.Payroll.Employees
{
    public class PayrollEmployeeListItemVm
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public decimal BasicSalary { get; set; }
        public decimal Additions { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetAmount { get; set; }
        public string? Notes { get; set; }
    }
}
