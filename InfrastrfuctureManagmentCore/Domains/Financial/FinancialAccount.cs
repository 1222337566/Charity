using InfrastructureManagmentCore.Domains.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Financial
{
    public enum AccountCategory
    {
        Asset = 1,
        Liability = 2,
        Equity = 3,
        Revenue = 4,
        Expense = 5
    }

    public enum FinancialAccountCashKind
    {
        None = 0,
        Treasury = 1,
        Bank = 2
    }
    public class FinancialAccount
    {
        public Guid Id { get; set; }

        public string AccountCode { get; set; } = string.Empty;
        public string AccountNameAr { get; set; } = string.Empty;
        public string? AccountNameEn { get; set; }

        public AccountCategory Category { get; set; }

        public Guid? ParentAccountId { get; set; }
        public FinancialAccount? ParentAccount { get; set; }

        public int Level { get; set; }

        public bool IsPosting { get; set; }
        public bool IsActive { get; set; } = true;

        public bool RequiresProject { get; set; } = false;
        public bool RequiresCostCenter { get; set; } = false;

        public FinancialAccountCashKind CashKind { get; set; } = FinancialAccountCashKind.None;
        public bool AllowNegativeCashBalance { get; set; } = false;

        public string? Notes { get; set; }

        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public string? UpdatedByUserId { get; set; }
        public string? UpdatedByUserName { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<FinancialAccount> Children { get; set; } = new List<FinancialAccount>();
    }
}
