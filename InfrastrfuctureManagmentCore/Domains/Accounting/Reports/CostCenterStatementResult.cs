using System;
using System.Collections.Generic;

namespace InfrastrfuctureManagmentCore.Domains.Accounting.Reports
{
    public class CostCenterStatementResult
    {
        public Guid CostCenterId { get; set; }
        public string CostCenterCode { get; set; } = string.Empty;
        public string CostCenterNameAr { get; set; } = string.Empty;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal ClosingBalance { get; set; }
        public List<CostCenterLedgerEntryRow> Rows { get; set; } = new();
    }
}
