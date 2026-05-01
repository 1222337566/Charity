using InfrastrfuctureManagmentCore.Domains.HR;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.HR
{
    public interface IHrAttendanceRecordRepository
    {
        Task<List<HrAttendanceRecord>> SearchAsync(Guid? employeeId, DateTime? fromDate, DateTime? toDate, string? status);
        Task<HrAttendanceRecord?> GetByIdAsync(Guid id);
        Task AddAsync(HrAttendanceRecord entity);
        Task UpdateAsync(HrAttendanceRecord entity);
    }
}
