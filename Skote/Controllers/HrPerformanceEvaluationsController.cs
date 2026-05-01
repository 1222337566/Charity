using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.HR.Advanced.Evaluations;
using InfrastrfuctureManagmentCore.Domains.HR;
using InfrastrfuctureManagmentCore.Domains.HR.Advanced;
using InfrastrfuctureManagmentCore.Persistence.Repositories.HR.Advanced;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class HrPerformanceEvaluationsController : Controller
{
    private readonly IHrPerformanceEvaluationRepository _repository;
    private readonly AppDbContext _db;
    public HrPerformanceEvaluationsController(IHrPerformanceEvaluationRepository repository, AppDbContext db) { _repository = repository; _db = db; }

    public async Task<IActionResult> Index(Guid? employeeId)
    {
        var items = await _repository.GetAllAsync(employeeId);
        ViewBag.EmployeeId = employeeId;
        return View(items.Select(x => new PerformanceEvaluationListItemVm
        {
            Id = x.Id,
            EmployeeId = x.EmployeeId,
            EmployeeName = x.Employee?.FullName ?? "-",
            EvaluationPeriod = x.EvaluationPeriod,
            EvaluationDate = x.EvaluationDate,
            TotalScore = x.TotalScore,
            Result = x.Result,
            EvaluatorName = x.EvaluatorEmployee?.FullName
        }).ToList());
    }

    public async Task<IActionResult> Create(Guid? employeeId)
    {
        var vm = new CreatePerformanceEvaluationVm { EmployeeId = employeeId ?? Guid.Empty };
        await FillLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePerformanceEvaluationVm vm)
    {
        await FillLookupsAsync(vm);
        if (!ModelState.IsValid) return View(vm);

        var total = vm.DisciplineScore + vm.PerformanceScore + vm.CooperationScore + vm.InitiativeScore;
        await _repository.AddAsync(new HrPerformanceEvaluation
        {
            Id = Guid.NewGuid(),
            EmployeeId = vm.EmployeeId,
            EvaluatorEmployeeId = vm.EvaluatorEmployeeId,
            EvaluationPeriod = vm.EvaluationPeriod.Trim(),
            EvaluationDate = vm.EvaluationDate,
            DisciplineScore = vm.DisciplineScore,
            PerformanceScore = vm.PerformanceScore,
            CooperationScore = vm.CooperationScore,
            InitiativeScore = vm.InitiativeScore,
            TotalScore = total,
            Result = vm.Result,
            Notes = vm.Notes?.Trim(),
            CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            CreatedAtUtc = DateTime.UtcNow
        });

        TempData["Success"] = "تم تسجيل التقييم";
        return RedirectToAction(nameof(Index), new { employeeId = vm.EmployeeId });
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return NotFound();
        var vm = new EditPerformanceEvaluationVm
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            EvaluatorEmployeeId = entity.EvaluatorEmployeeId,
            EvaluationPeriod = entity.EvaluationPeriod,
            EvaluationDate = entity.EvaluationDate,
            DisciplineScore = entity.DisciplineScore,
            PerformanceScore = entity.PerformanceScore,
            CooperationScore = entity.CooperationScore,
            InitiativeScore = entity.InitiativeScore,
            Result = entity.Result,
            Notes = entity.Notes
        };
        await FillLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditPerformanceEvaluationVm vm)
    {
        await FillLookupsAsync(vm);
        if (!ModelState.IsValid) return View(vm);

        var entity = await _repository.GetByIdAsync(vm.Id);
        if (entity == null) return NotFound();
        entity.EmployeeId = vm.EmployeeId;
        entity.EvaluatorEmployeeId = vm.EvaluatorEmployeeId;
        entity.EvaluationPeriod = vm.EvaluationPeriod.Trim();
        entity.EvaluationDate = vm.EvaluationDate;
        entity.DisciplineScore = vm.DisciplineScore;
        entity.PerformanceScore = vm.PerformanceScore;
        entity.CooperationScore = vm.CooperationScore;
        entity.InitiativeScore = vm.InitiativeScore;
        entity.TotalScore = vm.DisciplineScore + vm.PerformanceScore + vm.CooperationScore + vm.InitiativeScore;
        entity.Result = vm.Result;
        entity.Notes = vm.Notes?.Trim();
        await _repository.UpdateAsync(entity);

        TempData["Success"] = "تم تعديل التقييم";
        return RedirectToAction(nameof(Index), new { employeeId = vm.EmployeeId });
    }

    private async Task FillLookupsAsync(CreatePerformanceEvaluationVm vm)
    {
        var employees = await _db.Set<HrEmployee>().AsNoTracking().OrderBy(x => x.FullName)
            .Select(x => new SelectListItem(x.FullName, x.Id.ToString())).ToListAsync();
        vm.Employees = employees;
        vm.Results = new()
        {
            new("ممتاز", "Excellent"), new("جيد جدًا", "VeryGood"), new("جيد", "Good"), new("مقبول", "Average"), new("ضعيف", "Poor")
        };
    }
}
