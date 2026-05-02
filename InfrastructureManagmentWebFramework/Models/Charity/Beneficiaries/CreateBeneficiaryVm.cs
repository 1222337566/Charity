using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.Beneficiaries
{
    public class CreateBeneficiaryVm
    {
        [Required]
        [Display(Name = "كود المستفيد")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Display(Name = "الاسم بالكامل")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "الرقم القومي")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "الرقم القومي يجب أن يكون 14 رقمًا")]
        public string? NationalId { get; set; }

        [Display(Name = "تاريخ الميلاد")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "النوع")]
        public Guid? GenderId { get; set; }

        [Display(Name = "الحالة الاجتماعية")]
        public Guid? MaritalStatusId { get; set; }

        [Display(Name = "رقم الهاتف")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "رقم هاتف بديل")]
        public string? AlternatePhoneNumber { get; set; }

        [Display(Name = "العنوان")]
        public string? AddressLine { get; set; }

        [Display(Name = "المحافظة")]
        public Guid? GovernorateId { get; set; }

        [Display(Name = "المدينة / المركز")]
        public Guid? CityId { get; set; }

        [Display(Name = "المنطقة / القرية")]
        public Guid? AreaId { get; set; }

        [Display(Name = "عدد أفراد الأسرة")]
        [Range(0, 99)]
        public int FamilyMembersCount { get; set; }

        [Display(Name = "الدخل الشهري")]
        [Range(0, 999999999)]
        public decimal? MonthlyIncome { get; set; }

        [Display(Name = "مصدر الدخل")]
        public string? IncomeSource { get; set; }

        [Display(Name = "الحالة الصحية")]
        public string? HealthStatus { get; set; }

        [Display(Name = "الحالة التعليمية")]
        public string? EducationStatus { get; set; }

        [Display(Name = "حالة العمل")]
        public string? WorkStatus { get; set; }

        [Display(Name = "حالة السكن")]
        public string? HousingStatus { get; set; }

        [Display(Name = "حالة الملف")]
        public Guid? StatusId { get; set; }

        [Display(Name = "تاريخ التسجيل")]
        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; } = DateTime.Today;

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> Genders { get; set; } = new();
        public List<SelectListItem> MaritalStatuses { get; set; } = new();
        public List<SelectListItem> Governorates { get; set; } = new();
        public List<SelectListItem> Cities { get; set; } = new();
        public List<SelectListItem> Areas { get; set; } = new();
        public List<SelectListItem> Statuses { get; set; } = new();
    }
}
