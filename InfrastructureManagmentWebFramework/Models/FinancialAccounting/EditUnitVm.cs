using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.FinancialAccounting
{
    
    public class EditUnitVm
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "كود الوحدة")]
        public string UnitCode { get; set; } = string.Empty;

        [Required]
        [Display(Name = "اسم الوحدة")]
        public string UnitNameAr { get; set; } = string.Empty;

        [Display(Name = "الاسم الإنجليزي")]
        public string? UnitNameEn { get; set; }

        [Display(Name = "الرمز")]
        public string? Symbol { get; set; }

        [Display(Name = "نشطة")]
        public bool IsActive { get; set; }
    }
   
}
