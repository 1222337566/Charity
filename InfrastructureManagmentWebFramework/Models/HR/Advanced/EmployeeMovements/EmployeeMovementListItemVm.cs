namespace InfrastructureManagmentWebFramework.Models.HR.Advanced.EmployeeMovements
{
    public class EmployeeMovementListItemVm
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string MovementType { get; set; } = string.Empty;
        public DateTime EffectiveDate { get; set; }
        public string? FromDepartmentName { get; set; }
        public string? ToDepartmentName { get; set; }
        public string? FromJobTitleName { get; set; }
        public string? ToJobTitleName { get; set; }
        public string? DecisionNumber { get; set; }
    }
}
