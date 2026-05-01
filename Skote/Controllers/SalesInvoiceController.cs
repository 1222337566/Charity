using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Funders;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.company;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Customer;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Payments;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Prescriptions;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Sale;
using InfrastructureManagmentCore.Models.Print;
using InfrastructureManagmentServices.Sales;
//using InfrastructureManagmentWebFramework.Models.Print;
using InfrastructureManagmentWebFramework.Models.Sale;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class SalesInvoicesController : Controller
{
    private readonly ISalesInvoiceRepository _salesInvoiceRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IItemRepository _itemRepository;
    private readonly ISalesService _salesService;
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly ICompanyProfileRepository _companyProfileRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IPrescriptionRepository _prescriptionRepository;
    private readonly ICharityProjectRepository _charityProjectRepository;
    private readonly IGrantAgreementRepository _grantAgreementRepository;
    public SalesInvoicesController(
        ISalesInvoiceRepository salesInvoiceRepository,
        IWarehouseRepository warehouseRepository,
        IItemRepository itemRepository,
        ISalesService salesService,
        ICompanyProfileRepository companyProfileRepository,
        ICustomerRepository customerRepository,
        IPaymentMethodRepository paymentMethodRepository,
        IPrescriptionRepository prescriptionRepository,
        ICharityProjectRepository charityProjectRepository,
        IGrantAgreementRepository grantAgreementRepository)
    {
        _salesInvoiceRepository = salesInvoiceRepository;
        _warehouseRepository = warehouseRepository;
        _prescriptionRepository = prescriptionRepository;
        _companyProfileRepository = companyProfileRepository;
        _itemRepository = itemRepository;
        _salesService = salesService;
        _customerRepository = customerRepository;
        _paymentMethodRepository = paymentMethodRepository;
        _charityProjectRepository = charityProjectRepository;
        _grantAgreementRepository = grantAgreementRepository;
    }

    public async Task<IActionResult> Index()
    {
        var invoices = await _salesInvoiceRepository.GetAllAsync();
        return View(invoices);
    }
    [HttpGet]
    public async Task<IActionResult> CreateForCustomer(Guid customerId)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null)
            return NotFound();

        var vm = new CreateSalesInvoiceVm
        {
            InvoiceDateUtc = DateTime.UtcNow,
            InvoiceNumber = "SAL-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
            CustomerId = customer.Id,
            CustomerName = customer.NameAr,
            RevenueCategory = "SelfRevenue",
            Lines = new List<SalesInvoiceLineVm>
        {
            new SalesInvoiceLineVm()
        },
            Payments = new List<SalesPaymentVm>
        {
            new SalesPaymentVm()
        }
        };

        await FillLookups(vm);
        return View("Create", vm);
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new CreateSalesInvoiceVm
        {
            InvoiceDateUtc = DateTime.UtcNow,
            CustomerName = "عميل نقدي",
            RevenueCategory = "SelfRevenue",
            Lines = new List<SalesInvoiceLineVm>
            {
                new SalesInvoiceLineVm()
            }
        };

        await FillLookups(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSalesInvoiceVm vm)
    {
        await FillLookups(vm);

        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            await _salesService.CreateAsync(vm);
            TempData["Success"] = "تم حفظ فاتورة البيع وترحيل المخزون بنجاح";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(vm);
        }
    }
    [HttpGet]
    public async Task<IActionResult> PrintReceipt(Guid id)
    {
        var invoice = await _salesInvoiceRepository.GetByIdAsync(id);
        if (invoice == null)
            return NotFound();

        var company = await _companyProfileRepository.GetActiveAsync();

        var vm = new ReceiptPrintVm
        {
            Invoice = invoice,
            Company = company
        };

        return View(vm);
    }
    private async Task FillLookups(CreateSalesInvoiceVm vm)
    {
        var warehouses = await _warehouseRepository.GetActiveAsync();
        var items = await _itemRepository.GetActiveAsync();
        var methods = await _paymentMethodRepository.GetActiveAsync();
        var projects = await _charityProjectRepository.SearchAsync(null, null, true);
        var grantAgreements = await _grantAgreementRepository.GetAllAsync();

        vm.Warehouses = warehouses.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.WarehouseCode} - {x.WarehouseNameAr}"///
        }).ToList();

        vm.Projects = projects.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.Code} - {x.Name}"
        }).ToList();

        vm.GrantAgreements = grantAgreements.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.AgreementNumber} - {x.Title}"
        }).ToList();

        foreach (var line in vm.Lines)
        {
            line.Items = items
                .Where(x => !x.IsService && x.IsStockItem)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"{x.ItemCode} - {x.ItemNameAr}"
                }).ToList();
        }
        if (vm.Payments == null || !vm.Payments.Any())
        {
            vm.Payments = new List<SalesPaymentVm> { new SalesPaymentVm() };
        }

        foreach (var payment in vm.Payments)
        {
            payment.PaymentMethods = methods.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.MethodCode} - {x.MethodNameAr}"
            }).ToList();
        }
        if (vm.CustomerId.HasValue && vm.CustomerId.Value != Guid.Empty)
        {
            var prescriptions = await _prescriptionRepository.GetActiveByCustomerIdAsync(vm.CustomerId.Value);

            vm.Prescriptions = prescriptions.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.PrescriptionDateUtc:yyyy-MM-dd} - Dr: {(string.IsNullOrWhiteSpace(x.DoctorName) ? "-" : x.DoctorName)}"
            }).ToList();
        }
        else
        {
            vm.Prescriptions = new List<SelectListItem>();
        }
    }
}