using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastrfuctureManagmentCore.Domains.Payments;

namespace InfrastrfuctureManagmentCore.Domains.Payroll
{
    public class PayrollPayment
    {
        public Guid Id { get; set; }
        public Guid PayrollEmployeeId { get; set; }
        public PayrollEmployee? PayrollEmployee { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Today;
        public Guid? PaymentMethodId { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public Guid? FinancialAccountId { get; set; }
        public FinancialAccount? FinancialAccount { get; set; }
        public decimal Amount { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Notes { get; set; }
    }
}
