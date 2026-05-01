using System;

namespace InfrastrfuctureManagmentCore.Domains.Accounting.Reports
{
    public class RevenueExpenseSummaryResult
    {
        public Guid? FiscalPeriodId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal NetSurplusOrDeficit => TotalRevenue - TotalExpense;
        public decimal TotalAssets { get; set; }
        public decimal TotalLiabilities { get; set; }
        public decimal TotalEquity { get; set; }
    }
}
