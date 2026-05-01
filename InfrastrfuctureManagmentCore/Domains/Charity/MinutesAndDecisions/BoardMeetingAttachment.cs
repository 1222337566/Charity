using System;

namespace InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions;

public class BoardMeetingAttachment
{
    public Guid Id { get; set; }
    public Guid BoardMeetingId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? FilePath { get; set; }
    public string? AttachmentType { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public BoardMeeting? BoardMeeting { get; set; }
}
