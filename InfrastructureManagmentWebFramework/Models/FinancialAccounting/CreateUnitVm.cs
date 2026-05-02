using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace InfrastructureManagmentWebFramework.Models.FinancialAccounting
{
    
    public class CreateUnitVm
    {
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
    }
}
