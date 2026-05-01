using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Customer
{
    public class CustomerAccountTransaction
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }
        public CustomerClient? Customer { get; set; }

        public DateTime TransactionDateUtc { get; set; } = DateTime.UtcNow;

        public CustomerAccountTransactionType TransactionType { get; set; }

        public string? ReferenceType { get; set; }
        public Guid? ReferenceId { get; set; }
        public string? ReferenceNumber { get; set; }

        public string? Description { get; set; }

        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
