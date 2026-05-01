using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.FinancialAccounting
{
    public class WarehouseListItemVm
    {
        public Guid Id { get; set; }
        public string WarehouseCode { get; set; } = string.Empty;
        public string WarehouseNameAr { get; set; } = string.Empty;
        public string? WarehouseNameEn { get; set; }
        public string? Address { get; set; }
        public bool IsMain { get; set; }
        public bool IsActive { get; set; }
    }
}
