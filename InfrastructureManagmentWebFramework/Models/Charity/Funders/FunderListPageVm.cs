namespace InfrastructureManagmentWebFramework.Models.Charity.Funders
{
    public class FunderListPageVm
    {
        public FunderListFilterVm Filter { get; set; } = new();
        public List<FunderListRowVm> Rows { get; set; } = new();
        public int TotalCount { get; set; }
        public int ActiveCount { get; set; }
        public decimal TotalFunding { get; set; }
    }
}
