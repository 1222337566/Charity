using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations
{
    /// <summary>
    /// ViewModel للتخصيص العام للتبرعات العينية
    /// يتيح توزيع أصناف التبرعات العينية المتاحة على طلبات المساعدة المعتمدة
    /// </summary>
    public class InKindGeneralAllocationVm
    {
        // ── فلاتر البحث ──────────────────────────
        [Display(Name = "نوع المساعدة")]
        public Guid? AidTypeId { get; set; }

        [Display(Name = "الصنف")]
        public Guid? ItemId { get; set; }

        [Display(Name = "المخزن")]
        public Guid? WarehouseId { get; set; }

        // ── بيانات النموذج ────────────────────────
        [Display(Name = "تاريخ التخصيص")]
        [DataType(DataType.Date)]
        public DateTime AllocatedDate { get; set; } = DateTime.Today;

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        // ── البيانات المحملة ──────────────────────
        public List<InKindSourceItemVm>    SourceItems { get; set; } = new();
        public List<InKindAllocationRowVm> Rows        { get; set; } = new();

        // ── Dropdowns ─────────────────────────────
        public List<SelectListItem> AidTypes   { get; set; } = new();
        public List<SelectListItem> Items      { get; set; } = new();
        public List<SelectListItem> Warehouses { get; set; } = new();
    }

    /// <summary>
    /// صنف تبرع عيني متاح للتخصيص
    /// </summary>
    public class InKindSourceItemVm
    {
        public Guid   DonationInKindItemId { get; set; }
        public string DonationNumber       { get; set; } = string.Empty;
        public string DonorName            { get; set; } = string.Empty;
        public string ItemName             { get; set; } = string.Empty;
        public decimal TotalQuantity       { get; set; }
        public decimal AllocatedQuantity   { get; set; }
        public decimal RemainingQuantity   { get; set; }
        public string? WarehouseName       { get; set; }
        public DateTime? ExpiryDate        { get; set; }
    }

    /// <summary>
    /// صف طلب مساعدة يحتاج تخصيص عيني
    /// </summary>
    public class InKindAllocationRowVm
    {
        public Guid   AidRequestId         { get; set; }
        public Guid   BeneficiaryId        { get; set; }
        public Guid?  AidRequestLineId     { get; set; }
        public string BeneficiaryName      { get; set; } = string.Empty;
        public string RequestSummary       { get; set; } = string.Empty;
        public string ItemName             { get; set; } = string.Empty;
        public decimal RequestedQuantity   { get; set; }
        public decimal AllocatedQuantity   { get; set; }
        public decimal RemainingNeedQuantity { get; set; }
        public bool   HasPreviousDisbursement { get; set; }

        // حقول النموذج
        public Guid?   SelectedDonationInKindItemId { get; set; }
        public decimal AllocateQuantity { get; set; }
    }
}
