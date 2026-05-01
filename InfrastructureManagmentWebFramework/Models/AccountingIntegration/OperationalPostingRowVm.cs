namespace InfrastructureManagmentWebFramework.Models.AccountingIntegration
{
    public class OperationalPostingRowVm
    {
        public Guid SourceId { get; set; }
        public string SourceType { get; set; } = string.Empty;
        public string ReferenceNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime DocumentDate { get; set; }
        public decimal Amount { get; set; }
        public string? Status { get; set; }
        public Guid? JournalEntryId { get; set; }
        public bool HasJournalEntry => JournalEntryId.HasValue;
    }
}
