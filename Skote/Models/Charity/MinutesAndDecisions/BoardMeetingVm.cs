using System;
using System.Collections.Generic;
using InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions;

namespace Skote.Models.Charity.MinutesAndDecisions
{
    public class BoardMeetingVm
    {
        public Guid? Id { get; set; }
        public string MeetingNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime MeetingDate { get; set; } = DateTime.Today;
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? Location { get; set; }
        public string? MeetingType { get; set; }
        public string? Status { get; set; } = "Draft";
        public string? Agenda { get; set; }
        public string? Notes { get; set; }

        public string? LegalOpeningText { get; set; }
        public string? DiscussionSummary { get; set; }
        public string? RecommendationsSummary { get; set; }
        public string? LegalClosingText { get; set; }
        public string? FullMinuteText { get; set; }

        public List<BoardMeetingAttendeeVm> Attendees { get; set; } = new();
    }

    public class BoardMeetingAttendeeVm
    {
        public Guid? Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? PositionTitle { get; set; }
        public string AttendanceStatus { get; set; } = "Present";
        public string? Notes { get; set; }
    }
}
