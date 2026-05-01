using System;
using InfrastrfuctureManagmentCore.Domains.Financial;

namespace InfrastrfuctureManagmentCore.Domains.Accounting
{
    public class JournalEntryLine
    {
        public Guid Id { get; set; }

        public Guid JournalEntryId { get; set; }
        public JournalEntry? JournalEntry { get; set; }

        public Guid FinancialAccountId { get; set; }
        public FinancialAccount? FinancialAccount { get; set; }

        public Guid? CostCenterId { get; set; }
        public CostCenter? CostCenter { get; set; }

        public Guid? ProjectId { get; set; }
        public string? Description { get; set; }

        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
