using InfrastrfuctureManagmentCore.Domains.Financial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Accounting.Treasury
{
    public class TreasuryBankTransfer
    {
        public Guid Id { get; set; }

        public string TransferNumber { get; set; } = string.Empty;
        public DateTime TransferDate { get; set; } = DateTime.Today;

        public Guid FromFinancialAccountId { get; set; }
        public FinancialAccount? FromFinancialAccount { get; set; }

        public Guid ToFinancialAccountId { get; set; }
        public FinancialAccount? ToFinancialAccount { get; set; }

        public decimal Amount { get; set; }
        public TreasuryBankTransferStatus Status { get; set; } = TreasuryBankTransferStatus.Posted;

        public string Description { get; set; } = string.Empty;
        public string? Notes { get; set; }

        public Guid? JournalEntryId { get; set; }
        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public string? UpdatedByUserId { get; set; }
        public string? UpdatedByUserName { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
