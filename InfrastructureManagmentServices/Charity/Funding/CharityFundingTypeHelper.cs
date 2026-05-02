using System.Linq;

namespace InfrastructureManagmentServices.Charity.Funding
{
    public static class CharityFundingTypeHelper
    {
        public static string NormalizeAidKind(string? aidTypeName)
        {
            var name = (aidTypeName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(name))
                return "Unknown";

            if (ContainsAny(name, "مالي", "نقد", "كفالة", "إيجار", "سداد"))
                return "Cash";

            if (ContainsAny(name, "غذ", "سلة", "مواد غذائية", "إعاشة"))
                return "Food";

            if (ContainsAny(name, "طب", "علاج", "دواء", "مستلزمات طبية"))
                return "Medical";

            if (ContainsAny(name, "تعليم", "دراسة", "حقيبة", "مدرس"))
                return "Educational";

            if (ContainsAny(name, "كسوة", "ملابس", "بطاطين", "إغاث"))
                return "Relief";

            return "InKindGeneral";
        }

        public static bool IsCashAid(string? aidTypeName) => NormalizeAidKind(aidTypeName) == "Cash";

        public static bool IsInKindAid(string? aidTypeName) => !IsCashAid(aidTypeName);

        public static string GetExpectedDonationType(string? aidTypeName) => IsCashAid(aidTypeName) ? "نقدي" : "عيني";

        public static bool DonationMatchesAidRequest(string? donationType, string? aidTypeName)
        {
            var donation = (donationType ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(donation))
                return false;

            if (string.Equals(donation, "نقدي", System.StringComparison.OrdinalIgnoreCase))
                return IsCashAid(aidTypeName);

            if (string.Equals(donation, "عيني", System.StringComparison.OrdinalIgnoreCase))
                return IsInKindAid(aidTypeName);

            return false;
        }

        public static string BuildMismatchMessage(string? aidTypeName, string? donationType)
        {
            var expected = GetExpectedDonationType(aidTypeName);
            var donation = string.IsNullOrWhiteSpace(donationType) ? "غير محدد" : donationType!.Trim();
            return $"نوع التبرع المختار ({donation}) لا يطابق نوع طلب المساعدة. النوع المتوقع لهذه المساعدة هو ({expected}).";
        }

        private static bool ContainsAny(string value, params string[] needles)
            => needles.Any(x => value.Contains(x, System.StringComparison.OrdinalIgnoreCase));
    }
}
