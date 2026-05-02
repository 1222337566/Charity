using InfrastrfuctureManagmentCore.Domains.Item;

namespace InfrastrfuctureManagmentCore.Domains.Charity.Stores
{
    public class CharityStoreIssueLine
    {
        public Guid Id { get; set; }
        public Guid IssueId { get; set; }
        public CharityStoreIssue? Issue { get; set; }

        public Guid ItemId { get; set; }
        public InfrastrfuctureManagmentCore.Domains.Item.Item? Item { get; set; }

        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public string? Notes { get; set; }
    }
}
