using System;
using System.Collections.Generic;

namespace InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions
{
    public class BoardMeeting
    {
        public Guid Id { get; set; }
        public string MeetingNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime MeetingDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? Location { get; set; }
        public string? MeetingType { get; set; }
        public string? Status { get; set; } = "Draft";
        public string? Agenda { get; set; }
        public string? Notes { get; set; }
        public string? PreparedByUserId { get; set; }
        public string? ApprovedByUserId { get; set; }
        public DateTime? ApprovedAtUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
        public bool IsActive { get; set; } = true;

        public BoardMeetingMinute? Minute { get; set; }
        public ICollection<BoardMeetingAttendee> Attendees { get; set; } = new List<BoardMeetingAttendee>();
        public ICollection<BoardDecision> Decisions { get; set; } = new List<BoardDecision>();
        public ICollection<BoardMeetingAttachment> Attachments { get; set; } = new List<BoardMeetingAttachment>();
    }
}
