namespace InfrastructureManagmentWebFramework.Models.Payroll.SalaryItems
{
    public class SalaryItemDefinitionListItemVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;
        public string CalculationMethod { get; set; } = string.Empty;
        public decimal? DefaultValue { get; set; }
        public bool IsActive { get; set; }
    }
}
