using System;

namespace InfrastrfuctureManagmentCore.Domains.Accounting.Reports
{
    public class GeneralLedgerSummaryRow
    {
        public Guid AccountId { get; set; }
        public string AccountCode { get; set; } = string.Empty;
        public string AccountNameAr { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        public decimal OpeningBalance { get; set; }
        public decimal PeriodDebit { get; set; }
        public decimal PeriodCredit { get; set; }
        public decimal ClosingBalance { get; set; }
    }
}
