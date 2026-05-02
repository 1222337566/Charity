namespace Skote.Helpers
{
    public static class KafalaArabic
    {
        public static string SponsorshipType(string? v) => v switch {
            "Orphan"    => "كفالة يتيم",
            "Monthly"   => "كفالة شهرية",
            "Health"    => "كفالة صحية",
            "Education" => "كفالة تعليم",
            "Family"    => "كفالة أسرة",
            _ => v ?? "-"
        };

        public static string Frequency(string? v) => v switch {
            "Monthly"    => "شهري",
            "Quarterly"  => "ربع سنوي",
            "SemiAnnual" => "نصف سنوي",
            "Annual"     => "سنوي",
            _ => v ?? "-"
        };

        public static string CaseStatus(string? v) => v switch {
            "Draft"     => "مسودة",
            "Active"    => "نشطة",
            "Suspended" => "موقوفة",
            "Closed"    => "مغلقة",
            _ => v ?? "-"
        };

        public static string CaseStatusBadge(string? v) => v switch {
            "Active"    => "success",
            "Suspended" => "warning",
            "Closed"    => "secondary",
            "Draft"     => "info",
            _ => "light"
        };

        public static string SponsorType(string? v) => v switch {
            "Individual"  => "فرد",
            "Company"     => "شركة",
            "Institution" => "مؤسسة",
            _ => v ?? "-"
        };

        public static string Direction(string? v) => v switch {
            "Received"   => "تحصيل",
            "Disbursed"  => "صرف",
            "Adjustment" => "تسوية",
            _ => v ?? "-"
        };

        public static string DirectionBadge(string? v) => v switch {
            "Received"   => "success",
            "Disbursed"  => "primary",
            "Adjustment" => "warning",
            _ => "secondary"
        };

        public static string PaymentStatus(string? v) => v switch {
            "Confirmed" => "مؤكد",
            "Draft"     => "مسودة",
            "Reversed"  => "معكوس",
            _ => v ?? "-"
        };

        public static string PaymentStatusBadge(string? v) => v switch {
            "Confirmed" => "success",
            "Draft"     => "info",
            "Reversed"  => "danger",
            _ => "secondary"
        };
    }
}
