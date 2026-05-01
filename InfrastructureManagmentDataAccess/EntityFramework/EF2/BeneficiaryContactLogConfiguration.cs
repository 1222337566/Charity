using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class BeneficiaryContactLogConfiguration : IEntityTypeConfiguration<BeneficiaryContactLog>
    {
        public void Configure(EntityTypeBuilder<BeneficiaryContactLog> b)
        {
            b.ToTable("BeneficiaryContactLogs");
            b.HasKey(x => x.Id);
            b.Property(x => x.ContactType).HasMaxLength(30);
            b.Property(x => x.Outcome).HasMaxLength(30);
            b.Property(x => x.Subject).HasMaxLength(300);
            b.Property(x => x.Notes).HasMaxLength(2000);
            b.Property(x => x.FollowUpNote).HasMaxLength(500);
            b.HasIndex(x => x.BeneficiaryId);
            b.HasIndex(x => x.FollowUpDate);
        }
    }
}
