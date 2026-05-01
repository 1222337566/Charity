using InfrastrfuctureManagmentCore.Domains.Customer;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Customer;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Optics;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Prescriptions;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Sale;
using InfrastructureManagmentServices.Customers;
using InfrastructureManagmentWebFramework.Models.Optics.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
/// مؤجل مؤقتًا - خاص بمسار النظارات
/// غير مستخدم من الواجهة الحال
public class CustomerReportsController : Controller
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IPrescriptionRepository _prescriptionRepository;
    private readonly ISalesInvoiceRepository _salesInvoiceRepository;
    private readonly ICustomerAccountTransactionRepository _customerAccountTransactionRepository;
    private readonly ICustomerAccountService _customerAccountService;
    private readonly ICustomerOldRecordRepository _customerOldRecordRepository;
    private readonly IOpticalWorkOrderRepository _opticalWorkOrderRepository;

    public CustomerReportsController(
        ICustomerRepository customerRepository,
        IPrescriptionRepository prescriptionRepository,
        ISalesInvoiceRepository salesInvoiceRepository,
        ICustomerAccountTransactionRepository customerAccountTransactionRepository,
        ICustomerAccountService customerAccountService,
        ICustomerOldRecordRepository customerOldRecordRepository,
        IOpticalWorkOrderRepository opticalWorkOrderRepository)
    {
        _customerRepository = customerRepository;
        _prescriptionRepository = prescriptionRepository;
        _salesInvoiceRepository = salesInvoiceRepository;
        _customerAccountTransactionRepository = customerAccountTransactionRepository;
        _customerAccountService = customerAccountService;
        _customerOldRecordRepository = customerOldRecordRepository;
        _opticalWorkOrderRepository = opticalWorkOrderRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index(Guid? customerId, DateTime? dateFrom, DateTime? dateTo)
    {
        var vm = new CustomerReportPageVm
        {
            Filter = new CustomerReportFilterVm
            {
                CustomerId = customerId,
                DateFrom = dateFrom,
                DateTo = dateTo
            }
        };

        await FillCustomers(vm.Filter);

        if (!customerId.HasValue || customerId.Value == Guid.Empty)
            return View(vm);

        var customer = await _customerRepository.GetByIdAsync(customerId.Value);
        if (customer == null)
            return View(vm);

        var prescriptions = await _prescriptionRepository.GetByCustomerIdAsync(customer.Id);
        var soldItems = await _salesInvoiceRepository.GetSoldItemsByCustomerIdAsync(customer.Id);
        var accountTransactions = await _customerAccountTransactionRepository.GetByCustomerIdAsync(customer.Id);
        var oldRecords = await _customerOldRecordRepository.GetByCustomerIdAsync(customer.Id);
        var workOrders = await _opticalWorkOrderRepository.GetByCustomerIdAsync(customer.Id);

        if (dateFrom.HasValue)
        {
            var from = dateFrom.Value.Date;
            prescriptions = prescriptions.Where(x => x.PrescriptionDateUtc.Date >= from).ToList();
            soldItems = soldItems.Where(x => x.SalesInvoice != null && x.SalesInvoice.InvoiceDateUtc.Date >= from).ToList();
            accountTransactions = accountTransactions.Where(x => x.TransactionDateUtc.Date >= from).ToList();
            oldRecords = oldRecords.Where(x => x.RecordDateUtc.Date >= from).ToList();
            workOrders = workOrders.Where(x => x.OrderDateUtc.Date >= from).ToList();
        }

        if (dateTo.HasValue)
        {
            var to = dateTo.Value.Date;
            prescriptions = prescriptions.Where(x => x.PrescriptionDateUtc.Date <= to).ToList();
            soldItems = soldItems.Where(x => x.SalesInvoice != null && x.SalesInvoice.InvoiceDateUtc.Date <= to).ToList();
            accountTransactions = accountTransactions.Where(x => x.TransactionDateUtc.Date <= to).ToList();
            oldRecords = oldRecords.Where(x => x.RecordDateUtc.Date <= to).ToList();
            workOrders = workOrders.Where(x => x.OrderDateUtc.Date <= to).ToList();
        }

        decimal running = 0;
        var accountEntries = accountTransactions
            .OrderBy(x => x.TransactionDateUtc)
            .ThenBy(x => x.CreatedAtUtc)
            .Select(x =>
            {
                running += x.DebitAmount - x.CreditAmount;

                return new CustomerReportItemVm
                {
                    DateUtc = x.TransactionDateUtc,
                    Title = x.TransactionType switch
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
                    RefNo = x.ReferenceNumber,
                    Details = x.Description,
                    Debit = x.DebitAmount,
                    Credit = x.CreditAmount,
                    StatusText = $"الرصيد: {running}"
                };
            })
            .ToList();

        vm.Report = new CustomerReportVm
        {
            CustomerId = customer.Id,
            CustomerNumber = customer.CustomerNumber,
            CustomerName = customer.NameAr,
            GenderText = customer.Gender == CustomerGender.Male ? "ذكر" : "أنثى",
            Age = customer.Age,
            Tel = customer.Tel,
            MobileNo = customer.MobileNo,
            DateFrom = dateFrom,
            DateTo = dateTo,

            PrescriptionsCount = prescriptions.Count,
            SoldItemsCount = soldItems.Count,
            OldRecordsCount = oldRecords.Count,
            WorkOrdersCount = workOrders.Count,
            AccountBalance = await _customerAccountService.GetBalanceAsync(customer.Id),

            Prescriptions = prescriptions.Select(x => new CustomerReportItemVm
            {
                DateUtc = x.PrescriptionDateUtc,
                Title = "Prescription",
                RefNo = x.DoctorName,
                Details = $"R: {x.RightSph}/{x.RightCyl}/{x.RightAxis} | L: {x.LeftSph}/{x.LeftCyl}/{x.LeftAxis} | IPD: {x.IPD}",
                StatusText = x.IsActive ? "نشطة" : "غير نشطة"
            }).ToList(),

            Sales = soldItems.Select(x => new CustomerReportItemVm
            {
                DateUtc = x.SalesInvoice?.InvoiceDateUtc ?? DateTime.UtcNow,
                Title = x.Item?.ItemNameAr ?? "-",
                RefNo = x.SalesInvoice?.InvoiceNumber,
                Details = $"الكمية: {x.Quantity} | السعر: {x.UnitPrice}",
                Debit = x.LineTotal
            }).ToList(),

            AccountEntries = accountEntries,

            OldRecords = oldRecords.Select(x => new CustomerReportItemVm
            {
                DateUtc = x.RecordDateUtc,
                Title = x.Title,
                Details = x.Details,
                StatusText = x.IsActive ? "نشط" : "غير نشط"
            }).ToList(),

            WorkOrders = workOrders.Select(x => new CustomerReportItemVm
            {
                DateUtc = x.OrderDateUtc,
                Title = x.WorkOrderNumber,
                RefNo = x.SalesInvoice?.InvoiceNumber,
                Details = x.Prescription != null
                    ? $"Prescription: {x.Prescription.PrescriptionDateUtc.ToLocalTime():yyyy-MM-dd}"
                    : null,
                StatusText = x.Status.ToString()
            }).ToList()
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Print(Guid customerId, DateTime? dateFrom, DateTime? dateTo)
    {
        return await Index(customerId, dateFrom, dateTo);
    }

    private async Task FillCustomers(CustomerReportFilterVm vm)
    {
        var customers = await _customerRepository.GetActiveAsync();

        vm.Customers = customers.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.CustomerNumber} - {x.NameAr}"
        }).ToList();
    }
}