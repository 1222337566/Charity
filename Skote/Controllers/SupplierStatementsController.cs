using InfrastrfuctureManagmentCore.Domains.Purchase;
using InfrastrfuctureManagmentCore.Domains.Suppliers;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Statements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    [Authorize]
    public class SupplierStatementsController : Controller
    {
        private readonly AppDbContext _db;

        public SupplierStatementsController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(Guid? supplierId, DateTime? fromDate, DateTime? toDate, CancellationToken ct)
        {
            var vm = new SupplierStatementPageVm
            {
                SupplierId = supplierId,
                FromDate = fromDate,
                ToDate = toDate
            };

            await FillSuppliersAsync(vm, ct);

            if (!supplierId.HasValue)
                return View(vm);

            var supplier = await _db.Set<Supplier>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == supplierId.Value, ct);

            if (supplier == null)
                return NotFound();

            vm.SupplierNumber = supplier.SupplierNumber;
            vm.SupplierName = supplier.NameAr;

            var lines = new List<StatementLineVm>();

            var invoicesQuery = _db.Set<PurchaseInvoice>()
                .AsNoTracking()
                .Where(x => x.SupplierId == supplierId.Value);

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
                    DocumentType = "PurchaseInvoice",
                    DocumentTypeAr = "فاتورة شراء",
                    DocumentNumber = invoice.InvoiceNumber,
                    Description = $"فاتورة شراء رقم {invoice.InvoiceNumber}",
                    Debit = 0,
                    Credit = invoice.NetAmount,
                    SourceId = invoice.Id
                });
            }

            var paymentsQuery = _db.Set<SupplierPayment>()
                .AsNoTracking()
                .Where(x => x.SupplierId == supplierId.Value && x.Status == "Paid");

            if (fromDate.HasValue)
                paymentsQuery = paymentsQuery.Where(x => x.PaymentDate >= fromDate.Value);

            if (toDate.HasValue)
                paymentsQuery = paymentsQuery.Where(x => x.PaymentDate <= toDate.Value);

            var payments = await paymentsQuery
                .OrderBy(x => x.PaymentDate)
                .ToListAsync(ct);

            foreach (var payment in payments)
            {
                lines.Add(new StatementLineVm
                {
                    DateUtc = payment.PaymentDate,
                    DocumentType = "SupplierPayment",
                    DocumentTypeAr = "سداد مورد",
                    DocumentNumber = payment.PaymentNumber,
                    Description = $"سداد مورد رقم {payment.PaymentNumber}",
                    Debit = payment.Amount,
                    Credit = 0,
                    Notes = payment.Notes,
                    SourceId = payment.Id
                });
            }

            vm.Lines = BuildSupplierRunningBalance(lines);

            return View(vm);
        }

        private async Task FillSuppliersAsync(SupplierStatementPageVm vm, CancellationToken ct)
        {
            vm.Suppliers = await _db.Set<Supplier>()
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.NameAr)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"{x.SupplierNumber} - {x.NameAr}",
                    Selected = vm.SupplierId.HasValue && vm.SupplierId.Value == x.Id
                })
                .ToListAsync(ct);
        }

        private static List<StatementLineVm> BuildSupplierRunningBalance(List<StatementLineVm> lines)
        {
            var ordered = lines
                .OrderBy(x => x.DateUtc)
                .ThenBy(x => x.DocumentType == "PurchaseInvoice" ? 0 : 1)
                .ThenBy(x => x.DocumentNumber)
                .ToList();

            decimal balance = 0;

            foreach (var line in ordered)
            {
                balance += line.Credit - line.Debit;
                line.RunningBalance = balance;
            }

            return ordered;
        }
    }
}
