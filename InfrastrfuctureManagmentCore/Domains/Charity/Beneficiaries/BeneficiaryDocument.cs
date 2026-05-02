namespace InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;

public class BeneficiaryDocument
{
    public Guid Id { get; set; }
    public Guid BeneficiaryId { get; set; }

    public string DocumentType { get; set; } = null!;
    public string? Notes { get; set; }

    public string OriginalFileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public string FileExtension { get; set; } = null!;
    public long FileSizeBytes { get; set; }
    public byte[] FileContent { get; set; } = Array.Empty<byte>();

    public string? CreatedByUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public virtual Beneficiary Beneficiary { get; set; } = null!;
}
