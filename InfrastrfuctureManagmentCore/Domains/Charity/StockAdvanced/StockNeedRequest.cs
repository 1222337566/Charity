using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;

namespace InfrastrfuctureManagmentCore.Domains.Charity.StockAdvanced
{
    /// <summary>
    /// طلب الاحتياج — الدورة المستندية:
    /// Draft → Submitted → Approved → Ordered → PartiallyFulfilled → Fulfilled
    /// </summary>
    public class StockNeedRequest
    {
        public Guid Id { get; set; }
        public string RequestNumber { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; } = DateTime.UtcNow.Date;
        public string RequestType { get; set; } = "Project"; // Project | Beneficiary | Operations
        public Guid? ProjectId { get; set; }
        public Guid? BeneficiaryId { get; set; }

        public Guid? BeneficiaryAidRequestId { get; set; }
        public BeneficiaryAidRequest? BeneficiaryAidRequest { get; set; }

        public Guid? BeneficiaryAidRequestLineId { get; set; }
        public BeneficiaryAidRequestLine? BeneficiaryAidRequestLine { get; set; }

        public string? AidRequestLineDescriptionSnapshot { get; set; }
        public decimal? EstimatedTotalValue { get; set; }
        public string? RequestedByName { get; set; }

        /// <summary>
        /// Draft → Submitted → Approved → Ordered → PartiallyFulfilled → Fulfilled | Rejected | Cancelled
        /// </summary>
        public string Status { get; set; } = "Draft";

        public string? Notes { get; set; }
        public string? ApprovedByUserId { get; set; }
        public string? ApprovedByUserName { get; set; }
        public DateTime? ApprovedAtUtc { get; set; }
        public string? RejectionReason { get; set; }

        /// <summary>رقم أمر / فاتورة الشراء المرتبطة</summary>
        public Guid? LinkedPurchaseInvoiceId { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public string? UpdatedByUserId { get; set; }
        public string? UpdatedByUserName { get; set; }

        public List<StockNeedRequestLine> Lines { get; set; } = new();
    }
}
