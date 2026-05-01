namespace InfrastrfuctureManagmentCore.Domains.Charity.Reports
{
    public class MonthlyAidReportRow
    {
        public Guid BeneficiaryId { get; set; }
        public string BeneficiaryCode { get; set; } = string.Empty;
        public string BeneficiaryName { get; set; } = string.Empty;
        public string? AidTypeName { get; set; }
        public DateTime DisbursementDate { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethodName { get; set; }
        public string? CycleName { get; set; }
    }
}
