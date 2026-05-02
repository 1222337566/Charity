using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Reports
{
    public class InKindReceiptStatusReportVm
    {
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        public List<InKindReceiptStatusRowVm> Rows { get; set; } = new();

        public int TotalItems => Rows.Count;
        public int ReceivedItems => Rows.Count(x => x.StoreReceiptCount > 0);
        public int PendingItems => Rows.Count(x => x.StoreReceiptCount == 0);
        public decimal TotalDonatedQuantity => Rows.Sum(x => x.DonatedQuantity);
        public decimal TotalReceivedQuantity => Rows.Sum(x => x.ReceivedQuantity);
    }

    public class InKindReceiptStatusRowVm
    {
        public Guid DonationId { get; set; }
        public Guid DonationItemId { get; set; }
        public DateTime DonationDate { get; set; }
        public string DonationNumber { get; set; } = string.Empty;
        public string DonorName { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string? WarehouseName { get; set; }
        public decimal DonatedQuantity { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public int StoreReceiptCount { get; set; }
        public string ReceiptStatusName => StoreReceiptCount > 0 ? "دخل المخزن" : "لم يدخل المخزن";
        public decimal RemainingToReceiveQuantity => Math.Max(0m, DonatedQuantity - ReceivedQuantity);
    }

    public class InKindBeneficiaryIssueReportVm
    {
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        public List<InKindBeneficiaryIssueRowVm> Rows { get; set; } = new();

        public int TotalIssues => Rows.Select(x => x.StoreIssueId).Distinct().Count();
        public decimal TotalIssuedQuantity => Rows.Sum(x => x.Quantity);
    }

    public class InKindBeneficiaryIssueRowVm
    {
        public Guid DisbursementId { get; set; }
        public Guid? StoreIssueId { get; set; }
        public DateTime DisbursementDate { get; set; }
        public string StoreIssueNumber { get; set; } = string.Empty;
        public string DonationNumber { get; set; } = string.Empty;
        public string BeneficiaryName { get; set; } = string.Empty;
        public string AidTypeName { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string? WarehouseName { get; set; }
        public decimal Quantity { get; set; }
        public string? Notes { get; set; }
    }

    public class InKindDonationBalanceReportVm
    {
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        public List<InKindDonationBalanceRowVm> Rows { get; set; } = new();

        public decimal TotalDonatedQuantity => Rows.Sum(x => x.DonatedQuantity);
        public decimal TotalAllocatedQuantity => Rows.Sum(x => x.AllocatedQuantity);
        public decimal TotalRemainingQuantity => Rows.Sum(x => x.RemainingQuantity);
    }

    public class InKindDonationBalanceRowVm
    {
        public Guid DonationId { get; set; }
        public Guid DonationItemId { get; set; }
        public DateTime DonationDate { get; set; }
        public string DonationNumber { get; set; } = string.Empty;
        public string DonorName { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string? WarehouseName { get; set; }
        public decimal DonatedQuantity { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public decimal AllocatedQuantity { get; set; }
        public decimal RemainingQuantity => Math.Max(0m, DonatedQuantity - AllocatedQuantity);
        public int StoreReceiptCount { get; set; }
        public string ReceiptStatusName => StoreReceiptCount > 0 ? "دخل المخزن" : "لم يدخل المخزن";
    }
}
