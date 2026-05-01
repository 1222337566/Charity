using InfrastrfuctureManagmentCore.Persistence.Repositories.Customer;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Optics;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Sale;
using InfrastructureManagmentWebFramework.Models.Optics.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class SalesReportsController : Controller
{
    private readonly ISalesInvoiceRepository _salesInvoiceRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IOpticalWorkOrderRepository _opticalWorkOrderRepository;

    public SalesReportsController(
        ISalesInvoiceRepository salesInvoiceRepository,
        ICustomerRepository customerRepository,
        IWarehouseRepository warehouseRepository,
        IOpticalWorkOrderRepository opticalWorkOrderRepository)
    {
        _salesInvoiceRepository = salesInvoiceRepository;
        _customerRepository = customerRepository;
        _warehouseRepository = warehouseRepository;
        _opticalWorkOrderRepository = opticalWorkOrderRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index(DateTime? dateFrom, DateTime? dateTo, Guid? customerId, Guid? warehouseId)
    {
        var vm = new SalesReportPageVm
        {
            Filter = new SalesReportFilterVm
            {
                DateFrom = dateFrom,
                DateTo = dateTo,
                CustomerId = customerId,
                WarehouseId = warehouseId
            }
        };

        await FillLookups(vm.Filter);

        var invoices = await _salesInvoiceRepository.GetForSalesReportAsync();

        if (dateFrom.HasValue)
        {
            var from = dateFrom.Value.Date;
            invoices = invoices.Where(x => x.InvoiceDateUtc.Date >= from).ToList();
        }

        if (dateTo.HasValue)
        {
            var to = dateTo.Value.Date;
            invoices = invoices.Where(x => x.InvoiceDateUtc.Date <= to).ToList();
        }

        if (customerId.HasValue && customerId.Value != Guid.Empty)
        {
            invoices = invoices.Where(x => x.CustomerId == customerId.Value).ToList();
        }

        if (warehouseId.HasValue && warehouseId.Value != Guid.Empty)
        {
            invoices = invoices.Where(x => x.WarehouseId == warehouseId.Value).ToList();
        }

        var invoiceIds = invoices.Select(x => x.Id).ToList();
        var workOrders = await _opticalWorkOrderRepository.GetBySalesInvoiceIdsAsync(invoiceIds);

        vm.Report = new SalesReportVm
        {
            DateFrom = dateFrom,
            DateTo = dateTo,
            InvoiceCount = invoices.Count,
            NetAmountTotal = invoices.Sum(x => x.NetAmount),
            TotalPaid = invoices.Sum(x => x.Payments.Sum(p => p.Amount)),
            PrescriptionInvoicesCount = invoices.Count(x => x.PrescriptionId.HasValue),
            WorkOrdersCount = workOrders.Count,
            Rows = invoices.Select(x =>
            {
                var workOrder = workOrders.FirstOrDefault(w => w.SalesInvoiceId == x.Id);

                return new SalesReportRowVm
                {
                    SalesInvoiceId = x.Id,
                    InvoiceNumber = x.InvoiceNumber,
                    InvoiceDateUtc = x.InvoiceDateUtc,
                    CustomerName = x.Customer?.NameAr ?? x.CustomerName,
                    WarehouseName = x.Warehouse?.WarehouseNameAr ?? "-",
                    HasPrescription = x.PrescriptionId.HasValue,
                    PrescriptionText = x.Prescription != null
                        ? x.Prescription.PrescriptionDateUtc.ToLocalTime().ToString("yyyy-MM-dd")
                        : null,
                    NetAmount = x.NetAmount,
                    TotalPaid = x.Payments.Sum(p => p.Amount),
                    PaymentMethodsText = x.Payments.Any()
                        ? string.Join(" / ", x.Payments
                            .Where(p => p.PaymentMethod != null)
                            .Select(p => $"{p.PaymentMethod!.MethodNameAr}: {p.Amount}"))
                        : "-",
                    Notes = x.Notes,
                    HasWorkOrder = workOrder != null,
                    WorkOrderNumber = workOrder?.WorkOrderNumber
                };
            }).ToList()
        };

        return View(vm);
    }

    private async Task FillLookups(SalesReportFilterVm vm)
    {
        var customers = await _customerRepository.GetActiveAsync();
        var warehouses = await _warehouseRepository.GetActiveAsync();

        vm.Customers = customers.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.CustomerNumber} - {x.NameAr}"
        }).ToList();

        vm.Warehouses = warehouses.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.WarehouseCode} - {x.WarehouseNameAr}"
        }).ToList();
    }
}