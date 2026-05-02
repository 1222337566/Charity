using InfrastrfuctureManagmentCore.Persistence.Repositories.Customer;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Optics;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Sale;
using InfrastructureManagmentWebFramework.Models.Optics.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
/// مؤجل مؤقتًا - خاص بمسار النظارات
/// غير مستخدم من الواجهة الحال
public class FindSalesController : Controller
{
    private readonly ISalesInvoiceRepository _salesInvoiceRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IOpticalWorkOrderRepository _opticalWorkOrderRepository;

    public FindSalesController(
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
    public async Task<IActionResult> Index(
        string? q,
        DateTime? dateFrom,
        DateTime? dateTo,
        Guid? customerId,
        Guid? warehouseId,
        bool onlyWithPrescription = false,
        bool onlyWithWorkOrder = false)
    {
        var vm = new FindSalesPageVm
        {
            Filter = new FindSalesFilterVm
            {
                Q = q,
                DateFrom = dateFrom,
                DateTo = dateTo,
                CustomerId = customerId,
                WarehouseId = warehouseId,
                OnlyWithPrescription = onlyWithPrescription,
                OnlyWithWorkOrder = onlyWithWorkOrder
            }
        };

        await FillLookups(vm.Filter);

        var invoices = await _salesInvoiceRepository.SearchAsync(
            dateFrom, dateTo, customerId, warehouseId, q, onlyWithPrescription);

        var workOrders = await _opticalWorkOrderRepository.GetBySalesInvoiceIdsAsync(invoices.Select(x => x.Id).ToList());

        if (onlyWithWorkOrder)
        {
            invoices = invoices.Where(x => workOrders.Any(w => w.SalesInvoiceId == x.Id)).ToList();
        }

        vm.Rows = invoices.Select(x =>
        {
            var wo = workOrders.FirstOrDefault(w => w.SalesInvoiceId == x.Id);

            return new FindSalesRowVm
            {
                SalesInvoiceId = x.Id,
                InvoiceNumber = x.InvoiceNumber,
                InvoiceDateUtc = x.InvoiceDateUtc,
                CustomerName = x.Customer?.NameAr ?? x.CustomerName,
                WarehouseName = x.Warehouse?.WarehouseNameAr ?? "-",
                PrescriptionText = x.Prescription != null
                    ? x.Prescription.PrescriptionDateUtc.ToLocalTime().ToString("yyyy-MM-dd")
                    : null,
                NetAmount = x.NetAmount,
                PaymentMethodsText = x.Payments.Any()
                    ? string.Join(" / ", x.Payments
                        .Where(p => p.PaymentMethod != null)
                        .Select(p => $"{p.PaymentMethod!.MethodNameAr}: {p.Amount}"))
                    : "-",
                Notes = x.Notes,
                WorkOrderNumber = wo?.WorkOrderNumber
            };
        }).ToList();

        vm.Count = vm.Rows.Count;
        vm.TotalNetAmount = vm.Rows.Sum(x => x.NetAmount);

        return View(vm);
    }

    private async Task FillLookups(FindSalesFilterVm vm)
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