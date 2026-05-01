namespace Skote.ViewModels.BeneficiaryDocuments;

public class BeneficiaryDocumentListItemVm
{
    public Guid Id { get; set; }
    public Guid BeneficiaryId { get; set; }
    public string BeneficiaryCode { get; set; } = "";
    public string BeneficiaryName { get; set; } = "";
    public string DocumentType { get; set; } = "";
    public string OriginalFileName { get; set; } = "";
    public long FileSizeBytes { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
