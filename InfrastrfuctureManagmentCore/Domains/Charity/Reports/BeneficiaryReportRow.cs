namespace InfrastrfuctureManagmentCore.Domains.Charity.Reports
{
    public class BeneficiaryReportRow
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? NationalId { get; set; }
        public int FamilyMembersCount { get; set; }
        public decimal MonthlyIncome { get; set; }
        public string? StatusName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
