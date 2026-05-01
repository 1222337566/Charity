using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics
{
    public class CustomerAccountEntryVm
    {
        public DateTime TransactionDateUtc { get; set; }
        public string TransactionTypeText { get; set; } = string.Empty;
        public string? ReferenceNumber { get; set; }
        public string? Description { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal RunningBalance { get; set; }
    }
}
