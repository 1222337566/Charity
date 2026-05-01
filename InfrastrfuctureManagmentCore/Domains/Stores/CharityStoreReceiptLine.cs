using InfrastrfuctureManagmentCore.Domains.Item;

namespace InfrastrfuctureManagmentCore.Domains.Charity.Stores
{
    public class CharityStoreReceiptLine
    {
        public Guid Id { get; set; }
        public Guid ReceiptId { get; set; }
        public CharityStoreReceipt? Receipt { get; set; }

        public Guid ItemId { get; set; }
        public InfrastrfuctureManagmentCore.Domains.Item.Item? Item { get; set; }

        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? BatchNo { get; set; }
        public string? Notes { get; set; }
    }
}
