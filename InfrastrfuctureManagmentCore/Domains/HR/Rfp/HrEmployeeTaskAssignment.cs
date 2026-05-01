using InfrastrfuctureManagmentCore.Domains.HR;

namespace InfrastrfuctureManagmentCore.Domains.HR.Rfp
{
    public class HrEmployeeTaskAssignment
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public HrEmployee? Employee { get; set; }
        public Guid? CharityProjectId { get; set; }
        public string TaskTitle { get; set; } = null!;
        public string? TaskDescription { get; set; }
        public DateTime AssignedDate { get; set; } = DateTime.Today;
        public DateTime? DueDate { get; set; }
        public string Priority { get; set; } = "Medium";
        public string Status { get; set; } = "Assigned";
        public bool IsPrimaryDuty { get; set; }
        public string? Notes { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
