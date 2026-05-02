using InfrastrfuctureManagmentCore.Domains.Expenses;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Expenses;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Payments;
using InfrastructureManagmentWebFramework.Models.Expenses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class ExpensesController : Controller
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly IOperationalJournalHookService _journalHookService;

    public ExpensesController(
        IExpenseRepository expenseRepository,
        IExpenseCategoryRepository expenseCategoryRepository,
        IPaymentMethodRepository paymentMethodRepository,
        IOperationalJournalHookService journalHookService)
    {
        _expenseRepository = expenseRepository;
        _expenseCategoryRepository = expenseCategoryRepository;
        _paymentMethodRepository = paymentMethodRepository;
        _journalHookService = journalHookService;
    }

    public async Task<IActionResult> Index()
    {
        var expenses = await _expenseRepository.GetAllAsync();

        var model = expenses.Select(x => new ExpenseListItemVm
        {
            Id = x.Id,
            ExpenseNumber = x.ExpenseNumber,
            ExpenseDateUtc = x.ExpenseDateUtc,
            CategoryName = x.ExpenseCategory?.NameAr ?? "-",
            Description = x.Description,
            PaymentMethodName = x.PaymentMethod?.MethodNameAr,
            Amount = x.Amount
        }).ToList();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new CreateExpenseVm
        {
            ExpenseNumber = "EXP-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
            ExpenseDateUtc = DateTime.UtcNow
        };

        await FillLookups(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateExpenseVm vm)
    {
        await FillLookups(vm);

        if (!ModelState.IsValid)
            return View(vm);

        if (await _expenseRepository.ExpenseNumberExistsAsync(vm.ExpenseNumber.Trim()))
        {
            ModelState.AddModelError(nameof(vm.ExpenseNumber), "رقم المصروف موجود بالفعل");
            return View(vm);
        }

        var entity = new Expensex
        {
            Id = Guid.NewGuid(),
            ExpenseNumber = vm.ExpenseNumber.Trim(),
            ExpenseDateUtc = vm.ExpenseDateUtc,
            ExpenseCategoryId = vm.ExpenseCategoryId,
            PaymentMethodId = vm.PaymentMethodId,
            Amount = vm.Amount,
            Description = vm.Description.Trim(),
            Notes = vm.Notes?.Trim(),
            IsPosted = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _expenseRepository.AddAsync(entity);

        var postingResult = await _journalHookService.TryCreateExpenseEntryAsync(entity.Id);
        TempData[postingResult.IsSuccess ? "Success" : "Warning"] = postingResult.IsSuccess
            ? $"تم تسجيل المصروف بنجاح. {postingResult.Message}"
            : postingResult.Message;

        return RedirectToAction(nameof(Index));
    }

    private async Task FillLookups(CreateExpenseVm vm)
    {
        var categories = await _expenseCategoryRepository.GetActiveAsync();
        var methods = await _paymentMethodRepository.GetActiveAsync();

        vm.ExpenseCategories = categories.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.CategoryCode} - {x.NameAr}"
        }).ToList();

        vm.PaymentMethods = methods.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.MethodCode} - {x.MethodNameAr}"
        }).ToList();
    }
}
