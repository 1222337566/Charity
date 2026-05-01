using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Company
{
    using System.ComponentModel.DataAnnotations;

    public class CompanyProfileVm
    {
        public Guid? Id { get; set; }

        [Required]
        [Display(Name = "اسم الشركة")]
        public string CompanyNameAr { get; set; } = string.Empty;

        [Display(Name = "الاسم الإنجليزي")]
        public string? CompanyNameEn { get; set; }

        [Display(Name = "الهاتف")]
        public string? Phone { get; set; }

        [Display(Name = "العنوان")]
        public string? Address { get; set; }

        [Display(Name = "الرقم الضريبي")]
        public string? TaxNumber { get; set; }

        [Display(Name = "سطر أعلى الإيصال")]
        public string? ReceiptHeaderText { get; set; }

        [Display(Name = "سطر أسفل الإيصال")]
        public string? ReceiptFooterText { get; set; }
    }
}
