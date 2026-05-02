namespace InfrastructureManagmentWebFramework.Models.Payroll.Months
{
    public class PayrollMonthDetailsVm
    {
        public Guid Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string Status { get; set; } = string.Empty;
        public int EmployeesCount { get; set; }
        public decimal TotalBasic { get; set; }
        public decimal TotalAdditions { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal TotalNet { get; set; }
    }
}
