using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.FinancialAccounting
{
    public class UnitListItemVm
    {
        public Guid Id { get; set; }
        public string UnitCode { get; set; } = string.Empty;
        public string UnitNameAr { get; set; } = string.Empty;
        public string? UnitNameEn { get; set; }
        public string? Symbol { get; set; }
        public bool IsActive { get; set; }
    }
}
