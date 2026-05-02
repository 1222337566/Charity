namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Grants
{
    public class ProjectGrantListItemVm
    {
        public Guid Id { get; set; }
        public Guid GrantAgreementId { get; set; }
        public string AgreementNumber { get; set; } = string.Empty;
        public string AgreementTitle { get; set; } = string.Empty;
        public string FunderName { get; set; } = string.Empty;
        public decimal AllocatedAmount { get; set; }
        public DateTime AllocatedDate { get; set; }
        public string? Notes { get; set; }
    }
}
