using InfrastrfuctureManagmentCore.Domains.HR;

namespace InfrastrfuctureManagmentCore.Domains.HR.Advanced
{
    public class HrEmployeeMovement
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public HrEmployee? Employee { get; set; }
        public string MovementType { get; set; } = "Transfer";
        public DateTime EffectiveDate { get; set; } = DateTime.Today;
        public Guid? FromDepartmentId { get; set; }
        public HrDepartment? FromDepartment { get; set; }
        public Guid? ToDepartmentId { get; set; }
        public HrDepartment? ToDepartment { get; set; }
        public Guid? FromJobTitleId { get; set; }
        public HrJobTitle? FromJobTitle { get; set; }
        public Guid? ToJobTitleId { get; set; }
        public HrJobTitle? ToJobTitle { get; set; }
        public string? DecisionNumber { get; set; }
        public string? Notes { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
