using System;
using System.Collections.Generic;

namespace InfrastrfuctureManagmentCore.Domains.Charity.Funding
{
    public class Grantor
    {
        public Guid Id { get; set; }

        public string GrantorCode { get; set; } = string.Empty;

        public string NameAr { get; set; } = string.Empty;

        public string? NameEn { get; set; }

        public string? ContactPerson { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public string? CreatedByUserId { get; set; }

        public ICollection<ProjectFundingAgreement> FundingAgreements { get; set; } = new List<ProjectFundingAgreement>();
    }
}
