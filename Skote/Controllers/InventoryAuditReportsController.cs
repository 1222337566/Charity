using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using InfrastrfuctureManagmentCore.Domains.Purchase;
using InfrastrfuctureManagmentCore.Domains.Sale;
using InfrastrfuctureManagmentCore.Domains.Warehouses;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.InventoryAuditReports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    [Authorize]
    public class InventoryAuditReportsController : Controller
    {
        private const string PurchaseInvoiceSourceType = "PurchaseInvoice";
        private const string SalesInvoiceSourceType = "SalesInvoice";
        private const string StoreReceiptReferenceType = "StoreReceipt";
        private const string StoreIssueReferenceType = "StoreIssue";
        private const string ApprovedStatus = "Approved";

        private readonly AppDbContext _db;

        public InventoryAuditReportsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> InvoiceStockReconciliation(DateTime? fromDate, DateTime? toDate)
        {
            var vm = new InvoiceStockReconciliationVm
            {
                FromDate = fromDate,
                ToDate = toDate
            };

            vm.PurchaseRows = await BuildPurchaseRowsAsync(fromDate, toDate);
            vm.SalesRows = await BuildSalesRowsAsync(fromDate, toDate);

            return View(vm);
        }

        private async Task<List<InvoiceStockReconciliationRowVm>> BuildPurchaseRowsAsync(DateTime? fromDate, DateTime? toDate)
        {
            var invoicesQuery = _db.Set<PurchaseInvoice>()
                .AsNoTracking()
                .Include(x => x.Warehouse)
                .Include(x => x.Lines)
                .AsQueryable();

            if (fromDate.HasValue)
                invoicesQuery = invoicesQuery.Where(x => x.InvoiceDateUtc.Date >= fromDate.Value.Date);

            if (toDate.HasValue)
                invoicesQuery = invoicesQuery.Where(x => x.InvoiceDateUtc.Date <= toDate.Value.Date);

            var invoices = await invoicesQuery
                .OrderByDescending(x => x.InvoiceDateUtc)
                .ToListAsync();

            var invoiceIds = invoices.Select(x => x.Id).ToList();

            var receipts = await _db.Set<CharityStoreReceipt>()
                .AsNoTracking()
                .Include(x => x.Lines)
                .Where(x => x.SourceType == PurchaseInvoiceSourceType && x.SourceId.HasValue && invoiceIds.Contains(x.SourceId.Value))
                .ToListAsync();

            var receiptIds = receipts.Select(x => x.Id).ToList();

            var stockTransactions = await _db.Set<StockTransaction>()
                .AsNoTracking()
                .Where(x => x.ReferenceType == StoreReceiptReferenceType && x.ReferenceId.HasValue && receiptIds.Contains(x.ReferenceId.Value))
                .ToListAsync();

            var rows = new List<InvoiceStockReconciliationRowVm>();

            foreach (var invoice in invoices)
            {
                var receipt = receipts.FirstOrDefault(x => x.SourceId == invoice.Id);
                var invoiceQty = invoice.Lines.Sum(x => x.Quantity);
                var receiptQty = receipt?.Lines.Sum(x => x.Quantity) ?? 0m;
                var stockQty = receipt == null
                    ? 0m
                    : stockTransactions.Where(x => x.ReferenceId == receipt.Id).Sum(x => x.Quantity);

                var row = new InvoiceStockReconciliationRowVm
                {
                    InvoiceId = invoice.Id,
                    InvoiceType = "PurchaseInvoice",
                    InvoiceNumber = invoice.InvoiceNumber,
                    InvoiceDateUtc = invoice.InvoiceDateUtc,
                    PartyName = !string.IsNullOrWhiteSpace(invoice.SupplierName) ? invoice.SupplierName : invoice.Supplier?.NameAr ?? "",
                    WarehouseId = invoice.WarehouseId,
                    WarehouseName = invoice.Warehouse?.WarehouseNameAr ?? "",
                    InvoiceQuantity = invoiceQty,
                    VoucherId = receipt?.Id,
                    VoucherNumber = receipt?.ReceiptNumber,
                    VoucherApprovalStatus = receipt?.ApprovalStatus,
                    VoucherQuantity = receiptQty,
                    StockTransactionQuantity = stockQty
                };

                ApplyStatus(row, receipt?.ApprovalStatus);
                rows.Add(row);
            }

            return rows;
        }

        private async Task<List<InvoiceStockReconciliationRowVm>> BuildSalesRowsAsync(DateTime? fromDate, DateTime? toDate)
        {
            var invoicesQuery = _db.Set<SalesInvoice>()
                .AsNoTracking()
                .Include(x => x.Warehouse)
                .Include(x => x.Lines)
                .AsQueryable();

            if (fromDate.HasValue)
                invoicesQuery = invoicesQuery.Where(x => x.InvoiceDateUtc.Date >= fromDate.Value.Date);

            if (toDate.HasValue)
                invoicesQuery = invoicesQuery.Where(x => x.InvoiceDateUtc.Date <= toDate.Value.Date);

            var invoices = await invoicesQuery
                .OrderByDescending(x => x.InvoiceDateUtc)
                .ToListAsync();

            var invoiceIds = invoices.Select(x => x.Id).ToList();

            var issues = await _db.Set<CharityStoreIssue>()
                .AsNoTracking()
                .Include(x => x.Lines)
                .Where(x => x.SourceType == SalesInvoiceSourceType && x.SourceId.HasValue && invoiceIds.Contains(x.SourceId.Value))
                .ToListAsync();

            var issueIds = issues.Select(x => x.Id).ToList();

            var stockTransactions = await _db.Set<StockTransaction>()
                .AsNoTracking()
                .Where(x => x.ReferenceType == StoreIssueReferenceType && x.ReferenceId.HasValue && issueIds.Contains(x.ReferenceId.Value))
                .ToListAsync();

            var rows = new List<InvoiceStockReconciliationRowVm>();

            foreach (var invoice in invoices)
            {
                var issue = issues.FirstOrDefault(x => x.SourceId == invoice.Id);
                var invoiceQty = invoice.Lines.Sum(x => x.Quantity);
                var issueQty = issue?.Lines.Sum(x => x.Quantity) ?? 0m;
                var stockQty = issue == null
                    ? 0m
                    : stockTransactions.Where(x => x.ReferenceId == issue.Id).Sum(x => x.Quantity);

                var row = new InvoiceStockReconciliationRowVm
                {
                    InvoiceId = invoice.Id,
                    InvoiceType = "SalesInvoice",
                    InvoiceNumber = invoice.InvoiceNumber,
                    InvoiceDateUtc = invoice.InvoiceDateUtc,
                    PartyName = invoice.CustomerName,
                    WarehouseId = invoice.WarehouseId,
                    WarehouseName = invoice.Warehouse?.WarehouseNameAr ?? "",
                    InvoiceQuantity = invoiceQty,
                    VoucherId = issue?.Id,
                    VoucherNumber = issue?.IssueNumber,
                    VoucherApprovalStatus = issue?.ApprovalStatus,
                    VoucherQuantity = issueQty,
                    StockTransactionQuantity = stockQty
                };

                ApplyStatus(row, issue?.ApprovalStatus);
                rows.Add(row);
            }

            return rows;
        }

        private static void ApplyStatus(InvoiceStockReconciliationRowVm row, string? approvalStatus)
        {
            if (!row.VoucherId.HasValue)
            {
                row.StatusCode = InvoiceStockReconciliationStatusCodes.MissingVoucher;
                row.Notes = "الفاتورة ليس لها إذن مخزني مرتبط.";
                return;
            }

            if (!string.Equals(approvalStatus, ApprovedStatus, StringComparison.OrdinalIgnoreCase))
            {
                row.StatusCode = InvoiceStockReconciliationStatusCodes.VoucherNotApproved;
                row.Notes = "الإذن المخزني موجود لكنه غير معتمد.";
                return;
            }

            if (row.StockTransactionQuantity <= 0)
            {
                row.StatusCode = InvoiceStockReconciliationStatusCodes.MissingStockTransaction;
                row.Notes = "الإذن معتمد لكن لا توجد حركة مخزون مرتبطة به.";
                return;
            }

            if (Math.Round(row.InvoiceQuantity, 2) != Math.Round(row.VoucherQuantity, 2) ||
                Math.Round(row.VoucherQuantity, 2) != Math.Round(row.StockTransactionQuantity, 2))
            {
                row.StatusCode = InvoiceStockReconciliationStatusCodes.QuantityMismatch;
                row.Notes = "يوجد فرق بين كميات الفاتورة والإذن وحركة المخزون.";
                return;
            }

            row.StatusCode = InvoiceStockReconciliationStatusCodes.Matched;
            row.Notes = "مطابقة بين الفاتورة والإذن وحركة المخزون.";
        }
    }
}
