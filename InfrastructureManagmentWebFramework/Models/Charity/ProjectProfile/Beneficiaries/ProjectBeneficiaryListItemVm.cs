namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Beneficiaries
{
    public class ProjectBeneficiaryListItemVm
    {
        public Guid    Id              { get; set; }
        public Guid    BeneficiaryId   { get; set; }
        public string  BeneficiaryCode { get; set; } = string.Empty;
        public string  BeneficiaryName { get; set; } = string.Empty;
        public DateTime  EnrollmentDate { get; set; }
        public DateTime? ExitDate       { get; set; }
        public string? BenefitType      { get; set; }
        public string? TargetGroupName  { get; set; }   // ← الفئة المستهدفة
        public string? Notes            { get; set; }
    }
}
