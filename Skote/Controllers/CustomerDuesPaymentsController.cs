using InfrastrfuctureManagmentCore.Domains.Customer;
using InfrastrfuctureManagmentCore.Domains.Sale;
using InfrastrfuctureManagmentCore.Domains.Payments;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentServices.Customers;
using InfrastructureManagmentWebFramework.Models.CustomerDuesPayments;
using InfrastructureManagmentWebFramework.Models.Optics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    [Authorize]
    public class CustomerDuesPaymentsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ICustomerReceiptService _customerReceiptService;

        public CustomerDuesPaymentsController(
            AppDbContext db,
            ICustomerReceiptService customerReceiptService)
        {
            _db = db;
            _customerReceiptService = customerReceiptService;
        }

        public async Task<IActionResult> Index(Guid? customerId, CancellationToken ct)
        {
            var customers = await _db.Set<CustomerClient>()
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.NameAr)
                .ToListAsync(ct);

            var invoiceTotals = await _db.Set<SalesInvoice>()
                .AsNoTracking()
                .Where(x => x.CustomerId.HasValue)
                .GroupBy(x => x.CustomerId!.Value)
                .Select(g => new
                {
                    CustomerId = g.Key,
                    Total = g.Sum(x => x.NetAmount)
                })
                .ToListAsync(ct);

            var receiptTotals = await _db.Set<CustomerReceipt>()
                .AsNoTracking()
                .Where(x => x.Status == CustomerReceiptStatus.Posted)
                .GroupBy(x => x.CustomerId)
                .Select(g => new
                {
                    CustomerId = g.Key,
                    Total = g.Sum(x => x.Amount)
                })
                .ToListAsync(ct);

            var rows = customers.Select(c => new CustomerDuesPaymentsRowVm
            {
                CustomerId = c.Id,
                CustomerNumber = c.CustomerNumber,
                CustomerName = c.NameAr,
                MobileNo = c.MobileNo,
                Invoiced = invoiceTotals.FirstOrDefault(x => x.CustomerId == c.Id)?.Total ?? 0,
                Received = receiptTotals.FirstOrDefault(x => x.CustomerId == c.Id)?.Total ?? 0
            }).ToList();

            var vm = new CustomerDuesPaymentsPageVm
            {
                CustomerId = customerId,
                Rows = rows,
                TotalDue = rows.Where(x => x.Due > 0).Sum(x => x.Due)
            };

            if (customerId.HasValue)
            {
                vm.SelectedCustomer = rows.FirstOrDefault(x => x.CustomerId == customerId.Value);

                var receivedByInvoice = await _db.Set<CustomerReceipt>()
                    .AsNoTracking()
                    .Where(x =>
                        x.CustomerId == customerId.Value &&
                        x.Status == CustomerReceiptStatus.Posted &&
                        x.SalesInvoiceId.HasValue)
                    .GroupBy(x => x.SalesInvoiceId!.Value)
                    .Select(g => new
                    {
                        SalesInvoiceId = g.Key,
                        Total = g.Sum(x => x.Amount)
                    })
                    .ToListAsync(ct);

                vm.Invoices = await _db.Set<SalesInvoice>()
                    .AsNoTracking()
                    .Where(x => x.CustomerId == customerId.Value)
                    .OrderByDescending(x => x.InvoiceDateUtc)
                    .Select(x => new CustomerInvoiceDueRowVm
                    {
                        Id = x.Id,
                        InvoiceNumber = x.InvoiceNumber,
                        InvoiceDateUtc = x.InvoiceDateUtc,
                        NetAmount = x.NetAmount,
                        Received = 0
                    })
                    .ToListAsync(ct);

                foreach (var inv in vm.Invoices)
                    inv.Received = receivedByInvoice.FirstOrDefault(x => x.SalesInvoiceId == inv.Id)?.Total ?? 0;

                vm.Receipts = await _db.Set<CustomerReceipt>()
                    .AsNoTracking()
                    .Include(x => x.PaymentMethod)
                    .Include(x => x.SalesInvoice)
                    .Where(x => x.CustomerId == customerId.Value)
                    .OrderByDescending(x => x.ReceiptDateUtc)
                    .Select(x => new CustomerReceiptRowVm
                    {
                        Id = x.Id,
                        ReceiptNumber = x.ReceiptNumber,
                        ReceiptDateUtc = x.ReceiptDateUtc,
                        Amount = x.Amount,
                        PaymentMethodName = x.PaymentMethod != null ? x.PaymentMethod.MethodNameAr : null,
                        InvoiceNumber = x.SalesInvoice != null ? x.SalesInvoice.InvoiceNumber : null,
                        StatusText = x.Status == CustomerReceiptStatus.Posted ? "مرحّل" : x.Status.ToString(),
                        Notes = x.Notes
                    })
                    .ToListAsync(ct);
            }

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Receive(Guid customerId, Guid? invoiceId, CancellationToken ct)
        {
            var customer = await _db.Set<CustomerClient>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == customerId, ct);

            if (customer == null)
                return NotFound();

            var vm = new CustomerReceiveVm
            {
                CustomerId = customer.Id,
                CustomerNumber = customer.CustomerNumber,
                CustomerName = customer.NameAr,
                SalesInvoiceId = invoiceId,
                ReceiptDateUtc = DateTime.UtcNow,
                ReceiptNumber = await GenerateReceiptNumberAsync(ct)
            };

            await FillReceiveVmAsync(vm, ct);

            if (invoiceId.HasValue)
            {
                var invoice = await _db.Set<SalesInvoice>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == invoiceId.Value && x.CustomerId == customer.Id, ct);

                if (invoice != null)
                {
                    var received = await _db.Set<CustomerReceipt>()
                        .AsNoTracking()
                        .Where(x =>
                            x.Status == CustomerReceiptStatus.Posted &&
                            x.SalesInvoiceId == invoice.Id)
                        .SumAsync(x => (decimal?)x.Amount, ct) ?? 0;

                    vm.SuggestedAmount = Math.Max(0, invoice.NetAmount - received);
                    vm.Amount = vm.SuggestedAmount;
                }
            }
            else
            {
                vm.SuggestedAmount = Math.Max(0, vm.CurrentDue);
                vm.Amount = vm.SuggestedAmount;
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Receive(CustomerReceiveVm vm, CancellationToken ct)
        {
            await FillReceiveVmAsync(vm, ct);

            if (!ModelState.IsValid)
                return View(vm);

            var createVm = new CreateCustomerReceiptVm
            {
                ReceiptNumber = vm.ReceiptNumber,
                ReceiptDateUtc = vm.ReceiptDateUtc,
                CustomerId = vm.CustomerId,
                SalesInvoiceId = vm.SalesInvoiceId,
                Amount = vm.Amount,
                PaymentMethodId = vm.PaymentMethodId,
                Notes = vm.Notes
            };

            try
            {
                await _customerReceiptService.CreateAsync(createVm);
                TempData["Success"] = "تم تسجيل تحصيل العميل بنجاح.";
                return RedirectToAction(nameof(Index), new { customerId = vm.CustomerId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
        }

        private async Task FillReceiveVmAsync(CustomerReceiveVm vm, CancellationToken ct)
        {
            var customer = await _db.Set<CustomerClient>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == vm.CustomerId, ct);

            if (customer != null)
            {
                vm.CustomerNumber = customer.CustomerNumber;
                vm.CustomerName = customer.NameAr;
            }

            var debit = await _db.Set<SalesInvoice>()
                .AsNoTracking()
                .Where(x => x.CustomerId == vm.CustomerId)
                .SumAsync(x => (decimal?)x.NetAmount, ct) ?? 0;

            var credit = await _db.Set<CustomerReceipt>()
                .AsNoTracking()
                .Where(x => x.CustomerId == vm.CustomerId && x.Status == CustomerReceiptStatus.Posted)
                .SumAsync(x => (decimal?)x.Amount, ct) ?? 0;

            vm.CurrentDue = debit - credit;

            vm.PaymentMethods = await _db.Set<PaymentMethod>()
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.MethodNameAr)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"{x.MethodCode} - {x.MethodNameAr}",
                    Selected = vm.PaymentMethodId.HasValue && vm.PaymentMethodId.Value == x.Id
                })
                .ToListAsync(ct);

            var receivedByInvoice = await _db.Set<CustomerReceipt>()
                .AsNoTracking()
                .Where(x =>
                    x.CustomerId == vm.CustomerId &&
                    x.Status == CustomerReceiptStatus.Posted &&
                    x.SalesInvoiceId.HasValue)
                .GroupBy(x => x.SalesInvoiceId!.Value)
                .Select(g => new
                {
                    InvoiceId = g.Key,
                    Total = g.Sum(x => x.Amount)
                })
                .ToListAsync(ct);

            var invoices = await _db.Set<SalesInvoice>()
                .AsNoTracking()
                .Where(x => x.CustomerId == vm.CustomerId)
                .OrderByDescending(x => x.InvoiceDateUtc)
                .ToListAsync(ct);

            vm.Invoices = invoices.Select(inv =>
            {
                var received = receivedByInvoice.FirstOrDefault(x => x.InvoiceId == inv.Id)?.Total ?? 0;
                var balance = inv.NetAmount - received;

                return new SelectListItem
                {
                    Value = inv.Id.ToString(),
                    Text = $"{inv.InvoiceNumber} — إجمالي {inv.NetAmount:N2} — متبقي {balance:N2}",
                    Selected = vm.SalesInvoiceId.HasValue && vm.SalesInvoiceId.Value == inv.Id
                };
            }).ToList();
        }

        private async Task<string> GenerateReceiptNumberAsync(CancellationToken ct)
        {
            var count = await _db.Set<CustomerReceipt>().CountAsync(ct);
            return $"REC-{DateTime.Now:yyyyMM}-{(count + 1):000}";
        }
    }
}
