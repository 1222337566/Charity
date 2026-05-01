namespace InfrastrfuctureManagmentCore.Domains.Charity.Projects
{
    public class ProjectTeamMemberAttachment
    {
        public Guid   Id              { get; set; }
        public Guid   TeamMemberId   { get; set; }
        /// <summary>AttendanceSheet | Contract | IdPhoto | Certificate | Other</summary>
        public string AttachmentType  { get; set; } = "Other";
        public string OriginalFileName { get; set; } = string.Empty;
        public string ContentType      { get; set; } = string.Empty;
        public long   FileSizeBytes    { get; set; }
        public byte[] FileContent      { get; set; } = Array.Empty<byte>();
        public string? Notes           { get; set; }
        public string? UploadedByUserId { get; set; }
        public DateTime CreatedAtUtc   { get; set; } = DateTime.UtcNow;

        public ProjectTeamMember? TeamMember { get; set; }
    }
}
