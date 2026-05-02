using InfrastrfuctureManagmentCore.Domains.HR;

namespace InfrastrfuctureManagmentCore.Domains.Payroll
{
    public class PayrollEmployee
    {
        public Guid Id { get; set; }
        public Guid PayrollMonthId { get; set; }
        public PayrollMonth? PayrollMonth { get; set; }
        public Guid EmployeeId { get; set; }
        public HrEmployee? Employee { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal AttendanceDeduction { get; set; }
        public decimal OtherDeductions { get; set; }
        public decimal Additions { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetAmount { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public ICollection<PayrollEmployeeItem> Items { get; set; } = new List<PayrollEmployeeItem>();
        public ICollection<PayrollPayment> Payments { get; set; } = new List<PayrollPayment>();
    }
}
