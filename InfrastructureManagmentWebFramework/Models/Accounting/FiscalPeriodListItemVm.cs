namespace InfrastructureManagmentWebFramework.Models.Accounting
{
    public class FiscalPeriodListItemVm
    {
        public Guid Id { get; set; }
        public string PeriodCode { get; set; } = string.Empty;
        public string PeriodNameAr { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsClosed { get; set; }
    }
}
