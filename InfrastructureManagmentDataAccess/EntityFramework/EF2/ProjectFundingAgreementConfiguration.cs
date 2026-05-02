using InfrastrfuctureManagmentCore.Domains.Charity.Funding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class ProjectFundingAgreementConfiguration : IEntityTypeConfiguration<ProjectFundingAgreement>
    {
        public void Configure(EntityTypeBuilder<ProjectFundingAgreement> builder)
        {
            builder.ToTable("ProjectFundingAgreements");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.AgreementNumber).HasMaxLength(80).IsRequired();
            builder.Property(x => x.FundingAmount).HasPrecision(18, 2);
            builder.Property(x => x.ContactPerson).HasMaxLength(200);
            builder.Property(x => x.ContactEmail).HasMaxLength(200);
            builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Notes).HasMaxLength(1000);
            builder.Property(x => x.CreatedByUserId).HasMaxLength(450);

            builder.HasOne(x => x.Grantor)
                .WithMany(x => x.FundingAgreements)
                .HasForeignKey(x => x.GrantorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Project)
                .WithMany()
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.AgreementNumber).IsUnique();
            builder.HasIndex(x => x.GrantorId);
            builder.HasIndex(x => x.ProjectId);
            builder.HasIndex(x => x.Status);
        }
    }
}
