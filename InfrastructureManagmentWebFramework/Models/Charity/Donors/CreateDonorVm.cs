using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.Donors
{
    public class CreateDonorVm
    {
        [Required]
        [Display(Name = "كود المتبرع")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Display(Name = "نوع المتبرع")]
        public string DonorType { get; set; } = "فرد";

        [Required]
        [Display(Name = "الاسم / اسم الجهة")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "مسؤول التواصل")]
        public string? ContactPerson { get; set; }

        [Display(Name = "الرقم القومي / الرقم الضريبي")]
        public string? NationalIdOrTaxNo { get; set; }

        [Display(Name = "رقم الهاتف")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "البريد الإلكتروني")]
        [EmailAddress]
        public string? Email { get; set; }

        [Display(Name = "العنوان")]
        public string? AddressLine { get; set; }

        [Display(Name = "المحافظة")]
        public Guid? GovernorateId { get; set; }

        [Display(Name = "المدينة / المركز")]
        public Guid? CityId { get; set; }

        [Display(Name = "المنطقة / القرية")]
        public Guid? AreaId { get; set; }

        [Display(Name = "وسيلة التواصل المفضلة")]
        public string? PreferredCommunicationMethod { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> DonorTypes { get; set; } = new();
        public List<SelectListItem> Governorates { get; set; } = new();
        public List<SelectListItem> Cities { get; set; } = new();
        public List<SelectListItem> Areas { get; set; } = new();
        public List<SelectListItem> CommunicationMethods { get; set; } = new();
    }
}
