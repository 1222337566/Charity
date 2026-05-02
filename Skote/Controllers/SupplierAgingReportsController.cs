using InfrastrfuctureManagmentCore.Domains.Purchase;
using InfrastrfuctureManagmentCore.Domains.Suppliers;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.AgingReports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    [Authorize]
    public class SupplierAgingReportsController : Controller
    {
        private readonly AppDbContext _db;

        public SupplierAgingReportsController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(Guid? supplierId, DateTime? asOfDate, CancellationToken ct)
        {
            var vm = new SupplierAgingReportPageVm
            {
                SupplierId = supplierId,
                AsOfDate = asOfDate?.Date ?? DateTime.Today
            };

            await FillSuppliersAsync(vm, ct);

            var suppliersQuery = _db.Set<Supplier>()
                .AsNoTracking()
                .Where(x => x.IsActive);

            if (supplierId.HasValue)
                suppliersQuery = suppliersQuery.Where(x => x.Id == supplierId.Value);

            var suppliers = await suppliersQuery.OrderBy(x => x.NameAr).ToListAsync(ct);

            foreach (var supplier in suppliers)
            {
                var invoices = await _db.Set<PurchaseInvoice>()
                    .AsNoTracking()
                    .Where(x => x.SupplierId == supplier.Id && x.InvoiceDateUtc.Date <= vm.AsOfDate.Date)
                    .OrderBy(x => x.InvoiceDateUtc)
                    .Select(x => new { x.Id, x.InvoiceNumber, x.InvoiceDateUtc, x.NetAmount })
                    .ToListAsync(ct);

                var totalPayments = await _db.Set<SupplierPayment>()
                    .AsNoTracking()
                    .Where(x => x.SupplierId == supplier.Id && x.Status == "Paid" && x.PaymentDate.Date <= vm.AsOfDate.Date)
                    .SumAsync(x => (decimal?)x.Amount, ct) ?? 0m;

                var row = new AgingReportRowVm
                {
                    PartyId = supplier.Id,
                    PartyNumber = supplier.SupplierNumber,
                    PartyName = supplier.NameAr,
                    TotalInvoiced = invoices.Sum(x => x.NetAmount),
                    TotalPaidOrReceived = totalPayments
                };

                decimal remainingPaymentsToAllocate = totalPayments;

                foreach (var invoice in invoices)
                {
                    var allocated = Math.Min(invoice.NetAmount, remainingPaymentsToAllocate);
                    remainingPaymentsToAllocate -= allocated;

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

        private async Task FillSuppliersAsync(SupplierAgingReportPageVm vm, CancellationToken ct)
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
