namespace InfrastrfuctureManagmentCore.Queries.Reports
{
    public class BeneficiaryStatusReportRowDto
    {
        public string StatusName { get; set; } = string.Empty;
        public int BeneficiariesCount { get; set; }
        public int AidRequestsCount { get; set; }
        public int DisbursementsCount { get; set; }
        public decimal TotalDisbursedAmount { get; set; }
    }
}
