using System;

namespace InfrastructureManagmentWebFramework.Models.Accounting
{
    public class JournalEntryListItemVm
    {
        public Guid Id { get; set; }
        public string EntryNumber { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; }
        public string FiscalPeriodName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? SourceType { get; set; }
        public bool IsManual { get; set; }
        public bool IsBalanced { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public int LinesCount { get; set; }
    }
}
