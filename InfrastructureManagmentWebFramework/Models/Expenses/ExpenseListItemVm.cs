using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Expenses
{
    public class ExpenseListItemVm
    {
        public Guid Id { get; set; }
        public string ExpenseNumber { get; set; } = string.Empty;
        public DateTime ExpenseDateUtc { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? PaymentMethodName { get; set; }
        public decimal Amount { get; set; }
    }
}
