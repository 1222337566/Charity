using System;

namespace InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions
{
    public class BoardDecisionFollowUp
    {
        public Guid Id { get; set; }
        public Guid BoardDecisionId { get; set; }
        public DateTime FollowUpDate { get; set; }
        public string Status { get; set; } = "Open";
        public int? ProgressPercent { get; set; }
        public string? Details { get; set; }
        public string? NextAction { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public BoardDecision? BoardDecision { get; set; }
    }
}
