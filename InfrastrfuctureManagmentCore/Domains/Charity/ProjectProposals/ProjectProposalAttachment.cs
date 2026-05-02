using System;

namespace InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals
{
    public class ProjectProposalAttachment
    {
        public Guid Id { get; set; }
        public Guid ProjectProposalId { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string FileExtension { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public byte[] FileContent { get; set; } = Array.Empty<byte>();
        public string? AttachmentType { get; set; }
        public string? Notes { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public ProjectProposal? Proposal { get; set; }
    }
}
