using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Expenses
{
    public class ExpenseCategory
    {
        public Guid Id { get; set; }

        public string CategoryCode { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string? CategoryNameEn { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
     
    }
}
