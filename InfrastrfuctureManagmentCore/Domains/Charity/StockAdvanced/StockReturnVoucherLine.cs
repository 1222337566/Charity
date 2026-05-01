using System;

namespace InfrastrfuctureManagmentCore.Domains.Charity.StockAdvanced
{
    public class StockReturnVoucherLine
    {
        public Guid Id { get; set; }
        public Guid StockReturnVoucherId { get; set; }
        public StockReturnVoucher? StockReturnVoucher { get; set; }
        public Guid ItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public string? Notes { get; set; }
    }
}
