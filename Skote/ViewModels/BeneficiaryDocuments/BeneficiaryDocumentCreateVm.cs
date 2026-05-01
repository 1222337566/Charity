using Microsoft.AspNetCore.Mvc.Rendering;

namespace Skote.ViewModels.BeneficiaryDocuments;

public class BeneficiaryDocumentCreateVm
{
    public Guid? BeneficiaryId { get; set; }
    public string DocumentType { get; set; } = "";
    public string? Notes { get; set; }

    // الاسم الأساسي المعتمد
    public List<SelectListItem> Beneficiaries { get; set; } = new();

    // Alias للحفاظ على التوافق مع أي كود/Views قديمة تستخدم الاسم بالمفرد
    public List<SelectListItem> Beneficiary
    {
        get => Beneficiaries;
        set => Beneficiaries = value ?? new();
    }
}
