using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using System;
using System.Collections.Generic;

namespace InfrastrfuctureManagmentCore.Domains.Charity.Funding
{
    public class ProjectFundingAgreement
    {
        public Guid Id { get; set; }

        public string AgreementNumber { get; set; } = string.Empty;

        public Guid GrantorId { get; set; }

        public Grantor? Grantor { get; set; }

        public Guid ProjectId { get; set; }

        public CharityProject? Project { get; set; }

        public decimal FundingAmount { get; set; }

        public DateTime StartDateUtc { get; set; }

        public DateTime? EndDateUtc { get; set; }

        public string? ContactPerson { get; set; }

        public string? ContactEmail { get; set; }

        public string Status { get; set; } = "Active";

        public string? Notes { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public string? CreatedByUserId { get; set; }

        public ICollection<ProjectFundingInstallment> Installments { get; set; } = new List<ProjectFundingInstallment>();
    }
}
