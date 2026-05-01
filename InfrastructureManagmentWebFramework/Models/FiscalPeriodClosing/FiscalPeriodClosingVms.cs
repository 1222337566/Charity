using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace InfrastructureManagmentWebFramework.Models.FiscalPeriodClosing
{
    public class FiscalPeriodClosingIndexVm
    {
        public List<FiscalPeriodClosingRowVm> Periods { get; set; } = new();
    }

    public class FiscalPeriodClosingRowVm
    {
        public Guid Id { get; set; }

        public string PeriodName { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsClosed { get; set; }

        public DateTime? ClosedAtUtc { get; set; }

        public string? ClosingNotes { get; set; }

        public int JournalEntriesCount { get; set; }

        public decimal TotalDebit { get; set; }

        public decimal TotalCredit { get; set; }

        public decimal Difference => TotalDebit - TotalCredit;
    }

    public class FiscalPeriodClosingDetailsVm
    {
        public Guid FiscalPeriodId { get; set; }

        public string PeriodName { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsClosed { get; set; }

        public DateTime? ClosedAtUtc { get; set; }

        public string? ClosedByUserId { get; set; }

        public string? ClosingNotes { get; set; }

        public int JournalEntriesCount { get; set; }

        public int PostedEntriesCount { get; set; }

        public int DraftEntriesCount { get; set; }

        public decimal TotalDebit { get; set; }

        public decimal TotalCredit { get; set; }

        public decimal Difference => TotalDebit - TotalCredit;

        public bool CanClose => !IsClosed && Difference == 0 && DraftEntriesCount == 0;

        public List<FiscalPeriodClosingEntryRowVm> Entries { get; set; } = new();
    }

    public class FiscalPeriodClosingEntryRowVm
    {
        public Guid Id { get; set; }

        public string EntryNumber { get; set; } = string.Empty;

        public DateTime EntryDateUtc { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string? SourceType { get; set; }

        public string? SourceNumber { get; set; }

        public decimal TotalDebit { get; set; }

        public decimal TotalCredit { get; set; }

        public decimal Difference => TotalDebit - TotalCredit;
    }

    public class CloseFiscalPeriodVm
    {
        [Required]
        public Guid FiscalPeriodId { get; set; }

        [Display(Name = "ملاحظات الإقفال")]
        [MaxLength(1000)]
        public string? ClosingNotes { get; set; }
    }
}
