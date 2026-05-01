namespace InfrastrfuctureManagmentCore.Queries.Reports
{
    public class BeneficiaryAidDetailReportRowDto
    {
        public Guid BeneficiaryId { get; set; }
        public string BeneficiaryCode { get; set; } = string.Empty;
        public string BeneficiaryName { get; set; } = string.Empty;
        public string BeneficiaryStatus { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public string AidTypeName { get; set; } = string.Empty;
        public decimal RequestedAmount { get; set; }
        public string RequestStatus { get; set; } = string.Empty;
        public decimal DisbursedAmount { get; set; }
        public DateTime? LastDisbursementDate { get; set; }
    }
}
