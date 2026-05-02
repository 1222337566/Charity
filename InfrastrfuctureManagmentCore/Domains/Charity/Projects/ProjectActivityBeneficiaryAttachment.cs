namespace InfrastrfuctureManagmentCore.Domains.Charity.Projects
{
    /// <summary>
    /// مرفق إثبات مشاركة مستفيد في نشاط
    /// </summary>
    public class ProjectActivityBeneficiaryAttachment
    {
        public Guid Id { get; set; }
        public Guid ActivityBeneficiaryId { get; set; }

        /// <summary>AttendancePhoto | Signature | ParticipationForm | IdPhoto | ActivityReport | Other</summary>
        public string AttachmentType     { get; set; } = "Other";
        public string OriginalFileName   { get; set; } = string.Empty;
        public string ContentType        { get; set; } = string.Empty;
        public long   FileSizeBytes      { get; set; }
        public byte[] FileContent        { get; set; } = Array.Empty<byte>();
        public string? Notes             { get; set; }
        public string? UploadedByUserId  { get; set; }
        public DateTime CreatedAtUtc     { get; set; } = DateTime.UtcNow;

        public ProjectActivityBeneficiary? ActivityBeneficiary { get; set; }
    }
}
