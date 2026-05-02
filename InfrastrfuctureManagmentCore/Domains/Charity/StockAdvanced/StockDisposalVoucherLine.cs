using System;

namespace InfrastrfuctureManagmentCore.Domains.Charity.StockAdvanced
{
    public class StockDisposalVoucherLine
    {
        public Guid Id { get; set; }
        public Guid StockDisposalVoucherId { get; set; }
        public StockDisposalVoucher? StockDisposalVoucher { get; set; }
        public Guid ItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public string? Notes { get; set; }
    }
}
