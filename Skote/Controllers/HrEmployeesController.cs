using InfrastrfuctureManagmentCore.Domains.HR;
using InfrastrfuctureManagmentCore.Persistence.Repositories.HR;
using InfrastructureManagmentWebFramework.Models.HR.EmployeeProfile;
using InfrastructureManagmentWebFramework.Models.HR.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Skote.Helpers;
[Authorize(Policy = CharityPolicies.HrManage)]
public class HrEmployeesController : Controller
{
    private readonly IHrEmployeeRepository _employeeRepository;
    private readonly IHrDepartmentRepository _departmentRepository;
    private readonly IHrJobTitleRepository _jobTitleRepository;

    public HrEmployeesController(
        IHrEmployeeRepository employeeRepository,
        IHrDepartmentRepository departmentRepository,
        IHrJobTitleRepository jobTitleRepository)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _jobTitleRepository = jobTitleRepository;
    }

    public async Task<IActionResult> Index(EmployeeListFilterVm filter)
    {
        FillStatusLookups(filter);
        var items = await _employeeRepository.SearchAsync(filter.Q, filter.Status, filter.IsActive);
        return View(new EmployeeListPageVm
        {
            Filter = filter,
            Items = items.Select(x => new EmployeeListRowVm
            {
                Id = x.Id,
                Code = x.Code,
                FullName = x.FullName,
                DepartmentName = x.Department?.Name,
                JobTitleName = x.JobTitle?.Name,
                HireDate = x.HireDate,
                BasicSalary = x.BasicSalary,
                Status = x.Status,
                IsActive = x.IsActive
            }).ToList()
        });
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var entity = await _employeeRepository.GetByIdAsync(id);
        if (entity == null) return NotFound();

        ViewBag.EmployeeHeader = new EmployeeHeaderVm
        {
            Id = entity.Id,
            Code = entity.Code,
            FullName = entity.FullName,
            DepartmentName = entity.Department?.Name,
            JobTitleName = entity.JobTitle?.Name,
            Status = entity.Status,
            PhoneNumber = entity.PhoneNumber,
            HireDate = entity.HireDate,
            BasicSalary = entity.BasicSalary,
            IsActive = entity.IsActive
        };

        return View(new EmployeeDetailsVm
        {
            Id = entity.Id,
            Code = entity.Code,
            FullName = entity.FullName,
            NationalId = entity.NationalId,
            BirthDate = entity.BirthDate,
            PhoneNumber = entity.PhoneNumber,
            Email = entity.Email,
            AddressLine = entity.AddressLine,
            DepartmentName = entity.Department?.Name,
            JobTitleName = entity.JobTitle?.Name,
            HireDate = entity.HireDate,
            EmploymentType = entity.EmploymentType,
            BasicSalary = entity.BasicSalary,
            InsuranceSalary = entity.InsuranceSalary,
            Status = entity.Status,
            Notes = entity.Notes,
            IsActive = entity.IsActive,
            AttendanceRecordsCount = entity.AttendanceRecords.Count
        });
    }
    [Authorize(Policy = CharityPolicies.HrView)]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new CreateHrEmployeeVm();
        await FillLookupsAsync(vm);
        return View(vm);
    }
    [Authorize(Policy = CharityPolicies.HrView)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateHrEmployeeVm vm)
    {
        await FillLookupsAsync(vm);
        if (await _employeeRepository.CodeExistsAsync(vm.Code))
            ModelState.AddModelError(nameof(vm.Code), "الكود مستخدم من قبل.");
        if (!ModelState.IsValid)
            return View(vm);

        await _employeeRepository.AddAsync(new HrEmployee
        {
            Id = Guid.NewGuid(),
            Code = vm.Code.Trim(),
            FullName = vm.FullName.Trim(),
            NationalId = vm.NationalId,
            BirthDate = vm.BirthDate,
            PhoneNumber = vm.PhoneNumber,
            Email = vm.Email,
            AddressLine = vm.AddressLine,
            DepartmentId = vm.DepartmentId,
            JobTitleId = vm.JobTitleId,
            HireDate = vm.HireDate,
            EmploymentType = vm.EmploymentType,
            BasicSalary = vm.BasicSalary,
            InsuranceSalary = vm.InsuranceSalary,
            BankName = vm.BankName,
            BankAccountNumber = vm.BankAccountNumber,
            Status = vm.Status,
            Notes = vm.Notes,
            IsActive = vm.IsActive
        });
        return RedirectToAction(nameof(Index));
    }
    [Authorize(Policy = CharityPolicies.HrView)]
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _employeeRepository.GetByIdAsync(id);
        if (entity == null) return NotFound();
        var vm = new EditHrEmployeeVm
        {
            Id = entity.Id,
            Code = entity.Code,
            FullName = entity.FullName,
            NationalId = entity.NationalId,
            BirthDate = entity.BirthDate,
            PhoneNumber = entity.PhoneNumber,
            Email = entity.Email,
            AddressLine = entity.AddressLine,
            DepartmentId = entity.DepartmentId,
            JobTitleId = entity.JobTitleId,
            HireDate = entity.HireDate,
            EmploymentType = entity.EmploymentType,
            BasicSalary = entity.BasicSalary,
            InsuranceSalary = entity.InsuranceSalary,
            BankName = entity.BankName,
            BankAccountNumber = entity.BankAccountNumber,
            Status = entity.Status,
            Notes = entity.Notes,
            IsActive = entity.IsActive
        };
        await FillLookupsAsync(vm);
        return View(vm);
    }
    [Authorize(Policy = CharityPolicies.HrView)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditHrEmployeeVm vm)
    {
        var entity = await _employeeRepository.GetByIdAsync(vm.Id);
        if (entity == null) return NotFound();
        await FillLookupsAsync(vm);
        if (await _employeeRepository.CodeExistsAsync(vm.Code, vm.Id))
            ModelState.AddModelError(nameof(vm.Code), "الكود مستخدم من قبل.");
        if (!ModelState.IsValid)
            return View(vm);

        entity.Code = vm.Code.Trim();
        entity.FullName = vm.FullName.Trim();
        entity.NationalId = vm.NationalId;
        entity.BirthDate = vm.BirthDate;
        entity.PhoneNumber = vm.PhoneNumber;
        entity.Email = vm.Email;
        entity.AddressLine = vm.AddressLine;
        entity.DepartmentId = vm.DepartmentId;
        entity.JobTitleId = vm.JobTitleId;
        entity.HireDate = vm.HireDate;
        entity.EmploymentType = vm.EmploymentType;
        entity.BasicSalary = vm.BasicSalary;
        entity.InsuranceSalary = vm.InsuranceSalary;
        entity.BankName = vm.BankName;
        entity.BankAccountNumber = vm.BankAccountNumber;
        entity.Status = vm.Status;
        entity.Notes = vm.Notes;
        entity.IsActive = vm.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        await _employeeRepository.UpdateAsync(entity);
        return RedirectToAction(nameof(Index));
    }

    private async Task FillLookupsAsync(CreateHrEmployeeVm vm)
    {
        var departments = await _departmentRepository.GetActiveAsync();
        var jobTitles = await _jobTitleRepository.GetActiveAsync();
        vm.Departments = departments.Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();
        vm.JobTitles = jobTitles.Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();
        vm.EmploymentTypes = GetEmploymentTypes();
        vm.Statuses = GetStatuses();
    }

    private void FillStatusLookups(EmployeeListFilterVm vm)
    {
        vm.Statuses = new List<SelectListItem>
        {
            new("الكل", ""),
            new("نشط", "Active"),
            new("موقوف", "Suspended"),
            new("منتهي الخدمة", "Terminated")
        };
    }

    private static List<SelectListItem> GetEmploymentTypes() => new()
    {
        new("دائم", "Permanent"),
        new("مؤقت", "Temporary"),
        new("متعاقد", "Contract"),
        new("دوام جزئي", "PartTime")
    };

    private static List<SelectListItem> GetStatuses() => new()
    {
        new("نشط", "Active"),
        new("موقوف", "Suspended"),
        new("منتهي الخدمة", "Terminated")
    };
}
