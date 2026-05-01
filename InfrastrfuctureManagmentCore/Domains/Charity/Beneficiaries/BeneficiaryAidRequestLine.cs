using InfrastrfuctureManagmentCore.Domains.Item;
using InfrastrfuctureManagmentCore.Domains.Warehouses;

namespace InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries
{
    public class BeneficiaryAidRequestLine
    {
        public Guid Id { get; set; }

        public Guid AidRequestId { get; set; }
        public BeneficiaryAidRequest? AidRequest { get; set; }

        public Guid? ItemId { get; set; }
        public InfrastrfuctureManagmentCore.Domains.Item.Item? Item { get; set; }

        public string? ItemNameSnapshot { get; set; }
        public string? Description { get; set; }

        public decimal? RequestedQuantity { get; set; }
        public decimal? ApprovedQuantity { get; set; }

        public decimal? EstimatedUnitValue { get; set; }
        public decimal? EstimatedTotalValue { get; set; }

        public string FulfillmentMethod { get; set; } = "CashEquivalent"; // InKindFromStock | CashEquivalent | PurchaseNeedRequest | VendorPayment

        public Guid? WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        public string Status { get; set; } = "Pending";
        public string? Notes { get; set; }

        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public string? UpdatedByUserId { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
