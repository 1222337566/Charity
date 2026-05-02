namespace InfrastructureManagmentServices.Charity.Funding
{
    public class RebuildAidDisbursementFundingLinesResult
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public int LinesCreated { get; set; }
        public decimal TotalConsumedAmount { get; set; }
        public List<Guid> DonationIdsUsed { get; set; } = new();
    }
}
