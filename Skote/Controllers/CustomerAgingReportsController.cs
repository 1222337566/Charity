using InfrastrfuctureManagmentCore.Domains.Customer;
using InfrastrfuctureManagmentCore.Domains.Sale;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.AgingReports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    [Authorize]
    public class CustomerAgingReportsController : Controller
    {
        private readonly AppDbContext _db;

        public CustomerAgingReportsController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(Guid? customerId, DateTime? asOfDate, CancellationToken ct)
        {
            var vm = new CustomerAgingReportPageVm
            {
                CustomerId = customerId,
                AsOfDate = asOfDate?.Date ?? DateTime.Today
            };

            await FillCustomersAsync(vm, ct);

            var customersQuery = _db.Set<CustomerClient>()
                .AsNoTracking()
                .Where(x => x.IsActive);

            if (customerId.HasValue)
                customersQuery = customersQuery.Where(x => x.Id == customerId.Value);

            var customers = await customersQuery.OrderBy(x => x.NameAr).ToListAsync(ct);

            foreach (var customer in customers)
            {
                var invoices = await _db.Set<SalesInvoice>()
                    .AsNoTracking()
                    .Where(x => x.CustomerId == customer.Id && x.InvoiceDateUtc.Date <= vm.AsOfDate.Date)
                    .OrderBy(x => x.InvoiceDateUtc)
                    .Select(x => new { x.Id, x.InvoiceNumber, x.InvoiceDateUtc, x.NetAmount })
                    .ToListAsync(ct);

                var totalReceipts = await _db.Set<CustomerReceipt>()
                    .AsNoTracking()
                    .Where(x => x.CustomerId == customer.Id && x.ReceiptDateUtc.Date <= vm.AsOfDate.Date)
                    .SumAsync(x => (decimal?)x.Amount, ct) ?? 0m;

                var row = new AgingReportRowVm
                {
                    PartyId = customer.Id,
                    PartyNumber = customer.CustomerNumber,
                    PartyName = customer.NameAr,
                    TotalInvoiced = invoices.Sum(x => x.NetAmount),
                    TotalPaidOrReceived = totalReceipts
                };

                decimal remainingReceiptsToAllocate = totalReceipts;

                foreach (var invoice in invoices)
                {
                    var allocated = Math.Min(invoice.NetAmount, remainingReceiptsToAllocate);
                    remainingReceiptsToAllocate -= allocated;

                    var remaining = invoice.NetAmount - allocated;
                    if (remaining <= 0)
                        continue;

                    var ageDays = Math.Max(0, (vm.AsOfDate.Date - invoice.InvoiceDateUtc.Date).Days);
                    var bucket = AddToBucket(row, ageDays, remaining);

                    row.OpenInvoices.Add(new AgingInvoiceLineVm
                    {
                        InvoiceId = invoice.Id,
                        InvoiceNumber = invoice.InvoiceNumber,
                        InvoiceDateUtc = invoice.InvoiceDateUtc,
                        AgeDays = ageDays,
                        InvoiceAmount = invoice.NetAmount,
                        AllocatedPayment = allocated,
                        RemainingAmount = remaining,
                        Bucket = bucket
                    });
                }

                if (row.TotalOutstanding > 0 || row.TotalInvoiced > 0 || row.TotalPaidOrReceived > 0)
                    vm.Rows.Add(row);
            }

            return View(vm);
        }

        private async Task FillCustomersAsync(CustomerAgingReportPageVm vm, CancellationToken ct)
        {
            vm.Customers = await _db.Set<CustomerClient>()
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.NameAr)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"{x.CustomerNumber} - {x.NameAr}",
                    Selected = vm.CustomerId.HasValue && vm.CustomerId.Value == x.Id
                })
                .ToListAsync(ct);
        }

        private static string AddToBucket(AgingReportRowVm row, int ageDays, decimal amount)
        {
            if (ageDays <= 30)
            {
                row.Current0To30 += amount;
                return "0-30";
            }

            if (ageDays <= 60)
            {
                row.Days31To60 += amount;
                return "31-60";
            }

            if (ageDays <= 90)
            {
                row.Days61To90 += amount;
                return "61-90";
            }

            row.Over90 += amount;
            return "90+";
        }
    }
}
