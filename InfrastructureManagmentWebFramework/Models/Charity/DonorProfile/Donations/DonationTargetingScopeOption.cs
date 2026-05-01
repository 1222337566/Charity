namespace InfrastructureManagmentWebFramework.Models.Charity.DonorProfile.Donations
{
    public static class DonationTargetingScopeOption
    {
        public const string GeneralFund = "GeneralFund";
        public const string SpecificRequests = "SpecificRequests";
        public const string GeneralPurpose = "GeneralPurpose";

        public static IReadOnlyList<string> Values => new[]
        {
            GeneralFund,
            SpecificRequests,
            GeneralPurpose
        };

        public static string GetArabicName(string? code) => code switch
        {
            GeneralFund => "عام على حساب التبرعات العام",
            SpecificRequests => "مخصص لطلبات مساعدة معينة",
            GeneralPurpose => "مخصص لغرض / باب عام",
            _ => "مخصص لطلبات مساعدة معينة"
        };
    }
}
