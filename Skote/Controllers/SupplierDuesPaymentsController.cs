using InfrastrfuctureManagmentCore.Domains.Suppliers;
using InfrastrfuctureManagmentCore.Domains.Purchase;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.SupplierDuesPayments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    [Authorize]
    public class SupplierDuesPaymentsController : Controller
    {
        private readonly AppDbContext _db;

        public SupplierDuesPaymentsController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(Guid? supplierId, CancellationToken ct)
        {
            var suppliers = await _db.Set<Supplier>()
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.NameAr)
                .ToListAsync(ct);

            var invoiceTotals = await _db.Set<PurchaseInvoice>()
                .AsNoTracking()
                .GroupBy(x => x.SupplierId)
                .Select(g => new
                {
                    SupplierId = g.Key,
                    Total = g.Sum(x => x.NetAmount)
                })
                .ToListAsync(ct);

            var paymentTotals = await _db.Set<SupplierPayment>()
                .AsNoTracking()
                .Where(x => x.Status == "Paid")
                .GroupBy(x => x.SupplierId)
                .Select(g => new
                {
                    SupplierId = g.Key,
                    Total = g.Sum(x => x.Amount)
                })
                .ToListAsync(ct);

            var rows = suppliers.Select(s => new SupplierDuesPaymentsRowVm
            {
                SupplierId = s.Id,
                SupplierNumber = s.SupplierNumber,
                SupplierName = s.NameAr,
                MobileNo = s.MobileNo,
                Invoiced = invoiceTotals.FirstOrDefault(x => x.SupplierId == s.Id)?.Total ?? 0,
                Paid = paymentTotals.FirstOrDefault(x => x.SupplierId == s.Id)?.Total ?? 0
            }).ToList();

            var vm = new SupplierDuesPaymentsPageVm
            {
                SupplierId = supplierId,
                Rows = rows,
                TotalDue = rows.Where(x => x.Due > 0).Sum(x => x.Due)
            };

            if (supplierId.HasValue)
            {
                vm.SelectedSupplier = rows.FirstOrDefault(x => x.SupplierId == supplierId.Value);

                var paidByInvoice = await _db.Set<SupplierPayment>()
                    .AsNoTracking()
                    .Where(x =>
                        x.SupplierId == supplierId.Value &&
                        x.Status == "Paid" &&
                        x.PurchaseInvoiceId.HasValue)
                    .GroupBy(x => x.PurchaseInvoiceId!.Value)
                    .Select(g => new
                    {
                        PurchaseInvoiceId = g.Key,
                        Total = g.Sum(x => x.Amount)
                    })
                    .ToListAsync(ct);

                vm.Invoices = await _db.Set<PurchaseInvoice>()
                    .AsNoTracking()
                    .Where(x => x.SupplierId == supplierId.Value)
                    .OrderByDescending(x => x.InvoiceDateUtc)
                    .Select(x => new SupplierInvoiceDueRowVm
                    {
                        Id = x.Id,
                        InvoiceNumber = x.InvoiceNumber,
                        InvoiceDateUtc = x.InvoiceDateUtc,
                        NetAmount = x.NetAmount,
                        Paid = 0
                    })
                    .ToListAsync(ct);

                foreach (var inv in vm.Invoices)
                    inv.Paid = paidByInvoice.FirstOrDefault(x => x.PurchaseInvoiceId == inv.Id)?.Total ?? 0;
                vm.Payments = await _db.Set<SupplierPayment>()
                    .AsNoTracking()
                    .Include(x => x.PurchaseInvoice)
                    .Where(x => x.SupplierId == supplierId.Value)
                    .OrderByDescending(x => x.PaymentDate)
                    .Select(x => new SupplierPaymentRowVm
                    {
                        Id = x.Id,
                        PaymentNumber = x.PaymentNumber,
                        PaymentDateUtc = x.PaymentDate,
                        Amount = x.Amount,
                        PaymentMethodName = x.PaymentMethod,
                        InvoiceNumber = x.PurchaseInvoice != null ? x.PurchaseInvoice.InvoiceNumber : null,
                        StatusText = x.Status == "Paid"
                            ? "مدفوع"
                            : x.Status == "Pending"
                                ? "معلق"
                                : x.Status == "Cancelled"
                                    ? "ملغي"
                                    : x.Status,
                        Notes = x.Notes
                    })
                    .ToListAsync(ct);
            }

            return View(vm);
        }
    }
}
