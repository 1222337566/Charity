using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Expenses
{
    using System.ComponentModel.DataAnnotations;

    public class EditExpenseCategoryVm
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "كود التصنيف")]
        public string CategoryCode { get; set; } = string.Empty;

        [Required]
        [Display(Name = "اسم التصنيف")]
        public string CategoryNameAr { get; set; } = string.Empty;

        [Display(Name = "الاسم الإنجليزي")]
        public string? CategoryNameEn { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; }
    }
}
