using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.Stores.Issues
{
    public class CreateStoreIssueVm
    {
        [Required]
        [Display(Name = "رقم الإذن")]
        public string IssueNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "المخزن")]
        public Guid WarehouseId { get; set; }

        [Required]
        [Display(Name = "تاريخ الصرف")]
        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "نوع الصرف")]
        public string IssueType { get; set; } = "Internal";

        [Display(Name = "المستفيد")]
        public Guid? BeneficiaryId { get; set; }

        [Display(Name = "المشروع")]
        public Guid? ProjectId { get; set; }

        [Display(Name = "مصروف إلى")]
        public string? IssuedToName { get; set; }

        [Required]
        [Display(Name = "الصنف")]
        public Guid ItemId { get; set; }

        [Range(0.01, 999999999)]
        [Display(Name = "الكمية")]
        public decimal Quantity { get; set; }

        [Range(0, 999999999)]
        [Display(Name = "التكلفة للوحدة")]
        public decimal UnitCost { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> Warehouses { get; set; } = new();
        public List<SelectListItem> Items { get; set; } = new();
        public List<SelectListItem> Beneficiaries { get; set; } = new();
        public List<SelectListItem> Projects { get; set; } = new();

        public List<SelectListItem> IssueTypes { get; } = new()
        {
            new("صرف لمستفيد", "Beneficiary"),
            new("صرف لمشروع", "Project"),
            new("صرف داخلي", "Internal"),
            new("هالك / تالف", "Damage")
        };
    }
}
