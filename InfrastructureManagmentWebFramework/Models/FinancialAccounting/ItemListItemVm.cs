using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.FinancialAccounting
{
    public class ItemListItemVm
    {
        public Guid Id { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string ItemNameAr { get; set; } = string.Empty;
        public string OpticalItemTypeText { get; set; } = string.Empty;
        public string? BrandName { get; set; }
        public string? ModelName { get; set; }
        public string? Color { get; set; }
        public bool RequiresPrescription { get; set; }
        public string? Barcode { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public bool IsService { get; set; }
        public bool IsStockItem { get; set; }
        public bool IsTaxable { get; set; }
        public decimal TaxRate { get; set; }
        public bool IsActive { get; set; }
    }
}
