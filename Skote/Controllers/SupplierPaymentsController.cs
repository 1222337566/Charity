using InfrastrfuctureManagmentCore.Domains.Suppliers;
using InfrastrfuctureManagmentCore.Domains.Purchase;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Skote.Controllers
{
    [Authorize]
    public class SupplierPaymentsController : Controller
    {
        private readonly AppDbContext _db;
        public SupplierPaymentsController(AppDbContext db) => _db = db;

        // ── لوحة مستحقات الموردين ──
        public async Task<IActionResult> Index(Guid? supplierId, CancellationToken ct)
        {
            // إجماليات لكل مورد: الفواتير - المدفوع = المستحق
            var suppliers = await _db.Set<Supplier>()
                .AsNoTracking().Where(x => x.IsActive)
                .OrderBy(x => x.NameAr).ToListAsync(ct);

            var invoiceTotals = await _db.Set<PurchaseInvoice>()
                .AsNoTracking()
                .GroupBy(x => x.SupplierId)
                .Select(g => new { SupplierId = g.Key, Total = g.Sum(x => x.NetAmount) })
                .ToListAsync(ct);

            var paymentTotals = await _db.Set<SupplierPayment>()
                .AsNoTracking()
                .Where(x => x.Status == "Paid")
                .GroupBy(x => x.SupplierId)
                .Select(g => new { SupplierId = g.Key, Total = g.Sum(x => x.Amount) })
                .ToListAsync(ct);

            var rows = suppliers.Select(s => new {
                Supplier   = s,
                Invoiced   = invoiceTotals.FirstOrDefault(x => x.SupplierId == s.Id)?.Total ?? 0,
                Paid       = paymentTotals.FirstOrDefault(x => x.SupplierId == s.Id)?.Total ?? 0,
            }).Select(x => new {
                x.Supplier,
                x.Invoiced,
                x.Paid,
                Balance = x.Invoiced - x.Paid
            }).ToList();

            ViewBag.Rows       = rows;
            ViewBag.SupplierId = supplierId;
            ViewBag.TotalDue   = rows.Sum(x => x.Balance);

            // لو مورد محدد — جيب فواتيره وسداداته
            if (supplierId.HasValue)
            {
                var supplier = suppliers.FirstOrDefault(x => x.Id == supplierId.Value);
                ViewBag.Supplier = supplier;
                ViewBag.Invoices = await _db.Set<PurchaseInvoice>()
                    .AsNoTracking()
                    .Where(x => x.SupplierId == supplierId.Value)
                    .OrderByDescending(x => x.InvoiceDateUtc)
                    .ToListAsync(ct);
                ViewBag.Payments = await _db.Set<SupplierPayment>()
                    .AsNoTracking()
                    .Where(x => x.SupplierId == supplierId.Value)
                    .OrderByDescending(x => x.PaymentDate)
                    .ToListAsync(ct);
            }

            return View();
        }

        // ── تسجيل دفعة جديدة ──
        [HttpGet]
        public async Task<IActionResult> Pay(Guid supplierId, Guid? invoiceId, CancellationToken ct)
        {
            var supplier = await _db.Set<Supplier>().FindAsync(new object[]{supplierId}, ct);
            if (supplier == null) return NotFound();

            var unpaidInvoices = await _db.Set<PurchaseInvoice>()
                .AsNoTracking()
                .Where(x => x.SupplierId == supplierId)
                .OrderByDescending(x => x.InvoiceDateUtc)
                .ToListAsync(ct);

            var paid = await _db.Set<SupplierPayment>()
                .AsNoTracking()
                .Where(x => x.SupplierId == supplierId && x.Status == "Paid")
                .GroupBy(x => x.PurchaseInvoiceId)
                .Select(g => new { InvoiceId = g.Key, Total = g.Sum(x => x.Amount) })
                .ToListAsync(ct);

            ViewBag.Supplier = supplier;
            ViewBag.Invoices = unpaidInvoices.Select(inv => new SelectListItem {
                Value    = inv.Id.ToString(),
                Text     = $"{inv.InvoiceNumber} — {inv.NetAmount:N2} جنيه",
                Selected = inv.Id == invoiceId
            }).ToList();

            // المبلغ المقترح = رصيد الفاتورة المحددة
            decimal suggested = 0;
            if (invoiceId.HasValue)
            {
                var inv = unpaidInvoices.FirstOrDefault(x => x.Id == invoiceId.Value);
                var paidForInv = paid.FirstOrDefault(x => x.InvoiceId == invoiceId.Value)?.Total ?? 0;
                suggested = (inv?.NetAmount ?? 0) - paidForInv;
            }
            ViewBag.SuggestedAmount = suggested;

            // رقم الدفعة التلقائي
            var count = await _db.Set<SupplierPayment>().CountAsync(ct);
            ViewBag.PaymentNumber = $"PAY-{DateTime.Now:yyyyMM}-{(count + 1):000}";

            return View(new SupplierPayment {
                SupplierId       = supplierId,
                PurchaseInvoiceId = invoiceId,
                Amount           = suggested
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(SupplierPayment model, CancellationToken ct)
        {
            model.Id             = Guid.NewGuid();
            model.CreatedAtUtc   = DateTime.UtcNow;
            model.CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _db.Set<SupplierPayment>().Add(model);
            await _db.SaveChangesAsync(ct);
            TempData["Success"] = $"تم تسجيل الدفعة {model.PaymentNumber} بنجاح";
            return RedirectToAction(nameof(Index), new { supplierId = model.SupplierId });
        }
    }
}
