using InfrastrfuctureManagmentCore.Domains.Customer;
using System.ComponentModel.DataAnnotations;
namespace InfrastrfuctureManagmentCore.Domains.Customer
{
    public class CreateCustomerVm
    {
        [Required]
        [Display(Name = "رقم العميل")]
        public string CustomerNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "الاسم")]
        public string NameAr { get; set; } = string.Empty;

        [Display(Name = "الاسم الإنجليزي")]
        public string? NameEn { get; set; }

        [Required]
        [Display(Name = "النوع")]
        public CustomerGender Gender { get; set; }

        [Display(Name = "السن")]
        [Range(0, 120)]
        public int? Age { get; set; }

        [Display(Name = "تليفون")]
        public string? Tel { get; set; }

        [Display(Name = "موبايل")]
        public string? MobileNo { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Remarks { get; set; }
    }
}