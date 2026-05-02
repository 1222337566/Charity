namespace InfrastructureManagmentServices.Charity.Beneficiaries
{
    public static class AidDisbursementExecutionStatusCodes
    {
        public const string Available = "Available";
        public const string PartiallyDisbursed = "PartiallyDisbursed";
        public const string FullyDisbursed = "FullyDisbursed";
        public const string Cancelled = "Cancelled";

        public static string ToDisplayName(string? statusCode) => statusCode switch
        {
            PartiallyDisbursed => "مصروف جزئيًا",
            FullyDisbursed => "تم الصرف",
            Cancelled => "ملغي",
            _ => "متاح للصرف"
        };
    }
}
