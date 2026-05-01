namespace InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations
{
    public class DonationInKindItemListItemVm
    {
        public Guid Id { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal? EstimatedUnitValue { get; set; }
        public decimal? EstimatedTotalValue { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? BatchNo { get; set; }
        public string? WarehouseName { get; set; }
        public string? Notes { get; set; }
        public decimal AllocatedQuantity { get; set; }
        public decimal RemainingQuantity { get; set; }
        public int StoreReceiptCount { get; set; }
        public int StoreIssueCount { get; set; }
        public decimal ReceivedToStoreQuantity { get; set; }
        public decimal IssuedQuantity { get; set; }
        public decimal RemainingToIssueQuantity { get; set; }
        public string? TrackingStatusCode { get; set; }
        public string? TrackingStatusName { get; set; }
    }
}
