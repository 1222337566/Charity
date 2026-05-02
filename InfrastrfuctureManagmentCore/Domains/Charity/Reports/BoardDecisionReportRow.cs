namespace InfrastrfuctureManagmentCore.Domains.Charity.Reports
{
    public class BoardDecisionReportRow
    {
        public Guid MeetingId { get; set; }
        public string MeetingNumber { get; set; } = string.Empty;
        public string MeetingTitle { get; set; } = string.Empty;
        public DateTime MeetingDate { get; set; }
        public string? DecisionNumber { get; set; }
        public string? DecisionTitle { get; set; }
        public string? DecisionStatus { get; set; }
        public string? ResponsibleParty { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
