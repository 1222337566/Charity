using System;
using System.Collections.Generic;

namespace InfrastructureManagmentWebFramework.Models.Accounting
{
    public class JournalEntryDetailsVm
    {
        public Guid Id { get; set; }
        public string EntryNumber { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; }
        public string FiscalPeriodName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? SourceType { get; set; }
        public Guid? SourceId { get; set; }
        public Guid? RelatedJournalEntryId { get; set; }
        public bool IsManual { get; set; }
        public string? SourceReferenceName { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public bool IsBalanced { get; set; }
        public List<JournalEntryLineListItemVm> Lines { get; set; } = new();
    }
}
