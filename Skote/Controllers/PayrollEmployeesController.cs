using InfrastrfuctureManagmentCore.Persistence.Repositories.Payroll;
using InfrastructureManagmentWebFramework.Models.Payroll.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skote.Helpers;
[Authorize(Policy = CharityPolicies.PayrollView)]
public class PayrollEmployeesController : Controller
{
    private readonly IPayrollEmployeeRepository _repository;
    private readonly IPayrollMonthRepository _payrollMonthRepository;

    public PayrollEmployeesController(IPayrollEmployeeRepository repository, IPayrollMonthRepository payrollMonthRepository)
    {
        _repository = repository;
        _payrollMonthRepository = payrollMonthRepository;
    }

    public async Task<IActionResult> Index(Guid payrollMonthId)
    {
        var payrollMonth = await _payrollMonthRepository.GetByIdAsync(payrollMonthId);
        if (payrollMonth == null) return NotFound();
        ViewBag.PayrollMonth = payrollMonth;

        var items = await _repository.GetByPayrollMonthIdAsync(payrollMonthId);
        return View(items.Select(x => new PayrollEmployeeListItemVm
        {
            Id = x.Id,
            EmployeeId = x.EmployeeId,
            EmployeeCode = x.Employee?.Code ?? string.Empty,
            EmployeeName = x.Employee?.FullName ?? string.Empty,
            BasicSalary = x.BasicSalary,
            Additions = x.Additions,
            TotalDeductions = x.TotalDeductions,
            NetAmount = x.NetAmount,
            Notes = x.Notes
        }).ToList());
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return NotFound();
        return View(new PayrollEmployeeDetailsVm
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            EmployeeCode = entity.Employee?.Code ?? string.Empty,
            EmployeeName = entity.Employee?.FullName ?? string.Empty,
            MonthName = $"{entity.PayrollMonth?.Year}/{entity.PayrollMonth?.Month:00}",
            BasicSalary = entity.BasicSalary,
            Additions = entity.Additions,
            AttendanceDeduction = entity.AttendanceDeduction,
            OtherDeductions = entity.OtherDeductions,
            TotalDeductions = entity.TotalDeductions,
            NetAmount = entity.NetAmount,
            Items = entity.Items.Select(x => new PayrollEmployeeDetailsItemVm
            {
                Name = x.SalaryItemDefinition?.Name,
                ItemType = x.ItemType,
                Value = x.Value,
                Notes = x.Notes
            }).ToList()
        });
    }
}
