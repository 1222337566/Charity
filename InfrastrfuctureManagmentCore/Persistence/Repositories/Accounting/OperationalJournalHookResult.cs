using System;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting
{
    public class OperationalJournalHookResult
    {
        public bool IsSuccess { get; set; }
        public bool IsAlreadyExists { get; set; }
        public Guid? JournalEntryId { get; set; }
        public string Message { get; set; } = string.Empty;

        public static OperationalJournalHookResult Created(Guid journalEntryId, string message) => new()
        {
            IsSuccess = true,
            JournalEntryId = journalEntryId,
            Message = message
        };

        public static OperationalJournalHookResult AlreadyExists(Guid journalEntryId, string message) => new()
        {
            IsSuccess = true,
            IsAlreadyExists = true,
            JournalEntryId = journalEntryId,
            Message = message
        };

        public static OperationalJournalHookResult Failed(string message) => new()
        {
            IsSuccess = false,
            Message = message
        };
    }
}
