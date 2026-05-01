using System;

namespace InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions
{
    public class BoardMeetingMinute
    {
        public Guid Id { get; set; }
        public Guid BoardMeetingId { get; set; }
        public string? LegalOpeningText { get; set; }
        public string? DiscussionSummary { get; set; }
        public string? RecommendationsSummary { get; set; }
        public string? LegalClosingText { get; set; }
        public string? FullMinuteText { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        public BoardMeeting? BoardMeeting { get; set; }
    }
}
