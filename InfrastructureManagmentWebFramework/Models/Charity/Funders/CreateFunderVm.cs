using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.Funders
{
    public class CreateFunderVm
    {
        [Display(Name = "الكود")]
        [Required]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "نوع الممول")]
        [Required]
        public string FunderType { get; set; } = string.Empty;

        [Display(Name = "اسم الجهة")]
        [Required]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "مسؤول التواصل")]
        public string? ContactPerson { get; set; }

        [Display(Name = "رقم الهاتف")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "البريد الإلكتروني")]
        [EmailAddress]
        public string? Email { get; set; }

        [Display(Name = "العنوان")]
        public string? AddressLine { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; } = true;

        public List<SelectListItem> FunderTypes { get; set; } = new();
    }
}
