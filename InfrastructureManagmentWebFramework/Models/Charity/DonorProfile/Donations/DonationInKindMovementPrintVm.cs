namespace InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations
{
    public class DonationInKindMovementPrintVm
    {
        public Guid Id { get; set; }
        public string DonationNumber { get; set; } = string.Empty;
        public string? ReceiptNumber { get; set; }
        public DateTime DonationDate { get; set; }
        public string DonationType { get; set; } = string.Empty;
        public string? AidTypeName { get; set; }
        public string? DonorName { get; set; }
        public string? DonorCode { get; set; }
        public string? DonorPhone { get; set; }
        public string? Notes { get; set; }

        public List<DonationInKindMovementItemPrintVm> Items { get; set; } = new();
        public List<DonationInKindMovementStoreReceiptPrintVm> StoreReceipts { get; set; } = new();
        public List<DonationInKindMovementStoreIssuePrintVm> StoreIssues { get; set; } = new();

        public decimal EstimatedGrandTotal => Items.Sum(x => x.EstimatedTotalValue ?? 0m);
        public decimal ReceivedGrandQuantity => Items.Sum(x => x.ReceivedQuantity);
        public decimal QuantityGrandTotal => Items.Sum(x => x.Quantity);
        public bool IsInKind => string.Equals(DonationType, "عيني", StringComparison.OrdinalIgnoreCase)
                                || string.Equals(DonationType, "InKind", StringComparison.OrdinalIgnoreCase);
    }

    public class DonationInKindMovementItemPrintVm
    {
        public Guid DonationInKindItemId { get; set; }
        public Guid ItemId { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string? WarehouseName { get; set; }
        public decimal Quantity { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public decimal? EstimatedUnitValue { get; set; }
        public decimal? EstimatedTotalValue { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? BatchNo { get; set; }
        public string? Notes { get; set; }
    }

    public class DonationInKindMovementStoreReceiptPrintVm
    {
        public Guid Id { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public DateTime ReceiptDate { get; set; }
        public string? WarehouseName { get; set; }
        public string? SourceType { get; set; }
        public string? SourceName { get; set; }
        public int LinesCount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class DonationInKindMovementStoreIssuePrintVm
    {
        public Guid Id { get; set; }
        public string IssueNumber { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public string? WarehouseName { get; set; }
        public string? IssueType { get; set; }
        public string? BeneficiaryName { get; set; }
        public string? ProjectName { get; set; }
        public string? IssuedToName { get; set; }
        public int LinesCount { get; set; }
        public decimal TotalAmount { get; set; }

        public string DestinationName => !string.IsNullOrWhiteSpace(BeneficiaryName) ? BeneficiaryName!
            : !string.IsNullOrWhiteSpace(ProjectName) ? ProjectName!
            : (IssuedToName ?? string.Empty);
    }
}
