using InfrastrfuctureManagmentCore.Domains.Charity.Funders;

namespace InfrastrfuctureManagmentCore.Domains.Charity.Projects
{
    public class ProjectGrant
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid GrantAgreementId { get; set; }
        public decimal AllocatedAmount { get; set; }
        public DateTime AllocatedDate { get; set; }
        public string? Notes { get; set; }

        public CharityProject? Project { get; set; }
        public GrantAgreement? GrantAgreement { get; set; }
    }
}
