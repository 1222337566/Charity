using System;
using System.Collections.Generic;

namespace InfrastrfuctureManagmentCore.Domains.Accounting.Reports
{
    public class AccountStatementResult
    {
        public Guid AccountId { get; set; }
        public string AccountCode { get; set; } = string.Empty;
        public string AccountNameAr { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public decimal OpeningBalance { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal ClosingBalance { get; set; }

        public List<LedgerEntryRow> Rows { get; set; } = new();
    }
}
