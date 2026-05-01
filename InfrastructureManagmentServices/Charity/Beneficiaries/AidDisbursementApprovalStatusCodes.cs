namespace InfrastructureManagmentServices.Charity.Beneficiaries
{
    public static class AidDisbursementApprovalStatusCodes
    {
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";

        public static string ToDisplayName(string? statusCode) => statusCode switch
        {
            Approved => "معتمد",
            Rejected => "مرفوض",
            _ => "بانتظار الاعتماد"
        };
    }
}
