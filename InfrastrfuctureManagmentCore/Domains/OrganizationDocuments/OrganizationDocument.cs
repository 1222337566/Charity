using System;

namespace InfrastrfuctureManagmentCore.Domains.OrganizationDocuments
{
    public class OrganizationDocument
    {
        public Guid Id { get; set; }

        public string DocumentNumber { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string DocumentType { get; set; } = "Other";

        public DateTime DocumentDateUtc { get; set; } = DateTime.UtcNow;

        public string? RelatedEntityType { get; set; }

        public Guid? RelatedEntityId { get; set; }

        public string? RelatedEntityName { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public byte[] FileContent { get; set; } = Array.Empty<byte>();

        public long FileSize { get; set; }

        public string? Notes { get; set; }

        public bool IsArchived { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public string? CreatedByUserId { get; set; }
    }
}
