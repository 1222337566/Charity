using InfrastrfuctureManagmentCore.Domains.Financial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Accounting.Treasury
{
    public class CashBankVoucher
    {
        public Guid Id { get; set; }

        public string VoucherNumber { get; set; } = string.Empty;
        public DateTime VoucherDate { get; set; } = DateTime.Today;

        public CashBankVoucherType VoucherType { get; set; }
        public CashBankVoucherStatus Status { get; set; } = CashBankVoucherStatus.Posted;

        public Guid FinancialAccountId { get; set; }
        public FinancialAccount? FinancialAccount { get; set; }

        public Guid OppositeAccountId { get; set; }
        public FinancialAccount? OppositeAccount { get; set; }

        public decimal Amount { get; set; }
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
