using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.FinancialAccounting
{
    using InfrastrfuctureManagmentCore.Domains.Item;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.ComponentModel.DataAnnotations;

    public class CreateItemVm
    {
        [Required]
        [Display(Name = "كود الصنف")]
        public string ItemCode { get; set; } = string.Empty;

        [Required]
        [Display(Name = "اسم الصنف")]
        public string ItemNameAr { get; set; } = string.Empty;

        [Display(Name = "الاسم الإنجليزي")]
        public string? ItemNameEn { get; set; }

        [Display(Name = "الباركود")]
        public string? Barcode { get; set; }

        [Required]
        [Display(Name = "المجموعة")]
        public Guid ItemGroupId { get; set; }

        [Required]
        [Display(Name = "الوحدة")]
        public Guid UnitId { get; set; }

        [Display(Name = "خدمة")]
        public bool IsService { get; set; }

        [Display(Name = "صنف مخزني")]
        public bool IsStockItem { get; set; } = true;

        [Range(0, double.MaxValue)]
        [Display(Name = "سعر الشراء")]
        public decimal PurchasePrice { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "سعر البيع")]
        public decimal SalePrice { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "الحد الأدنى")]
        public decimal MinimumQuantity { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "حد إعادة الطلب")]
        public decimal ReorderQuantity { get; set; }

        [Display(Name = "خاضع للضريبة")]
        public bool IsTaxable { get; set; } = true;

        [Range(0, 100)]
        [Display(Name = "نسبة الضريبة")]
        public decimal TaxRate { get; set; }

        [Display(Name = "الوصف")]
        public string? Description { get; set; }
        [Display(Name = "نوع الصنف البصري")]
        public OpticalItemType OpticalItemType { get; set; } = OpticalItemType.General;

        [Display(Name = "الماركة")]
        public string? BrandName { get; set; }

        [Display(Name = "الموديل")]
        public string? ModelName { get; set; }

        [Display(Name = "اللون")]
        public string? Color { get; set; }

        [Display(Name = "Eye Size")]
        public decimal? EyeSize { get; set; }

        [Display(Name = "Bridge Size")]
        public decimal? BridgeSize { get; set; }

        [Display(Name = "Temple Length")]
        public decimal? TempleLength { get; set; }

        [Display(Name = "Lens Material")]
        public string? LensMaterial { get; set; }

        [Display(Name = "Lens Index")]
        public string? LensIndex { get; set; }

        [Display(Name = "Lens Coating")]
        public string? LensCoating { get; set; }

        [Display(Name = "يحتاج Prescription")]
        public bool RequiresPrescription { get; set; }
        public List<SelectListItem> ItemGroups { get; set; } = new();
        public List<SelectListItem> Units { get; set; } = new();
    }
}
