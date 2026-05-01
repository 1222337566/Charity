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
            builder.Property(x => x.EventCode).HasMaxLength(100);
            builder.Property(x => x.EventNameAr).HasMaxLength(200);
            builder.Property(x => x.ProfileNameAr).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(1000);

            builder.Property(x => x.MatchDonationType).HasMaxLength(50);
            builder.Property(x => x.MatchTargetingScopeCode).HasMaxLength(100);
            builder.Property(x => x.MatchPurposeName).HasMaxLength(200);
            builder.Property(x => x.MatchStoreMovementType).HasMaxLength(100);

            builder.HasIndex(x => x.SourceType);
            builder.HasIndex(x => new { x.SourceType, x.EventCode, x.IsActive });
            builder.HasIndex(x => new { x.SourceType, x.MatchDonationType, x.MatchTargetingScopeCode });

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
