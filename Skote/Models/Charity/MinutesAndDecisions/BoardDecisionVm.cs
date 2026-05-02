using System;

namespace Skote.Models.Charity.MinutesAndDecisions
{
    public class BoardDecisionVm
    {
        public Guid? Id { get; set; }
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
    }

    public class BoardDecisionFollowUpVm
    {
        public Guid BoardDecisionId { get; set; }
        public DateTime FollowUpDate { get; set; } = DateTime.Today;
        public string Status { get; set; } = "Open";
        public int? ProgressPercent { get; set; }
        public string? Details { get; set; }
        public string? NextAction { get; set; }
    }
}
