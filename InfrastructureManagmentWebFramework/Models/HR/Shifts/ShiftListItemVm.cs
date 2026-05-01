namespace InfrastructureManagmentWebFramework.Models.HR.Shifts
{
    public class ShiftListItemVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int GraceMinutes { get; set; }
        public bool IsActive { get; set; }
    }
}
