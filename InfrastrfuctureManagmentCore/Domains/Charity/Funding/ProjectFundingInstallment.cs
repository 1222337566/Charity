using System;

namespace InfrastrfuctureManagmentCore.Domains.Charity.Funding
{
    public class ProjectFundingInstallment
    {
        public Guid Id { get; set; }

        public Guid ProjectFundingAgreementId { get; set; }

        public ProjectFundingAgreement? ProjectFundingAgreement { get; set; }

        public string InstallmentNumber { get; set; } = string.Empty;

        public DateTime DueDateUtc { get; set; }

        public DateTime? ReceivedDateUtc { get; set; }

        public decimal Amount { get; set; }

        public string Status { get; set; } = "Due";

        public string? ReceiptDocumentNumber { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
