using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using InfrastrfuctureManagmentCore.Domains.Financial;

public class EditAccountVm
{
    public Guid Id { get; set; }

    [Required]
    [Display(Name = "كود الحساب")]
    public string AccountCode { get; set; } = string.Empty;

    [Required]
    [Display(Name = "اسم الحساب")]
    public string AccountNameAr { get; set; } = string.Empty;

    [Display(Name = "الاسم الإنجليزي")]
    public string? AccountNameEn { get; set; }

    [Required]
    [Display(Name = "الحساب الأب")]
    public Guid? ParentAccountId { get; set; }

    [Display(Name = "حساب حركي")]
    public bool IsPosting { get; set; }

    [Display(Name = "نشط")]
    public bool IsActive { get; set; }

    [Display(Name = "يتطلب مشروع")]
    public bool RequiresProject { get; set; }

    [Display(Name = "يتطلب مركز تكلفة")]
    public bool RequiresCostCenter { get; set; }

    [Display(Name = "طبيعة حساب النقدية")]
    public FinancialAccountCashKind CashKind { get; set; } = FinancialAccountCashKind.None;

    [Display(Name = "السماح برصيد سالب")]
    public bool AllowNegativeCashBalance { get; set; }

    public List<SelectListItem> ParentAccounts { get; set; } = new();
    public List<SelectListItem> CashKinds { get; set; } = new();
}