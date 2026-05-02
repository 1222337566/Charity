using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfrastrfuctureManagmentCore.Domains.Financial;

namespace InfrastructureManagmentWebFramework.Models.FinancialAccounting
{
    public  class AccountListItemVm
    {
        public Guid Id { get; set; }
        public string AccountCode { get; set; } = string.Empty;
        public string AccountNameAr { get; set; } = string.Empty;
        public int Level { get; set; }
        public bool IsPosting { get; set; }
        public bool IsActive { get; set; }
        public bool RequiresProject { get; set; }
        public bool RequiresCostCenter { get; set; }
        public Guid? ParentAccountId { get; set; }
        public string Category { get; set; } = string.Empty;
        public FinancialAccountCashKind CashKind { get; set; } = FinancialAccountCashKind.None;
        public bool AllowNegativeCashBalance { get; set; }
    }
}
