using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.Volunteers
{
    public class CreateVolunteerVm
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "كود المتطوع")]
        public string VolunteerCode { get; set; } = string.Empty;

        [Required]
        [Display(Name = "الاسم")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "المؤهل")]
        public string? Qualification { get; set; }

        [Display(Name = "العنوان")]
        public string? AddressLine { get; set; }

        [Display(Name = "الجنسية")]
        public string? Nationality { get; set; }

        [Display(Name = "رقم البطاقة")]
        public string? NationalId { get; set; }

        [Display(Name = "رقم الهاتف")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "البريد الإلكتروني")]
        [EmailAddress]
        public string? Email { get; set; }

        [Display(Name = "تاريخ الميلاد")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "النوع")]
        public string? Gender { get; set; }

        [Display(Name = "الحالة الاجتماعية")]
        public string? MaritalStatus { get; set; }

        [Display(Name = "مجال التطوع المفضل")]
        public string? PreferredArea { get; set; }

        [Display(Name = "التوفر / المواعيد")]
        public string? AvailabilityNotes { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> Genders { get; set; } = new();
        public List<SelectListItem> MaritalStatuses { get; set; } = new();
    }
}
