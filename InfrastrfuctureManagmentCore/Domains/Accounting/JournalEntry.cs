using System;
using System.Collections.Generic;

namespace InfrastrfuctureManagmentCore.Domains.Accounting
{
    public class JournalEntry
    {
        public Guid Id { get; set; }
        public string EntryNumber { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? Notes { get; set; }

        public Guid FiscalPeriodId { get; set; }
        public FiscalPeriod? FiscalPeriod { get; set; }

        public JournalEntryStatus Status { get; set; } = JournalEntryStatus.Draft;

        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }

        public string? SourceType { get; set; }
        public Guid? SourceId { get; set; }
        public Guid? RelatedJournalEntryId { get; set; }

        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public string? UpdatedByUserId { get; set; }
        public string? UpdatedByUserName { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public DateTime? PostedAtUtc { get; set; }
        public string? PostedByUserId { get; set; }
        public string? PostedByUserName { get; set; }

        public ICollection<JournalEntryLine> Lines { get; set; } = new List<JournalEntryLine>();
    }
}
