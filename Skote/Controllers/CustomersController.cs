using InfrastrfuctureManagmentCore.Domains.Customer;
using InfrastrfuctureManagmentCore.Domains.Optics;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Customer;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Optics;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Prescriptions;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Sale;
using InfrastructureManagmentCore.Domains.Profiling;
using InfrastructureManagmentServices.Customers;
using InfrastructureManagmentWebFramework.Models.Customer;
using InfrastructureManagmentWebFramework.Models.Optics;
using InfrastructureManagmentWebFramework.Models.Optics.Customers;
using InfrastructureManagmentWebFramework.Models.Optics.OldRecords;
using InfrastructureManagmentWebFramework.Models.SoldItems;
using Microsoft.AspNetCore.Mvc;

public class CustomersController : Controller
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IPrescriptionRepository _prescriptionRepository;
    private readonly ISalesInvoiceRepository _salesInvoiceRepository;
    private readonly ICustomerAccountTransactionRepository _customerAccountTransactionRepository;
    private readonly ICustomerAccountService _customerAccountService;
    private readonly ICustomerOldRecordRepository _customerOldRecordRepository;
    private readonly IOpticalWorkOrderRepository _opticalWorkOrderRepository;
    public CustomersController(ICustomerRepository customerRepository,IPrescriptionRepository prescriptionRepository,
        ISalesInvoiceRepository salesInvoiceRepository,
        ICustomerAccountService customerAccountService,
        ICustomerAccountTransactionRepository customerAccountTransactionRepository, ICustomerOldRecordRepository customerOldRecordRepository,
        IOpticalWorkOrderRepository opticalWorkOrderRepository)
    {
        _customerRepository = customerRepository;
        _customerAccountService = customerAccountService;
        _customerAccountTransactionRepository = customerAccountTransactionRepository;
        _prescriptionRepository = prescriptionRepository;
        _opticalWorkOrderRepository = opticalWorkOrderRepository;
        _salesInvoiceRepository = salesInvoiceRepository;
        _customerOldRecordRepository = customerOldRecordRepository;

    }

    public async Task<IActionResult> Index(string? q, bool? isActive)
    {
        var customers = await _customerRepository.SearchAsync(q, isActive);

        var rows = new List<CustomerListRowVm>();

        foreach (var customer in customers)
        {
            var prescriptions = await _prescriptionRepository.GetByCustomerIdAsync(customer.Id);
            var soldItems = await _salesInvoiceRepository.GetSoldItemsByCustomerIdAsync(customer.Id);
            var balance = await _customerAccountService.GetBalanceAsync(customer.Id);
            var workOrders = await _opticalWorkOrderRepository.GetByCustomerIdAsync(customer.Id);

            rows.Add(new CustomerListRowVm
            {
                Id = customer.Id,
                CustomerNumber = customer.CustomerNumber,
                NameAr = customer.NameAr,
                MobileNo = customer.MobileNo,
                Tel = customer.Tel,
                LastPrescriptionDateUtc = prescriptions.Any()
                    ? prescriptions.Max(x => x.PrescriptionDateUtc)
                    : null,
                SoldItemsCount = soldItems.Count,
                AccountBalance = balance,
                OpenWorkOrdersCount = workOrders.Count(x =>
                    x.Status != OpticalWorkOrderStatus.Delivered &&
                    x.Status != OpticalWorkOrderStatus.Cancelled),
                IsActive = customer.IsActive
            });
        }

        var vm = new CustomerListPageVm
        {
            Filter = new CustomerListFilterVm
            {
                Q = q,
                IsActive = isActive
            },
            Rows = rows,
            TotalCount = rows.Count,
            ActiveCount = rows.Count(x => x.IsActive),
            HasBalanceCount = rows.Count(x => x.AccountBalance != 0)
        };

        return View(vm);
    }
    [HttpGet]
    public IActionResult Create()
    {
        var vm = new CreateCustomerVm
        {
            CustomerNumber = GenerateCustomerNumber()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCustomerVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        if (await _customerRepository.NumberExistsAsync(vm.CustomerNumber.Trim()))
        {
            ModelState.AddModelError(nameof(vm.CustomerNumber), "رقم العميل موجود بالفعل");
            return View(vm);
        }

        var entity = new CustomerClient
        {
            Id = Guid.NewGuid(),
            CustomerNumber = vm.CustomerNumber.Trim(),
            NameAr = vm.NameAr.Trim(),
            NameEn = vm.NameEn?.Trim(),
            Gender = vm.Gender,
            Age = vm.Age,
            Tel = vm.Tel?.Trim(),
            MobileNo = vm.MobileNo?.Trim(),
            Remarks = vm.Remarks?.Trim(),
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _customerRepository.AddAsync(entity);

        TempData["Success"] = "تم إضافة العميل بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _customerRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var vm = new EditCustomerVm
        {
            Id = entity.Id,
            CustomerNumber = entity.CustomerNumber,
            NameAr = entity.NameAr,
            NameEn = entity.NameEn,
            Gender = entity.Gender,
            Age = entity.Age,
            Tel = entity.Tel,
            MobileNo = entity.MobileNo,
            Remarks = entity.Remarks,
            IsActive = entity.IsActive
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditCustomerVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var entity = await _customerRepository.GetByIdAsync(vm.Id);
        if (entity == null)
            return NotFound();

        if (await _customerRepository.NumberExistsAsync(vm.CustomerNumber.Trim(), vm.Id))
        {
            ModelState.AddModelError(nameof(vm.CustomerNumber), "رقم العميل موجود بالفعل");
            return View(vm);
        }

        entity.CustomerNumber = vm.CustomerNumber.Trim();
        entity.NameAr = vm.NameAr.Trim();
        entity.NameEn = vm.NameEn?.Trim();
        entity.Gender = vm.Gender;
        entity.Age = vm.Age;
        entity.Tel = vm.Tel?.Trim();
        entity.MobileNo = vm.MobileNo?.Trim();
        entity.Remarks = vm.Remarks?.Trim();
        entity.IsActive = vm.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _customerRepository.UpdateAsync(entity);

        TempData["Success"] = "تم تعديل العميل بنجاح";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var entity = await _customerRepository.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var accountTransactions = await _customerAccountTransactionRepository.GetByCustomerIdAsync(entity.Id);
        var accountBalance = await _customerAccountService.GetBalanceAsync(entity.Id);

        decimal runningBalance = 0;
        var accountEntries = accountTransactions
            .OrderBy(x => x.TransactionDateUtc)
            .ThenBy(x => x.CreatedAtUtc)
            .Select(x =>
            {
                runningBalance += x.DebitAmount - x.CreditAmount;

                return new CustomerAccountEntryVm
                {
                    TransactionDateUtc = x.TransactionDateUtc,
                    TransactionTypeText = x.TransactionType switch
                    {
                        CustomerAccountTransactionType.SaleInvoice => "فاتورة بيع",
                        CustomerAccountTransactionType.SalesReturn => "مرتجع مبيعات",
                        CustomerAccountTransactionType.Receipt => "سند قبض",
                        CustomerAccountTransactionType.OpeningDebit => "رصيد افتتاحي مدين",
                        CustomerAccountTransactionType.OpeningCredit => "رصيد افتتاحي دائن",
                        CustomerAccountTransactionType.AdjustmentDebit => "تسوية مدين",
                        CustomerAccountTransactionType.AdjustmentCredit => "تسوية دائن",
                        _ => "-"
                    },
                    ReferenceNumber = x.ReferenceNumber,
                    Description = x.Description,
                    DebitAmount = x.DebitAmount,
                    CreditAmount = x.CreditAmount,
                    RunningBalance = runningBalance
                };
            }).ToList();

        var vm = new CustomerDetailsVm
        {
            Id = entity.Id,
            CustomerNumber = entity.CustomerNumber,
            NameAr = entity.NameAr,
            GenderText = entity.Gender == CustomerGender.Male ? "ذكر" : "أنثى",
            Age = entity.Age,
            Tel = entity.Tel,
            MobileNo = entity.MobileNo,
            Remarks = entity.Remarks,
            IsActive = entity.IsActive,
            AccountBalance = accountBalance,
            AccountEntries = accountEntries
        };

        return View(vm);
    }
    private static string GenerateCustomerNumber()
    {
        return DateTime.Now.ToString("yyMMddHHmmss");
    }
}