using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Optics
{
    using System.ComponentModel.DataAnnotations;

    public class EditSupplierVm
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "رقم المورد")]
        public string SupplierNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "اسم المورد")]
        public string NameAr { get; set; } = string.Empty;

        [Display(Name = "الاسم الإنجليزي")]
        public string? NameEn { get; set; }

        [Display(Name = "الشخص المسؤول")]
        public string? ContactPerson { get; set; }

        [Display(Name = "تليفون")]
        public string? Tel { get; set; }

        [Display(Name = "موبايل")]
        public string? MobileNo { get; set; }

        [Display(Name = "العنوان")]
        public string? Address { get; set; }

        [Display(Name = "الرقم الضريبي")]
        public string? TaxNumber { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Remarks { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; }
    }
}
