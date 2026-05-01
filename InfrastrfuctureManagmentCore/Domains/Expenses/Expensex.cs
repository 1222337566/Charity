using InfrastrfuctureManagmentCore.Domains.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Expenses
{
    public class Expensex
    {
        public Guid Id { get; set; }

        public string ExpenseNumber { get; set; } = string.Empty;
        public DateTime ExpenseDateUtc { get; set; } = DateTime.UtcNow;

        public Guid ExpenseCategoryId { get; set; }
        public ExpenseCategory? ExpenseCategory { get; set; }

        public Guid? PaymentMethodId { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }

        public decimal Amount { get; set; }

        public string Description { get; set; } = string.Empty;
        public string? Notes { get; set; }

        public bool IsPosted { get; set; } = true;

        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public string? UpdatedByUserId { get; set; }
        public string? UpdatedByUserName { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
