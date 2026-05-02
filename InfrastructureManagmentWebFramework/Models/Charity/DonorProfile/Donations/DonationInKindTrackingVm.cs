namespace InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations
{
    public class DonationInKindTrackingVm
    {
        public Guid DonationId { get; set; }
        public string DonationNumber { get; set; } = string.Empty;
        public DateTime DonationDate { get; set; }
        public string DonationType { get; set; } = string.Empty;
        public string? DonorName { get; set; }
        public string? AidTypeName { get; set; }
        public decimal DonationAmount { get; set; }
        public int ItemsCount { get; set; }
        public decimal TotalQuantity { get; set; }
        public int StoreReceiptCount { get; set; }
        public int StoreIssueCount { get; set; }
        public decimal ReceivedToStoreQuantity { get; set; }
        public decimal IssuedQuantity { get; set; }
        public decimal RemainingToIssueQuantity { get; set; }
        public string TrackingStatusCode { get; set; } = "NotReceivedToStore";
        public string TrackingStatusName { get; set; } = "لم يدخل المخزن";
        public List<DonationInKindTrackingItemVm> Items { get; set; } = new();
        public List<DonationInKindTrackingDocumentVm> StoreReceipts { get; set; } = new();
        public List<DonationInKindTrackingDocumentVm> StoreIssues { get; set; } = new();
    }

    public class DonationInKindTrackingItemVm
    {
        public Guid DonationInKindItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? WarehouseName { get; set; }
        public string? BatchNo { get; set; }
        public decimal Quantity { get; set; }
        public decimal AllocatedQuantity { get; set; }
        public decimal RemainingAllocationQuantity { get; set; }
        public int StoreReceiptCount { get; set; }
        public int StoreIssueCount { get; set; }
        public decimal ReceivedToStoreQuantity { get; set; }
        public decimal IssuedQuantity { get; set; }
        public decimal RemainingToIssueQuantity { get; set; }
        public string TrackingStatusCode { get; set; } = "NotReceivedToStore";
        public string TrackingStatusName { get; set; } = "لم يدخل المخزن";
    }

    public class DonationInKindTrackingDocumentVm
    {
        public Guid Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public DateTime DocumentDate { get; set; }
        public string? WarehouseName { get; set; }
        public string? BeneficiaryName { get; set; }
        public decimal TotalQuantity { get; set; }
        public string? Notes { get; set; }
    }
}
