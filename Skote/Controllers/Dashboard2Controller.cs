using InfrastrfuctureManagmentCore.Domains.Optics;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Customer;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Expenses;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Optics;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Purchase;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Sale;
using InfrastructureManagmentWebFramework.Models.Optics.Dashboard;
using Microsoft.AspNetCore.Mvc;
/// مؤجل مؤقتًا - خاص بمسار النظارات
/// لا يستخدم في التشغيل الحالي للبرنامج المحاسبي
public class Dashboard2Controller : Controller
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomerAccountTransactionRepository _customerAccountTransactionRepository;
    private readonly ISalesInvoiceRepository _salesInvoiceRepository;
    private readonly IPurchaseInvoiceRepository _purchaseInvoiceRepository;
    private readonly ICustomerReceiptRepository _customerReceiptRepository;
    private readonly IExpenseRepository _expenseRepository;
    private readonly IOpticalWorkOrderRepository _opticalWorkOrderRepository;

    public Dashboard2Controller(
        ICustomerRepository customerRepository,
        ICustomerAccountTransactionRepository customerAccountTransactionRepository,
        ISalesInvoiceRepository salesInvoiceRepository,
        IPurchaseInvoiceRepository purchaseInvoiceRepository,
        ICustomerReceiptRepository customerReceiptRepository,
        IExpenseRepository expenseRepository,
        IOpticalWorkOrderRepository opticalWorkOrderRepository)
    {
        _customerRepository = customerRepository;
        _customerAccountTransactionRepository = customerAccountTransactionRepository;
        _salesInvoiceRepository = salesInvoiceRepository;
        _purchaseInvoiceRepository = purchaseInvoiceRepository;
        _customerReceiptRepository = customerReceiptRepository;
        _expenseRepository = expenseRepository;
        _opticalWorkOrderRepository = opticalWorkOrderRepository;
    }

    public async Task<IActionResult> Index()
    {
        var today = DateTime.UtcNow.Date;

        var customers = await _customerRepository.GetAllAsync();
        var sales = await _salesInvoiceRepository.GetForSalesReportAsync();
        var purchases = await _purchaseInvoiceRepository.GetAllAsync();
        var receipts = await _customerReceiptRepository.GetAllAsync();
        var expenses = await _expenseRepository.GetAllAsync();
        var workOrders = await _opticalWorkOrderRepository.GetAllAsync();

        var customersWithBalance = 0;
        foreach (var customer in customers)
        {
            var txs = await _customerAccountTransactionRepository.GetByCustomerIdAsync(customer.Id);
            var balance = txs.Sum(x => x.DebitAmount - x.CreditAmount);
            if (balance != 0)
                customersWithBalance++;
        }

        var model = new OpticsDashboardVm
        {
            CustomersCount = customers.Count,
            ActiveCustomersCount = customers.Count(x => x.IsActive),
            CustomersWithBalanceCount = customersWithBalance,

            SalesTodayCount = sales.Count(x => x.InvoiceDateUtc.Date == today),
            SalesTodayTotal = sales.Where(x => x.InvoiceDateUtc.Date == today).Sum(x => x.NetAmount),

            PurchasesTodayCount = purchases.Count(x => x.InvoiceDateUtc.Date == today),
            PurchasesTodayTotal = purchases.Where(x => x.InvoiceDateUtc.Date == today).Sum(x => x.NetAmount),

            ReceiptsTodayCount = receipts.Count(x => x.ReceiptDateUtc.Date == today),
            ReceiptsTodayTotal = receipts.Where(x => x.ReceiptDateUtc.Date == today).Sum(x => x.Amount),

            ExpensesTodayCount = expenses.Count(x => x.ExpenseDateUtc.Date == today),
            ExpensesTodayTotal = expenses.Where(x => x.ExpenseDateUtc.Date == today).Sum(x => x.Amount),

            WorkOrdersNewCount = workOrders.Count(x => x.Status == OpticalWorkOrderStatus.New),
            WorkOrdersInLabCount = workOrders.Count(x => x.Status == OpticalWorkOrderStatus.InLab),
            WorkOrdersReadyCount = workOrders.Count(x => x.Status == OpticalWorkOrderStatus.Ready),
            WorkOrdersOverdueCount = workOrders.Count(x =>
                x.Status != OpticalWorkOrderStatus.Delivered &&
                x.Status != OpticalWorkOrderStatus.Cancelled &&
                x.ExpectedDeliveryDateUtc.HasValue &&
                x.ExpectedDeliveryDateUtc.Value < DateTime.UtcNow),

            ReadyOrders = workOrders
                .Where(x => x.Status == OpticalWorkOrderStatus.Ready)
                .OrderBy(x => x.ExpectedDeliveryDateUtc)
                .Take(5)
                .Select(x => new OpticsDashboardOrderVm
                {
                    Id = x.Id,
                    WorkOrderNumber = x.WorkOrderNumber,
                    CustomerName = x.Customer?.NameAr ?? x.SalesInvoice?.CustomerName ?? "-",
                    ExpectedDeliveryDateUtc = x.ExpectedDeliveryDateUtc,
                    IsUrgent = x.IsUrgent
                }).ToList(),

            LatestSales = sales
                .OrderByDescending(x => x.InvoiceDateUtc)
                .Take(5)
                .Select(x => new OpticsDashboardInvoiceVm
                {
                    Id = x.Id,
                    InvoiceNumber = x.InvoiceNumber,
                    CustomerName = x.Customer?.NameAr ?? x.CustomerName,
                    NetAmount = x.NetAmount,
                    InvoiceDateUtc = x.InvoiceDateUtc
                }).ToList(),

            LatestReceipts = receipts
                .OrderByDescending(x => x.ReceiptDateUtc)
                .Take(5)
                .Select(x => new OpticsDashboardReceiptVm
                {
                    ReceiptNumber = x.ReceiptNumber,
                    CustomerName = x.Customer?.NameAr ?? "-",
                    Amount = x.Amount,
                    ReceiptDateUtc = x.ReceiptDateUtc
                }).ToList()
        };

        return View(model);
    }
}