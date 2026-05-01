using InfrastrfuctureManagmentCore.Domains.Customer;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Customer;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Payments;
using InfrastructureManagmentServices.Customers;
using InfrastructureManagmentWebFramework.Models.Optics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class CustomerReceiptsController : Controller
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomerReceiptRepository _customerReceiptRepository;
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly ICustomerReceiptService _customerReceiptService;
    private readonly ICustomerAccountService _customerAccountService;

    public CustomerReceiptsController(
        ICustomerRepository customerRepository,
        ICustomerReceiptRepository customerReceiptRepository,
        IPaymentMethodRepository paymentMethodRepository,
        ICustomerReceiptService customerReceiptService,
        ICustomerAccountService customerAccountService)
    {
        _customerRepository = customerRepository;
        _customerReceiptRepository = customerReceiptRepository;
        _paymentMethodRepository = paymentMethodRepository;
        _customerReceiptService = customerReceiptService;
        _customerAccountService = customerAccountService;
    }

    public async Task<IActionResult> Index()
    {
        var receipts = await _customerReceiptRepository.GetAllAsync();

        var model = receipts.Select(x => new CustomerReceiptListItemVm
        {
            Id = x.Id,
            ReceiptNumber = x.ReceiptNumber,
            ReceiptDateUtc = x.ReceiptDateUtc,
            CustomerName = x.Customer?.NameAr ?? "-",
            PaymentMethodName = x.PaymentMethod?.MethodNameAr,
            Amount = x.Amount,
            StatusText = x.Status == CustomerReceiptStatus.Posted ? "مرحّل" : x.Status.ToString()
        }).ToList();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid customerId)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null)
            return NotFound();

        var vm = new CreateCustomerReceiptVm
        {
            ReceiptNumber = "REC-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
            ReceiptDateUtc = DateTime.UtcNow,
            CustomerId = customer.Id,
            CustomerNumber = customer.CustomerNumber,
            CustomerName = customer.NameAr,
            CurrentBalance = await _customerAccountService.GetBalanceAsync(customer.Id)
        };

        await FillLookups(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCustomerReceiptVm vm)
    {
        var customer = await _customerRepository.GetByIdAsync(vm.CustomerId);
        if (customer == null)
            return NotFound();

        vm.CustomerNumber = customer.CustomerNumber;
        vm.CustomerName = customer.NameAr;
        vm.CurrentBalance = await _customerAccountService.GetBalanceAsync(customer.Id);

        await FillLookups(vm);

        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            await _customerReceiptService.CreateAsync(vm);
            TempData["Success"] = "تم حفظ سند القبض بنجاح";
            return RedirectToAction("Details", "Customers", new { id = vm.CustomerId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(vm);
        }
    }

    private async Task FillLookups(CreateCustomerReceiptVm vm)
    {
        var methods = await _paymentMethodRepository.GetActiveAsync();

        vm.PaymentMethods = methods.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.MethodCode} - {x.MethodNameAr}"
        }).ToList();
    }
}