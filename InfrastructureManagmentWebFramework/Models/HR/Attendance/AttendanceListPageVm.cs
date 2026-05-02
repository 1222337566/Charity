namespace InfrastructureManagmentWebFramework.Models.HR.Attendance
{
    public class AttendanceListPageVm
    {
        public AttendanceListFilterVm Filter { get; set; } = new();
        public List<AttendanceListRowVm> Items { get; set; } = new();
    }
}
