using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.FinancialAccounting
{
    public class ItemGroupListItemVm
    {
        public Guid Id { get; set; }
        public string GroupCode { get; set; } = string.Empty;
        public string GroupNameAr { get; set; } = string.Empty;
        public string? GroupNameEn { get; set; }
        public Guid? ParentGroupId { get; set; }
        public string? ParentGroupName { get; set; }
        public bool IsActive { get; set; }
    }
}
