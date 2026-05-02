using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Item
{
    public class Item
    {
        public Guid Id { get; set; }
        public OpticalItemType OpticalItemType { get; set; } = OpticalItemType.General;

        public string? BrandName { get; set; }
        public string? ModelName { get; set; }
        public string? Color { get; set; }

        public decimal? EyeSize { get; set; }
        public decimal? BridgeSize { get; set; }
        public decimal? TempleLength { get; set; }

        public string? LensMaterial { get; set; }
        public string? LensIndex { get; set; }
        public string? LensCoating { get; set; }

        public bool RequiresPrescription { get; set; } = false;
        
        public string ItemCode { get; set; } = string.Empty;
        public string ItemNameAr { get; set; } = string.Empty;
        public string? ItemNameEn { get; set; }

        public string? Barcode { get; set; }

        public Guid ItemGroupId { get; set; }
        public ItemGroup? ItemGroup { get; set; }

        public Guid UnitId { get; set; }
        public Unit? Unit { get; set; }

        public bool IsService { get; set; } = false;   // خدمة أم صنف مخزني
        public bool IsStockItem { get; set; } = true;  // هل يدخل المخزون
        public bool IsActive { get; set; } = true;

        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }

        public decimal MinimumQuantity { get; set; } = 0;
        public decimal ReorderQuantity { get; set; } = 0;

        public bool IsTaxable { get; set; } = true;
        public decimal TaxRate { get; set; } = 0;

        public string? Description { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
