using InfrastrfuctureManagmentCore.Domains.Customer;
using InfrastrfuctureManagmentCore.Domains.Sale;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Statements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    [Authorize]
    public class CustomerStatementsController : Controller
    {
        private readonly AppDbContext _db;

        public CustomerStatementsController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(Guid? customerId, DateTime? fromDate, DateTime? toDate, CancellationToken ct)
        {
            var vm = new CustomerStatementPageVm
            {
                CustomerId = customerId,
                FromDate = fromDate,
                ToDate = toDate
            };

            await FillCustomersAsync(vm, ct);

            if (!customerId.HasValue)
                return View(vm);

            var customer = await _db.Set<CustomerClient>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == customerId.Value, ct);

            if (customer == null)
                return NotFound();

            vm.CustomerNumber = customer.CustomerNumber;
            vm.CustomerName = customer.NameAr;

            var lines = new List<StatementLineVm>();

            var invoicesQuery = _db.Set<SalesInvoice>()
                .AsNoTracking()
                .Where(x => x.CustomerId == customerId.Value);

            if (fromDate.HasValue)
                invoicesQuery = invoicesQuery.Where(x => x.InvoiceDateUtc >= fromDate.Value);

            if (toDate.HasValue)
                invoicesQuery = invoicesQuery.Where(x => x.InvoiceDateUtc <= toDate.Value);

            var invoices = await invoicesQuery
                .OrderBy(x => x.InvoiceDateUtc)
                .ToListAsync(ct);

            foreach (var invoice in invoices)
            {
                lines.Add(new StatementLineVm
                {
                    DateUtc = invoice.InvoiceDateUtc,
                    DocumentType = "SalesInvoice",
                    DocumentTypeAr = "فاتورة بيع",
                    DocumentNumber = invoice.InvoiceNumber,
                    Description = $"فاتورة بيع رقم {invoice.InvoiceNumber}",
                    Debit = invoice.NetAmount,
                    Credit = 0,
                    SourceId = invoice.Id
                });
            }

            var receiptsQuery = _db.Set<CustomerReceipt>()
                .AsNoTracking()
                .Where(x => x.CustomerId == customerId.Value);

            if (fromDate.HasValue)
                receiptsQuery = receiptsQuery.Where(x => x.ReceiptDateUtc >= fromDate.Value);

            if (toDate.HasValue)
                receiptsQuery = receiptsQuery.Where(x => x.ReceiptDateUtc <= toDate.Value);

            var receipts = await receiptsQuery
                .OrderBy(x => x.ReceiptDateUtc)
                .ToListAsync(ct);

            foreach (var receipt in receipts)
            {
                lines.Add(new StatementLineVm
                {
                    DateUtc = receipt.ReceiptDateUtc,
                    DocumentType = "CustomerReceipt",
                    DocumentTypeAr = "سند قبض",
                    DocumentNumber = receipt.ReceiptNumber,
                    Description = $"سند قبض رقم {receipt.ReceiptNumber}",
                    Debit = 0,
                    Credit = receipt.Amount,
                    Notes = receipt.Notes,
                    SourceId = receipt.Id
                });
            }

            vm.Lines = BuildCustomerRunningBalance(lines);

            return View(vm);
        }

        private async Task FillCustomersAsync(CustomerStatementPageVm vm, CancellationToken ct)
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

        private static List<StatementLineVm> BuildCustomerRunningBalance(List<StatementLineVm> lines)
        {
            var ordered = lines
                .OrderBy(x => x.DateUtc)
                .ThenBy(x => x.DocumentType == "SalesInvoice" ? 0 : 1)
                .ThenBy(x => x.DocumentNumber)
                .ToList();

            decimal balance = 0;

            foreach (var line in ordered)
            {
                balance += line.Debit - line.Credit;
                line.RunningBalance = balance;
            }

            return ordered;
        }
    }
}
