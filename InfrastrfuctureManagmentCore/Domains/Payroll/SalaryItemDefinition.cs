namespace InfrastrfuctureManagmentCore.Domains.Payroll
{
    public class SalaryItemDefinition
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ItemType { get; set; } = "Addition"; // Addition / Deduction
        public string CalculationMethod { get; set; } = "Fixed"; // Fixed / Percentage / Manual
        public decimal? DefaultValue { get; set; }
        public bool IsTaxable { get; set; }
        public bool IsInsuranceIncluded { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }

        public ICollection<EmployeeSalaryStructure> EmployeeSalaryStructures { get; set; } = new List<EmployeeSalaryStructure>();
        public ICollection<PayrollEmployeeItem> PayrollEmployeeItems { get; set; } = new List<PayrollEmployeeItem>();
    }
}
