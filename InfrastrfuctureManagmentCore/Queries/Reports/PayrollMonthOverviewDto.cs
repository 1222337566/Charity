namespace InfrastrfuctureManagmentCore.Queries.Reports
{
    public class PayrollMonthOverviewDto
    {
        public Guid PayrollMonthId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string Status { get; set; } = string.Empty;
        public int EmployeesCount { get; set; }
        public decimal TotalNet { get; set; }
    }
}
