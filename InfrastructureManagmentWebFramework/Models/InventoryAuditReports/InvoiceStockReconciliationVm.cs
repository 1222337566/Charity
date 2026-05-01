using System;
using System.Collections.Generic;
using System.Linq;

namespace InfrastructureManagmentWebFramework.Models.InventoryAuditReports
{
    public class InvoiceStockReconciliationVm
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public List<InvoiceStockReconciliationRowVm> PurchaseRows { get; set; } = new();
        public List<InvoiceStockReconciliationRowVm> SalesRows { get; set; } = new();

        public int PurchaseMatchedCount => PurchaseRows.Count(x => x.StatusCode == InvoiceStockReconciliationStatusCodes.Matched);
        public int PurchaseIssueCount => PurchaseRows.Count(x => x.StatusCode != InvoiceStockReconciliationStatusCodes.Matched);

        public int SalesMatchedCount => SalesRows.Count(x => x.StatusCode == InvoiceStockReconciliationStatusCodes.Matched);
        public int SalesIssueCount => SalesRows.Count(x => x.StatusCode != InvoiceStockReconciliationStatusCodes.Matched);

        public int TotalIssueCount => PurchaseIssueCount + SalesIssueCount;
    }

    public class InvoiceStockReconciliationRowVm
    {
        public Guid InvoiceId { get; set; }
        public string InvoiceType { get; set; } = string.Empty;
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDateUtc { get; set; }
        public string PartyName { get; set; } = string.Empty;
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;

        public decimal InvoiceQuantity { get; set; }
        public decimal VoucherQuantity { get; set; }
        public decimal StockTransactionQuantity { get; set; }

        public Guid? VoucherId { get; set; }
        public string? VoucherNumber { get; set; }
        public string? VoucherApprovalStatus { get; set; }

        public string StatusCode { get; set; } = InvoiceStockReconciliationStatusCodes.MissingVoucher;
        public string StatusNameAr => InvoiceStockReconciliationStatusCodes.ToArabic(StatusCode);
        public string StatusCssClass => InvoiceStockReconciliationStatusCodes.ToCss(StatusCode);

        public string? Notes { get; set; }
    }

    public static class InvoiceStockReconciliationStatusCodes
    {
        public const string Matched = "Matched";
        public const string MissingVoucher = "MissingVoucher";
        public const string VoucherNotApproved = "VoucherNotApproved";
        public const string MissingStockTransaction = "MissingStockTransaction";
        public const string QuantityMismatch = "QuantityMismatch";

        public static string ToArabic(string? code)
        {
            return code switch
            {
                Matched => "مطابقة",
                MissingVoucher => "لا يوجد إذن مخزني",
                VoucherNotApproved => "الإذن غير معتمد",
                MissingStockTransaction => "لا توجد حركة مخزون",
                QuantityMismatch => "فرق في الكمية",
                _ => "غير محدد"
            };
        }

        public static string ToCss(string? code)
        {
            return code switch
            {
                Matched => "success",
                MissingVoucher => "danger",
                VoucherNotApproved => "warning",
                MissingStockTransaction => "danger",
                QuantityMismatch => "warning",
                _ => "secondary"
            };
        }
    }
}
