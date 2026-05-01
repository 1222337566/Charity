using InfrastrfuctureManagmentCore.Domains.Charity.Funders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class GrantAgreementConfiguration : IEntityTypeConfiguration<GrantAgreement>
    {
        public void Configure(EntityTypeBuilder<GrantAgreement> builder)
        {
            builder.ToTable("CharityGrantAgreements");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AgreementNumber).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Title).HasMaxLength(300).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(4000);
            builder.Property(x => x.Currency).HasMaxLength(20).IsRequired();
            builder.Property(x => x.PaymentTerms).HasMaxLength(4000);
            builder.Property(x => x.ReportingRequirements).HasMaxLength(4000);
            builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Notes).HasMaxLength(4000);
            builder.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");

            builder.HasOne(x => x.Funder)
                .WithMany(x => x.GrantAgreements)
                .HasForeignKey(x => x.FunderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.AgreementNumber).IsUnique();
            builder.HasIndex(x => new { x.FunderId, x.Status });
        }
    }
}
