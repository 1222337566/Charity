namespace InfrastrfuctureManagmentCore.Domains.Accounting.Integration
{
    public static class AccountingPostingEventCodes
    {
        public const string DonationReceipt = "DonationReceipt";
        public const string InKindDonationReceipt = "InKindDonationReceipt";
        public const string AidCashDisbursement = "AidCashDisbursement";
        public const string AidInKindIssue = "AidInKindIssue";
    }

    public static class AccountingPostingModuleCodes
    {
        public const string Donations = "Donations";
        public const string Aid = "Aid";
        public const string Inventory = "Inventory";
    }
}
