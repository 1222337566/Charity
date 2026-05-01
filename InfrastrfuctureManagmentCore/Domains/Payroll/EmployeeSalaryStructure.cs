using InfrastrfuctureManagmentCore.Domains.HR;

namespace InfrastrfuctureManagmentCore.Domains.Payroll
{
    public class EmployeeSalaryStructure
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public HrEmployee? Employee { get; set; }
        public Guid SalaryItemDefinitionId { get; set; }
        public SalaryItemDefinition? SalaryItemDefinition { get; set; }
        public decimal Value { get; set; }
        public DateTime FromDate { get; set; } = DateTime.Today;
        public DateTime? ToDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }
    }
}
