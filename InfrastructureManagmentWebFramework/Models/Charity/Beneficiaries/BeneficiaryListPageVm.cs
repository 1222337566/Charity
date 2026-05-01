namespace InfrastructureManagmentWebFramework.Models.Charity.Beneficiaries
{
    public class BeneficiaryListPageVm
    {
        public BeneficiaryListFilterVm Filter { get; set; } = new();
        public List<BeneficiaryListRowVm> Rows { get; set; } = new();
        public int TotalCount { get; set; }
        public int ActiveCount { get; set; }
        public int ApprovedCount { get; set; }
    }
}
