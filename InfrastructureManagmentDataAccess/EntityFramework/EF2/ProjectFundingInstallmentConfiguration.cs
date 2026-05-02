using InfrastrfuctureManagmentCore.Domains.Charity.Funding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class ProjectFundingInstallmentConfiguration : IEntityTypeConfiguration<ProjectFundingInstallment>
    {
        public void Configure(EntityTypeBuilder<ProjectFundingInstallment> builder)
        {
            builder.ToTable("ProjectFundingInstallments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.InstallmentNumber).HasMaxLength(80).IsRequired();
            builder.Property(x => x.Amount).HasPrecision(18, 2);
            builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
            builder.Property(x => x.ReceiptDocumentNumber).HasMaxLength(100);
            builder.Property(x => x.Notes).HasMaxLength(1000);

            builder.HasOne(x => x.ProjectFundingAgreement)
                .WithMany(x => x.Installments)
                .HasForeignKey(x => x.ProjectFundingAgreementId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.ProjectFundingAgreementId);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.DueDateUtc);
        }
    }
}
