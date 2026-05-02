namespace InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations
{
    public class DonationReceiptPrintVm
    {
        public Guid DonationId { get; set; }
        public string DonationNumber { get; set; } = string.Empty;
        public string? ReceiptNumber { get; set; }
        public DateTime DonationDate { get; set; }
        public string DonationType { get; set; } = string.Empty;
        public string? AidTypeName { get; set; }
        public decimal? Amount { get; set; }
        public string? PaymentMethodName { get; set; }
        public string? FinancialAccountName { get; set; }
        public bool IsRestricted { get; set; }
        public string? CampaignName { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Notes { get; set; }

        public string? DonorCode { get; set; }
        public string? DonorType { get; set; }
        public string DonorName { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? DonorPhoneNumber { get; set; }
        public string? DonorAddress { get; set; }

        public string CompanyNameAr { get; set; } = "اسم الجمعية";
        public string? CompanyPhone { get; set; }
        public string? CompanyAddress { get; set; }
        public string? CompanyTaxNumber { get; set; }
        public string? ReceiptHeaderText { get; set; }
        public string? ReceiptFooterText { get; set; }

        public List<DonationReceiptInKindItemVm> InKindItems { get; set; } = new();

        public bool IsInKind => string.Equals(DonationType, "عيني", StringComparison.OrdinalIgnoreCase);
        public decimal EstimatedInKindTotal => InKindItems.Sum(x => x.EstimatedTotalValue ?? 0m);
        public string ReceiptTitle => IsInKind ? "إيصال استلام تبرع عيني" : "إيصال استلام تبرع نقدي";
    }

    public class DonationReceiptInKindItemVm
    {
        public string ItemName { get; set; } = string.Empty;
        public string? WarehouseName { get; set; }
        public decimal Quantity { get; set; }
        public decimal? EstimatedUnitValue { get; set; }
        public decimal? EstimatedTotalValue { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? BatchNo { get; set; }
        public string? Notes { get; set; }
    }
}
