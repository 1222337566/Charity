namespace InfrastructureManagmentWebFramework.Models.Charity.Beneficiaries
{
    public class BeneficiaryListRowVm
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? NationalId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? GenderText { get; set; }
        public string? GovernorateName { get; set; }
        public string? StatusText { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int FamilyMembersCount { get; set; }
        public bool IsActive { get; set; }
    }
}
