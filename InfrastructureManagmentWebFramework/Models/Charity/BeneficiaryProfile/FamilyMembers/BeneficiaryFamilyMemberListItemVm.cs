namespace InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.FamilyMembers
{
    public class BeneficiaryFamilyMemberListItemVm
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty;
        public string? NationalId { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? GenderText { get; set; }
        public string? EducationStatus { get; set; }
        public string? WorkStatus { get; set; }
        public decimal? MonthlyIncome { get; set; }
        public bool IsDependent { get; set; }
    }
}
