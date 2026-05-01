namespace InfrastrfuctureManagmentCore.Domains.Payroll
{
    public class PayrollEmployeeItem
    {
        public Guid Id { get; set; }
        public Guid PayrollEmployeeId { get; set; }
        public PayrollEmployee? PayrollEmployee { get; set; }
        public Guid? SalaryItemDefinitionId { get; set; }
        public SalaryItemDefinition? SalaryItemDefinition { get; set; }
        public string ItemType { get; set; } = "Addition";
        public decimal Value { get; set; }
        public string? Notes { get; set; }
    }
}
