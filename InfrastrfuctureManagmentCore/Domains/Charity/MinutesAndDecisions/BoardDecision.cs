using System;
using System.Collections.Generic;

namespace InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions
{
    public class BoardDecision
    {
        public Guid Id { get; set; }
        public Guid BoardMeetingId { get; set; }
        public string DecisionNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string DecisionKind { get; set; } = "Decision";
        public string? Description { get; set; }
        public string? ResponsibleParty { get; set; }
        public DateTime? DueDate { get; set; }
        public string Status { get; set; } = "Open";
        public string Priority { get; set; } = "Medium";
        public string? RelatedEntityType { get; set; }
        public Guid? RelatedEntityId { get; set; }
        public string? Notes { get; set; }
        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public string? UpdatedByUserId { get; set; }
        public string? UpdatedByUserName { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }

        public BoardMeeting? BoardMeeting { get; set; }
        public ICollection<BoardDecisionFollowUp> FollowUps { get; set; } = new List<BoardDecisionFollowUp>();
        public ICollection<BoardDecisionAttachment> Attachments { get; set; } = new List<BoardDecisionAttachment>();
    }
}
