using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models.Expenses
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.ComponentModel.DataAnnotations;

    public class CreateExpenseVm
    {
        [Required]
        [Display(Name = "رقم المصروف")]
        public string ExpenseNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "تاريخ المصروف")]
        public DateTime ExpenseDateUtc { get; set; } = DateTime.UtcNow;

        [Required]
        [Display(Name = "تصنيف المصروف")]
        public Guid ExpenseCategoryId { get; set; }

        [Display(Name = "طريقة الدفع")]
        public Guid? PaymentMethodId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        [Display(Name = "المبلغ")]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "البيان")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> ExpenseCategories { get; set; } = new();
        public List<SelectListItem> PaymentMethods { get; set; } = new();
    }
}
