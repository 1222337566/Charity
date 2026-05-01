namespace InfrastrfuctureManagmentCore.Domains.HR
{
    public class HrLeaveBalance
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public HrEmployee? Employee { get; set; }
        public Guid LeaveTypeId { get; set; }
        public HrLeaveType? LeaveType { get; set; }
        public int Year { get; set; }
        public decimal TotalEntitled { get; set; }   // المستحق
        public decimal TotalUsed { get; set; }        // المستخدم
        public decimal TotalPending { get; set; }     // المعلق (طلبات لم تُعتمد)
        public decimal CarriedOver { get; set; }      // مُرحَّل من العام السابق
        public decimal Remaining => TotalEntitled + CarriedOver - TotalUsed;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
