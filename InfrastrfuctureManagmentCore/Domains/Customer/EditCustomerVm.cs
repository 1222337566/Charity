using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Customer
{
    using System.ComponentModel.DataAnnotations;

    public class EditCustomerVm
    {
        public Guid Id { get; set; }

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

        [Display(Name = "نشط")]
        public bool IsActive { get; set; }
    }
}
