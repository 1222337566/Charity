using System;

namespace InfrastrfuctureManagmentCore.Domains.Accounting
{
    public class FiscalPeriod
    {
        public Guid Id { get; set; }
        public string PeriodCode { get; set; } = string.Empty;
        public string PeriodNameAr { get; set; } = string.Empty;
        public string? PeriodNameEn { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsClosed { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
        public bool IsActive { get; set; }
        public bool IsOpen { get; set; }
       

        public DateTime? ClosedAtUtc { get; set; }

        public string? ClosedByUserId { get; set; }

        public string? ClosingNotes { get; set; }

    }
}
