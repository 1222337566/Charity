using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using InfrastrfuctureManagmentCore.Domains.Financial;

namespace InfrastructureManagmentWebFramework.Models.FinancialAccounting
{
    public class CreateAccountVm
    {
        [Required]
        [Display(Name = "كود الحساب")]
        public string AccountCode { get; set; } = string.Empty;

        [Required]
        [Display(Name = "اسم الحساب")]
        public string AccountNameAr { get; set; } = string.Empty;

        [Display(Name = "الاسم الإنجليزي")]
        public string? AccountNameEn { get; set; }

        [Display(Name = "الحساب الأب")]
        public Guid? ParentAccountId { get; set; }

        [Display(Name = "فئة الحساب الرئيسي")]
        public AccountCategory? Category { get; set; }

        [Display(Name = "حساب حركي")]
        public bool IsPosting { get; set; }

        [Display(Name = "يتطلب مشروع")]
        public bool RequiresProject { get; set; }

        [Display(Name = "يتطلب مركز تكلفة")]
        public bool RequiresCostCenter { get; set; }

        [Display(Name = "طبيعة حساب النقدية")]
        public FinancialAccountCashKind CashKind { get; set; } = FinancialAccountCashKind.None;

        [Display(Name = "السماح برصيد سالب")]
        public bool AllowNegativeCashBalance { get; set; }

        public List<SelectListItem> ParentAccounts { get; set; } = new();
        public List<SelectListItem> Categories { get; set; } = new();
        public List<SelectListItem> CashKinds { get; set; } = new();
    }
}
