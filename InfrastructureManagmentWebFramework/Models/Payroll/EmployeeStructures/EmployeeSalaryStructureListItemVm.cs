namespace InfrastructureManagmentWebFramework.Models.Payroll.EmployeeStructures
{
    public class EmployeeSalaryStructureListItemVm
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string SalaryItemName { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool IsActive { get; set; }
    }
}
