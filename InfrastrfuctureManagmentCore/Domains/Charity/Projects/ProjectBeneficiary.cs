using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;

namespace InfrastrfuctureManagmentCore.Domains.Charity.Projects
{
    public class ProjectBeneficiary
    {
        public Guid Id { get; set; }
        public Guid ProjectId      { get; set; }
        public Guid BeneficiaryId  { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public DateTime? ExitDate  { get; set; }
        public string? BenefitType { get; set; }
        public string? Notes       { get; set; }

        // ── الفئة المستهدفة من المقترح ──
        public string? TargetGroupName { get; set; }

        public CharityProject? Project    { get; set; }
        public Beneficiary?    Beneficiary { get; set; }
    }
}
