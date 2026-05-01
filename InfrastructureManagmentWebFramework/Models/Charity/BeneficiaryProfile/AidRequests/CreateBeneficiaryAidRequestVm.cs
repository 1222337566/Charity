using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.AidRequests
{
    public class CreateBeneficiaryAidRequestVm
    {
        [Required]
        [Display(Name = "المستفيد")]
        public Guid BeneficiaryId { get; set; }

        public List<SelectListItem> Beneficiaries { get; set; } = new();

        [Required(ErrorMessage = "تاريخ الطلب مطلوب")]
        [Display(Name = "تاريخ الطلب")]
        [DataType(DataType.Date)]
        public DateTime RequestDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "نوع المساعدة مطلوب")]
        [Display(Name = "نوع المساعدة")]
        public Guid AidTypeId { get; set; }

        [Display(Name = "القيمة المطلوبة")]
        public decimal? RequestedAmount { get; set; }

        [Display(Name = "المشروع المرتبط")]
        public Guid? ProjectId { get; set; }

        [Display(Name = "سبب الطلب")]
        public string? Reason { get; set; }

        [Display(Name = "درجة الاستعجال")]
        public string? UrgencyLevel { get; set; }

        [Display(Name = "الحالة")]
        public string Status { get; set; } = "Pending";

        [Display(Name = "تفاصيل الأصناف / الخدمات المطلوبة")]
        public List<BeneficiaryAidRequestLineVm> Lines { get; set; } = new();

        public List<SelectListItem> AidTypes { get; set; } = new();
        public List<SelectListItem> Statuses { get; set; } = new();
        public List<SelectListItem> Projects { get; set; } = new();
        public List<SelectListItem> Items { get; set; } = new();
        public List<SelectListItem> Warehouses { get; set; } = new();
        public List<SelectListItem> FulfillmentMethods { get; set; } = BeneficiaryAidRequestFulfillmentMethodOption.Items();
    }

    public class BeneficiaryAidRequestLineVm
    {
        public Guid? Id { get; set; }

        [Display(Name = "الصنف المخزني")]
        public Guid? ItemId { get; set; }

        [Display(Name = "صنف / خدمة غير مخزنية")]
        [StringLength(250)]
        public string? ItemNameSnapshot { get; set; }

        [Display(Name = "الوصف")]
        [StringLength(1000)]
        public string? Description { get; set; }

        [Display(Name = "الكمية المطلوبة")]
        [Range(0, 999999999)]
        public decimal? RequestedQuantity { get; set; }

        [Display(Name = "الكمية المعتمدة")]
        [Range(0, 999999999)]
        public decimal? ApprovedQuantity { get; set; }

        [Display(Name = "القيمة التقديرية للوحدة")]
        [Range(0, 999999999)]
        public decimal? EstimatedUnitValue { get; set; }

        [Display(Name = "الإجمالي التقديري")]
        [Range(0, 999999999)]
        public decimal? EstimatedTotalValue { get; set; }

        [Display(Name = "طريقة التنفيذ")]
        public string FulfillmentMethod { get; set; } = BeneficiaryAidRequestFulfillmentMethodOption.CashEquivalent;

        [Display(Name = "المخزن المقترح")]
        public Guid? WarehouseId { get; set; }

        [Display(Name = "الحالة")]
        public string Status { get; set; } = "Pending";

        [Display(Name = "ملاحظات")]
        [StringLength(1000)]
        public string? Notes { get; set; }
    }

    public static class BeneficiaryAidRequestFulfillmentMethodOption
    {
        public const string InKindFromStock = "InKindFromStock";
        public const string CashEquivalent = "CashEquivalent";
        public const string PurchaseNeedRequest = "PurchaseNeedRequest";
        public const string VendorPayment = "VendorPayment";

        public static List<SelectListItem> Items() => new()
        {
            new("صرف عيني من المخزن", InKindFromStock),
            new("صرف نقدي بالقيمة", CashEquivalent),
            new("طلب احتياج للشراء", PurchaseNeedRequest),
            new("دفع لمورد / جهة خدمة", VendorPayment)
        };

        public static string ToDisplayName(string? code) => code switch
        {
            InKindFromStock => "صرف عيني من المخزن",
            CashEquivalent => "صرف نقدي بالقيمة",
            PurchaseNeedRequest => "طلب احتياج للشراء",
            VendorPayment => "دفع لمورد / جهة خدمة",
            _ => "غير محدد"
        };
    }
}
