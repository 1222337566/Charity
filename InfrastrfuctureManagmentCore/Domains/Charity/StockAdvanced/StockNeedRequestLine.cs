using System;

namespace InfrastrfuctureManagmentCore.Domains.Charity.StockAdvanced
{
    public class StockNeedRequestLine
    {
        public Guid Id { get; set; }
        public Guid StockNeedRequestId { get; set; }
        public StockNeedRequest? StockNeedRequest { get; set; }
        public Guid? ItemId { get; set; }
        public string? ItemNameSnapshot { get; set; }
        public decimal? EstimatedUnitValue { get; set; }
        public decimal? EstimatedTotalValue { get; set; }
        public decimal RequestedQuantity { get; set; }
        public decimal ApprovedQuantity { get; set; }
        public decimal FulfilledQuantity { get; set; }
        public string? Notes { get; set; }
    }
}
