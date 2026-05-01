using InfrastrfuctureManagmentCore.Domains.HR;
using InfrastrfuctureManagmentCore.Persistence.Repositories.HR;
using InfrastructureManagmentWebFramework.Models.HR.Attendance;
using InfrastructureManagmentWebFramework.Models.HR.EmployeeProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Skote.Helpers;
[Authorize(Policy = CharityPolicies.HrManage)]
public class HrAttendanceRecordsController : Controller
{
    private readonly IHrAttendanceRecordRepository _attendanceRepository;
    private readonly IHrEmployeeRepository _employeeRepository;
    private readonly IHrShiftRepository _shiftRepository;

    public HrAttendanceRecordsController(
        IHrAttendanceRecordRepository attendanceRepository,
        IHrEmployeeRepository employeeRepository,
        IHrShiftRepository shiftRepository)
    {
        _attendanceRepository = attendanceRepository;
        _employeeRepository = employeeRepository;
        _shiftRepository = shiftRepository;
    }

    public async Task<IActionResult> Index(AttendanceListFilterVm filter)
    {
        await FillFilterLookupsAsync(filter);
        var items = await _attendanceRepository.SearchAsync(filter.EmployeeId, filter.FromDate, filter.ToDate, filter.Status);

        if (filter.EmployeeId.HasValue)
        {
            var employee = await _employeeRepository.GetByIdAsync(filter.EmployeeId.Value);
            if (employee != null)
            {
                ViewBag.EmployeeHeader = new EmployeeHeaderVm
                {
                    Id = employee.Id,
                    Code = employee.Code,
                    FullName = employee.FullName,
                    DepartmentName = employee.Department?.Name,
                    JobTitleName = employee.JobTitle?.Name,
                    Status = employee.Status,
                    PhoneNumber = employee.PhoneNumber,
                    HireDate = employee.HireDate,
                    BasicSalary = employee.BasicSalary,
                    IsActive = employee.IsActive
                };
            }
        }

        return View(new AttendanceListPageVm
        {
            Filter = filter,
            Items = items.Select(x => new AttendanceListRowVm
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                EmployeeName = x.Employee?.FullName ?? string.Empty,
                DepartmentName = x.Employee?.Department?.Name,
                AttendanceDate = x.AttendanceDate,
                CheckInTime = x.CheckInTime,
                CheckOutTime = x.CheckOutTime,
                ShiftName = x.Shift?.Name,
                WorkedHours = x.WorkedHours,
                LateMinutes = x.LateMinutes,
                EarlyLeaveMinutes = x.EarlyLeaveMinutes,
                OvertimeMinutes = x.OvertimeMinutes,
                Status = x.Status,
                Source = x.Source
            }).ToList()
        });
    }
    [Authorize(Policy = CharityPolicies.HrView)]
    [HttpGet]
    public async Task<IActionResult> Create(Guid? employeeId = null)
    {
        var vm = new CreateAttendanceRecordVm { EmployeeId = employeeId ?? Guid.Empty };
        await FillLookupsAsync(vm);
        return View(vm);
    }
    [Authorize(Policy = CharityPolicies.HrView)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateAttendanceRecordVm vm)
    {
        await FillLookupsAsync(vm);
        if (!ModelState.IsValid)
            return View(vm);

        var entity = new HrAttendanceRecord
        {
            Id = Guid.NewGuid(),
            EmployeeId = vm.EmployeeId,
            AttendanceDate = vm.AttendanceDate,
            CheckInTime = vm.CheckInTime,
            CheckOutTime = vm.CheckOutTime,
            ShiftId = vm.ShiftId,
            Status = vm.Status,
            Source = vm.Source,
            Notes = vm.Notes
        };

        if (vm.ShiftId.HasValue)
        {
            var shift = await _shiftRepository.GetByIdAsync(vm.ShiftId.Value);
            if (shift != null)
                ApplyCalculatedTimes(entity, shift);
        }
        else
        {
            ApplyWorkedHoursOnly(entity);
        }

        await _attendanceRepository.AddAsync(entity);
        return RedirectToAction(nameof(Index), new { employeeId = vm.EmployeeId });
    }
    [Authorize(Policy = CharityPolicies.HrView)]
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _attendanceRepository.GetByIdAsync(id);
        if (entity == null) return NotFound();

        var vm = new EditAttendanceRecordVm
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            AttendanceDate = entity.AttendanceDate,
            CheckInTime = entity.CheckInTime,
            CheckOutTime = entity.CheckOutTime,
            ShiftId = entity.ShiftId,
            Status = entity.Status,
            Source = entity.Source,
            Notes = entity.Notes
        };
        await FillLookupsAsync(vm);
        return View(vm);
    }
    [Authorize(Policy = CharityPolicies.HrView)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditAttendanceRecordVm vm)
    {
        var entity = await _attendanceRepository.GetByIdAsync(vm.Id);
        if (entity == null) return NotFound();

        await FillLookupsAsync(vm);
        if (!ModelState.IsValid)
            return View(vm);

        entity.EmployeeId = vm.EmployeeId;
        entity.AttendanceDate = vm.AttendanceDate;
        entity.CheckInTime = vm.CheckInTime;
        entity.CheckOutTime = vm.CheckOutTime;
        entity.ShiftId = vm.ShiftId;
        entity.Status = vm.Status;
        entity.Source = vm.Source;
        entity.Notes = vm.Notes;
        entity.LateMinutes = 0;
        entity.EarlyLeaveMinutes = 0;
        entity.OvertimeMinutes = 0;
        entity.WorkedHours = 0;

        if (vm.ShiftId.HasValue)
        {
            var shift = await _shiftRepository.GetByIdAsync(vm.ShiftId.Value);
            if (shift != null)
                ApplyCalculatedTimes(entity, shift);
        }
        else
        {
            ApplyWorkedHoursOnly(entity);
        }

        await _attendanceRepository.UpdateAsync(entity);
        return RedirectToAction(nameof(Index), new { employeeId = vm.EmployeeId });
    }

    private async Task FillLookupsAsync(CreateAttendanceRecordVm vm)
    {
        var employees = await _employeeRepository.GetActiveAsync();
        var shifts = await _shiftRepository.GetActiveAsync();
        vm.Employees = employees.Select(x => new SelectListItem(x.FullName, x.Id.ToString())).ToList();
        vm.Shifts = shifts.Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();
        vm.Statuses = GetStatuses();
        vm.Sources = GetSources();
    }

    private async Task FillFilterLookupsAsync(AttendanceListFilterVm vm)
    {
        var employees = await _employeeRepository.GetActiveAsync();
        vm.Employees = new List<SelectListItem> { new("الكل", "") };
        vm.Employees.AddRange(employees.Select(x => new SelectListItem(x.FullName, x.Id.ToString())));
        vm.Statuses = new List<SelectListItem> { new("الكل", "") };
        vm.Statuses.AddRange(GetStatuses());
    }

    private static List<SelectListItem> GetStatuses() => new()
    {
        new("حاضر", "Present"),
        new("غائب", "Absent"),
        new("إجازة", "Leave"),
        new("مأمورية", "Mission"),
        new("تأخير", "Late"),
        new("انصراف مبكر", "EarlyLeave")
    };

    private static List<SelectListItem> GetSources() => new()
    {
        new("يدوي", "Manual"),
        new("بصمة", "Biometric"),
        new("استيراد", "Import")
    };

    private static void ApplyCalculatedTimes(HrAttendanceRecord entity, HrShift shift)
    {
        ApplyWorkedHoursOnly(entity);

        if (entity.CheckInTime.HasValue)
        {
            var delay = entity.CheckInTime.Value - shift.StartTime - TimeSpan.FromMinutes(shift.GraceMinutes);
            entity.LateMinutes = delay > TimeSpan.Zero ? (int)Math.Floor(delay.TotalMinutes) : 0;
        }

        if (entity.CheckOutTime.HasValue)
        {
            var early = shift.EndTime - entity.CheckOutTime.Value;
            entity.EarlyLeaveMinutes = early > TimeSpan.Zero ? (int)Math.Floor(early.TotalMinutes) : 0;

            var overtime = entity.CheckOutTime.Value - shift.EndTime;
            entity.OvertimeMinutes = overtime > TimeSpan.Zero ? (int)Math.Floor(overtime.TotalMinutes) : 0;
        }
    }

    private static void ApplyWorkedHoursOnly(HrAttendanceRecord entity)
    {
        if (entity.CheckInTime.HasValue && entity.CheckOutTime.HasValue && entity.CheckOutTime.Value > entity.CheckInTime.Value)
            entity.WorkedHours = Math.Round((decimal)(entity.CheckOutTime.Value - entity.CheckInTime.Value).TotalHours, 2);
        else
            entity.WorkedHours = 0;
    }
}
