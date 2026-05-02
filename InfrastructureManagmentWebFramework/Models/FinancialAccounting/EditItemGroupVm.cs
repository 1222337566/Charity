using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

public class EditItemGroupVm
{
    public Guid Id { get; set; }

    [Required]
    [Display(Name = "كود المجموعة")]
    public string GroupCode { get; set; } = string.Empty;

    [Required]
    [Display(Name = "اسم المجموعة")]
    public string GroupNameAr { get; set; } = string.Empty;

    [Display(Name = "الاسم الإنجليزي")]
    public string? GroupNameEn { get; set; }

    [Display(Name = "المجموعة الأب")]
    public Guid? ParentGroupId { get; set; }

    [Display(Name = "نشطة")]
    public bool IsActive { get; set; }

    public List<SelectListItem> ParentGroups { get; set; } = new();
}