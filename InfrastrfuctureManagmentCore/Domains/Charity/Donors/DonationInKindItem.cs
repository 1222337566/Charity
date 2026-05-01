using InfrastrfuctureManagmentCore.Domains.Item;
using InfrastrfuctureManagmentCore.Domains.Warehouses;

namespace InfrastrfuctureManagmentCore.Domains.Charity.Donors
{
    public class DonationInKindItem
    {
        public Guid Id { get; set; }

        public Guid DonationId { get; set; }
        public Donation? Donation { get; set; }

        public Guid ItemId { get; set; }
        public InfrastrfuctureManagmentCore.Domains.Item.Item? Item { get; set; }

        public decimal Quantity { get; set; }
        public decimal? EstimatedUnitValue { get; set; }
        public decimal? EstimatedTotalValue { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public string? BatchNo { get; set; }

        public Guid? WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        public string? Notes { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
