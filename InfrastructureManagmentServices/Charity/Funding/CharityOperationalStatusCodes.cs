namespace InfrastructureManagmentServices.Charity.Funding
{
    public static class CharityOperationalStatusCodes
    {
        public const string Open = "Open";
        public const string Closed = "Closed";
        public const string Pending = "Pending";
        public const string Rejected = "Rejected";

        public const string Unallocated = "Unallocated";
        public const string PartiallyAllocated = "PartiallyAllocated";
        public const string FullyAllocated = "FullyAllocated";

        public const string NotReceivedToStore = "NotReceivedToStore";
        public const string PartiallyReceivedToStore = "PartiallyReceivedToStore";
        public const string ReceivedToStore = "ReceivedToStore";

        public const string NotIssued = "NotIssued";
        public const string PartiallyIssued = "PartiallyIssued";
        public const string FullyIssued = "FullyIssued";
    }
}
