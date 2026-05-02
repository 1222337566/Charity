using System;

namespace InfrastrfuctureManagmentCore.Domains.Accounting.Reports
{
    public class CostCenterLedgerEntryRow
    {
        public Guid JournalEntryId { get; set; }
        public Guid JournalEntryLineId { get; set; }
        public DateTime EntryDate { get; set; }
        public string EntryNumber { get; set; } = string.Empty;
        public string AccountCode { get; set; } = string.Empty;
        public string AccountNameAr { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? ProjectId { get; set; }
        public string? SourceType { get; set; }
        public Guid? SourceId { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal RunningBalance { get; set; }
    }
}
