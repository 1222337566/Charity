using InfrastrfuctureManagmentCore.Domains.HR;
using InfrastrfuctureManagmentCore.Domains.Payroll;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Payroll;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Payroll
{
    public class PayrollEmployeeRepository : IPayrollEmployeeRepository
    {
        private readonly AppDbContext _db;
        public PayrollEmployeeRepository(AppDbContext db) => _db = db;

        public Task<List<PayrollEmployee>> GetByPayrollMonthIdAsync(Guid payrollMonthId)
            => _db.Set<PayrollEmployee>()
                .AsNoTracking()
                .Include(x => x.Employee)
                .Include(x => x.Items).ThenInclude(x => x.SalaryItemDefinition)
                .Where(x => x.PayrollMonthId == payrollMonthId)
                .OrderBy(x => x.Employee!.FullName)
                .ToListAsync();

        public Task<PayrollEmployee?> GetByIdAsync(Guid id)
            => _db.Set<PayrollEmployee>()
                .AsNoTracking()
                .Include(x => x.Employee)
                .Include(x => x.PayrollMonth)
                .Include(x => x.Items).ThenInclude(x => x.SalaryItemDefinition)
                .Include(x => x.Payments).ThenInclude(x => x.PaymentMethod)
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task GenerateForMonthAsync(Guid payrollMonthId)
        {
            var payrollMonth = await _db.Set<PayrollMonth>().FirstOrDefaultAsync(x => x.Id == payrollMonthId);
            if (payrollMonth == null) throw new InvalidOperationException("Payroll month not found.");
            if (payrollMonth.Status == "Approved" || payrollMonth.Status == "Posted")
                throw new InvalidOperationException("لا يمكن إعادة احتساب شهر معتمد أو مرحل.");

            var fromDate = new DateTime(payrollMonth.Year, payrollMonth.Month, 1);
            var toDate = fromDate.AddMonths(1).AddDays(-1);
            var daysInMonth = DateTime.DaysInMonth(payrollMonth.Year, payrollMonth.Month);

            var employees = await _db.Set<HrEmployee>()
                .AsNoTracking()
                .Where(x => x.IsActive && x.Status == "Active")
                .OrderBy(x => x.FullName)
                .ToListAsync();

            var structures = await _db.Set<EmployeeSalaryStructure>()
                .AsNoTracking()
                .Include(x => x.SalaryItemDefinition)
                .Where(x => x.IsActive && x.FromDate <= toDate && (x.ToDate == null || x.ToDate >= fromDate))
                .ToListAsync();

            var attendance = await _db.Set<HrAttendanceRecord>()
                .AsNoTracking()
                .Where(x => x.AttendanceDate >= fromDate && x.AttendanceDate <= toDate)
                .ToListAsync();

            var existing = _db.Set<PayrollEmployee>().Where(x => x.PayrollMonthId == payrollMonthId);
            var existingItems = _db.Set<PayrollEmployeeItem>().Where(x => existing.Select(e => e.Id).Contains(x.PayrollEmployeeId));
            _db.Set<PayrollEmployeeItem>().RemoveRange(existingItems);
            _db.Set<PayrollEmployee>().RemoveRange(existing);
            await _db.SaveChangesAsync();

            var payrollEmployees = new List<PayrollEmployee>();
            var payrollItems = new List<PayrollEmployeeItem>();

            foreach (var employee in employees)
            {
                var employeeAttendance = attendance.Where(x => x.EmployeeId == employee.Id).ToList();
                var absentDays = employeeAttendance.Count(x => x.Status == "Absent");
                var lateMinutes = employeeAttendance.Sum(x => x.LateMinutes + x.EarlyLeaveMinutes);

                var dailyRate = daysInMonth == 0 ? 0 : employee.BasicSalary / daysInMonth;
                var minuteRate = dailyRate / 8m / 60m;
                var attendanceDeduction = (absentDays * dailyRate) + (lateMinutes * minuteRate);

                var employeeStructures = structures.Where(x => x.EmployeeId == employee.Id).ToList();
                decimal additions = 0;
                decimal otherDeductions = 0;

                var payrollEmployeeId = Guid.NewGuid();
                foreach (var structure in employeeStructures)
                {
                    var itemType = structure.SalaryItemDefinition?.ItemType ?? "Addition";
                    if (itemType == "Addition") additions += structure.Value; else otherDeductions += structure.Value;

                    payrollItems.Add(new PayrollEmployeeItem
                    {
                        Id = Guid.NewGuid(),
                        PayrollEmployeeId = payrollEmployeeId,
                        SalaryItemDefinitionId = structure.SalaryItemDefinitionId,
                        ItemType = itemType,
                        Value = structure.Value,
                        Notes = structure.Notes
                    });
                }

                var gross = employee.BasicSalary + additions;
                var totalDeductions = attendanceDeduction + otherDeductions;
                var net = gross - totalDeductions;
                if (net < 0) net = 0;

                payrollEmployees.Add(new PayrollEmployee
                {
                    Id = payrollEmployeeId,
                    PayrollMonthId = payrollMonthId,
                    EmployeeId = employee.Id,
                    BasicSalary = employee.BasicSalary,
                    AttendanceDeduction = Math.Round(attendanceDeduction, 2),
                    OtherDeductions = Math.Round(otherDeductions, 2),
                    Additions = Math.Round(additions, 2),
                    GrossAmount = Math.Round(gross, 2),
                    TotalDeductions = Math.Round(totalDeductions, 2),
                    NetAmount = Math.Round(net, 2),
                    Notes = absentDays > 0 || lateMinutes > 0 ? $"غياب: {absentDays} يوم، تأخير/انصراف مبكر: {lateMinutes} دقيقة" : null,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            _db.Set<PayrollEmployee>().AddRange(payrollEmployees);
            _db.Set<PayrollEmployeeItem>().AddRange(payrollItems);
            payrollMonth.Status = "Calculated";
            await _db.SaveChangesAsync();
        }
    }
}
