namespace InfrastructureManagmentServices.Charity.Funding
{
    public class DonationOperationalStatusSnapshot
    {
        public Guid DonationId { get; set; }
        public string DonationType { get; set; } = string.Empty;

        public decimal DonationAmount { get; set; }
        public decimal AllocatedAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string AllocationStatusCode { get; set; } = CharityOperationalStatusCodes.Unallocated;
        public string AllocationStatusName { get; set; } = "غير مخصص";

        public decimal TotalQuantity { get; set; }
        public decimal AllocatedQuantity { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public decimal IssuedQuantity { get; set; }
        public decimal RemainingQuantity { get; set; }
        public string StoreStatusCode { get; set; } = CharityOperationalStatusCodes.NotReceivedToStore;
        public string StoreStatusName { get; set; } = "لم يدخل المخزن";
        public string IssueStatusCode { get; set; } = CharityOperationalStatusCodes.NotIssued;
        public string IssueStatusName { get; set; } = "لم يُصرف";

        public string OperationalStatusCode { get; set; } = CharityOperationalStatusCodes.Open;
        public string OperationalStatusName { get; set; } = "مفتوح";
        public bool IsClosed { get; set; }
    }
}
