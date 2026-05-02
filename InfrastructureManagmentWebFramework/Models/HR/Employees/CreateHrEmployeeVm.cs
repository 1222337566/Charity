using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.HR.Employees
{
    public class CreateHrEmployeeVm
    {
        [Required, Display(Name = "الكود")]
        public string Code { get; set; } = string.Empty;

        [Required, Display(Name = "الاسم")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "الرقم القومي")]
        public string? NationalId { get; set; }

        [Display(Name = "تاريخ الميلاد"), DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "الهاتف")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "البريد الإلكتروني")]
        public string? Email { get; set; }

        [Display(Name = "العنوان")]
        public string? AddressLine { get; set; }

        [Display(Name = "القسم")]
        public Guid? DepartmentId { get; set; }

        [Display(Name = "المسمى الوظيفي")]
        public Guid? JobTitleId { get; set; }

        [Display(Name = "تاريخ التعيين"), DataType(DataType.Date)]
        public DateTime HireDate { get; set; } = DateTime.Today;

        [Required, Display(Name = "نوع التعيين")]
        public string EmploymentType { get; set; } = "Permanent";

        [Display(Name = "المرتب الأساسي")]
        public decimal BasicSalary { get; set; }

        [Display(Name = "مرتب التأمين")]
        public decimal? InsuranceSalary { get; set; }

        [Display(Name = "البنك")]
        public string? BankName { get; set; }

        [Display(Name = "رقم الحساب البنكي")]
        public string? BankAccountNumber { get; set; }

        [Required, Display(Name = "الحالة")]
        public string Status { get; set; } = "Active";

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; } = true;

        public List<SelectListItem> Departments { get; set; } = new();
        public List<SelectListItem> JobTitles { get; set; } = new();
        public List<SelectListItem> EmploymentTypes { get; set; } = new();
        public List<SelectListItem> Statuses { get; set; } = new();
    }
}
