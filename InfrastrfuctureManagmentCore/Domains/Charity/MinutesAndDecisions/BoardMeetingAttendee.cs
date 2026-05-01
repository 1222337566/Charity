using System;

namespace InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions
{
    public class BoardMeetingAttendee
    {
        public Guid Id { get; set; }
        public Guid BoardMeetingId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? PositionTitle { get; set; }
        public string AttendanceStatus { get; set; } = "Present";
        public string? Notes { get; set; }

        public BoardMeeting? BoardMeeting { get; set; }
    }
}
