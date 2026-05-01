namespace InfrastructureManagmentServices.Charity.Funding
{
    public class AidRequestOperationalStatusSnapshot
    {
        public Guid AidRequestId { get; set; }
        public string WorkflowStatus { get; set; } = string.Empty;
        public string OperationalStatusCode { get; set; } = CharityOperationalStatusCodes.Open;
        public string OperationalStatusName { get; set; } = "مفتوح";
        public bool IsClosed { get; set; }

        public decimal RequestedAmount { get; set; }
        public decimal FundedAmount { get; set; }
        public decimal DisbursedAmount { get; set; }
        public decimal RemainingToFundAmount { get; set; }
        public decimal RemainingToDisburseAmount { get; set; }

        public string FundingStatusCode { get; set; } = AidRequestFundingStatusCodes.NotFunded;
        public string FundingStatusName { get; set; } = "غير ممول";
        public string DisbursementStatusCode { get; set; } = AidRequestFundingStatusCodes.NotDisbursed;
        public string DisbursementStatusName { get; set; } = "غير مصروف";
    }
}
