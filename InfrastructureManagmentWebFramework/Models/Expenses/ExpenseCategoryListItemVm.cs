using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Expenses
{
    public class ExpenseCategoryListItemVm
    {
        public Guid Id { get; set; }
        public string CategoryCode { get; set; } = string.Empty;
        public string CategoryNameAr { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
