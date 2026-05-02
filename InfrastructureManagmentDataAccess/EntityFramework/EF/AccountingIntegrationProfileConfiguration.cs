using InfrastrfuctureManagmentCore.Domains.Accounting.Integration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class AccountingIntegrationProfileConfiguration : IEntityTypeConfiguration<AccountingIntegrationProfile>
    {
        public void Configure(EntityTypeBuilder<AccountingIntegrationProfile> builder)
        {
            builder.ToTable("AccountingIntegrationProfiles");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SourceType).HasMaxLength(100).IsRequired();
            builder.Property(x => x.ProfileNameAr).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(1000);

            builder.HasIndex(x => x.SourceType).IsUnique();

            builder.HasOne(x => x.DebitAccount)
                .WithMany()
                .HasForeignKey(x => x.DebitAccountId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.CreditAccount)
                .WithMany()
                .HasForeignKey(x => x.CreditAccountId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.DefaultCostCenter)
                .WithMany()
                .HasForeignKey(x => x.DefaultCostCenterId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
