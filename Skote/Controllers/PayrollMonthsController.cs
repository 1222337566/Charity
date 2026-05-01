using InfrastrfuctureManagmentCore.Domains.Payroll;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Payroll;
using InfrastructureManagmentWebFramework.Models.Payroll.Months;
using Microsoft.AspNetCore.Mvc;

public class PayrollMonthsController : Controller
{
    private readonly IPayrollMonthRepository _payrollMonthRepository;
    private readonly IPayrollEmployeeRepository _payrollEmployeeRepository;
    private readonly IOperationalJournalHookService _journalHookService;

    public PayrollMonthsController(
        IPayrollMonthRepository payrollMonthRepository,
        IPayrollEmployeeRepository payrollEmployeeRepository,
        IOperationalJournalHookService journalHookService)
    {
        _payrollMonthRepository = payrollMonthRepository;
        _payrollEmployeeRepository = payrollEmployeeRepository;
        _journalHookService = journalHookService;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _payrollMonthRepository.GetAllAsync();
        return View(items.Select(x => new PayrollMonthListItemVm
        {
            Id = x.Id,
            Year = x.Year,
            Month = x.Month,
            Status = x.Status,
            EmployeesCount = x.Employees.Count,
            TotalNet = x.Employees.Sum(e => e.NetAmount)
        }).ToList());
    }

    [HttpGet]
    public IActionResult Create() => View(new CreatePayrollMonthVm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePayrollMonthVm vm)
    {
        if (await _payrollMonthRepository.ExistsAsync(vm.Year, vm.Month))
            ModelState.AddModelError(string.Empty, "هذا الشهر موجود بالفعل.");
        if (!ModelState.IsValid) return View(vm);

        await _payrollMonthRepository.AddAsync(new PayrollMonth
        {
            Id = Guid.NewGuid(),
            Year = vm.Year,
            Month = vm.Month,
            Status = "Draft"
        });
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate(Guid id)
    {
        await _payrollEmployeeRepository.GenerateForMonthAsync(id);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(Guid id)
    {
        var entity = await _payrollMonthRepository.GetByIdAsync(id);
        if (entity == null) return NotFound();
        entity.Status = "Approved";
        entity.ApprovedAtUtc = DateTime.UtcNow;
        entity.ApprovedByUserId = User?.Identity?.Name;
        await _payrollMonthRepository.UpdateAsync(entity);

        var postingResult = await _journalHookService.TryCreatePayrollEntryAsync(id);
        TempData[postingResult.IsSuccess ? "Success" : "Warning"] = postingResult.IsSuccess
            ? $"تم اعتماد شهر المرتبات. {postingResult.Message}"
            : postingResult.Message;

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var entity = await _payrollMonthRepository.GetByIdAsync(id);
        if (entity == null) return NotFound();

        return View(new PayrollMonthDetailsVm
        {
            Id = entity.Id,
            Year = entity.Year,
            Month = entity.Month,
            Status = entity.Status,
            EmployeesCount = entity.Employees.Count,
            TotalBasic = entity.Employees.Sum(x => x.BasicSalary),
            TotalAdditions = entity.Employees.Sum(x => x.Additions),
            TotalDeductions = entity.Employees.Sum(x => x.TotalDeductions),
            TotalNet = entity.Employees.Sum(x => x.NetAmount)
        });
    }
}
