namespace InfrastructureManagmentWebFramework.Models.Charity.Donors
{
    public class DonorListPageVm
    {
        public DonorListFilterVm Filter { get; set; } = new();
        public List<DonorListRowVm> Rows { get; set; } = new();
        public int TotalCount { get; set; }
        public int ActiveCount { get; set; }
        public decimal TotalDonations { get; set; }
    }
}
