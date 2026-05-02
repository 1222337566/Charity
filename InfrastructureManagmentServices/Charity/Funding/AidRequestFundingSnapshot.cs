namespace InfrastructureManagmentServices.Charity.Funding
{
    public sealed class AidRequestFundingSnapshot
    {
        public Guid AidRequestId { get; set; }
        public decimal RequestedAmount { get; set; }
        public decimal FundedAmount { get; set; }
        public decimal DisbursedAmount { get; set; }
        public decimal RemainingToFundAmount { get; set; }
        public decimal RemainingToDisburseAmount { get; set; }
        public decimal RemainingOnRequestAmount { get; set; }

        public string FundingStatusCode { get; set; } = AidRequestFundingStatusCodes.NotFunded;
        public string DisbursementStatusCode { get; set; } = AidRequestFundingStatusCodes.NotDisbursed;

        public string FundingStatusName => FundingStatusCode switch
        {
            AidRequestFundingStatusCodes.FullyFunded => "ممول بالكامل",
            AidRequestFundingStatusCodes.PartiallyFunded => "ممول جزئيًا",
            _ => "غير ممول"
        };

        public string DisbursementStatusName => DisbursementStatusCode switch
        {
            AidRequestFundingStatusCodes.FullyDisbursed => "مصروف بالكامل",
            AidRequestFundingStatusCodes.PartiallyDisbursed => "مصروف جزئيًا",
            _ => "غير مصروف"
        };
    }
}
