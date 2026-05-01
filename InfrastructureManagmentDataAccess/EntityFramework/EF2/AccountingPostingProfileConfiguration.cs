using InfrastrfuctureManagmentCore.Domains.Accounting.Integration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class AccountingPostingProfileConfiguration : IEntityTypeConfiguration<AccountingPostingProfile>
    {
        public void Configure(EntityTypeBuilder<AccountingPostingProfile> builder)
        {
            builder.ToTable("AccountingPostingProfiles");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code).HasMaxLength(80).IsRequired();
            builder.Property(x => x.NameAr).HasMaxLength(200).IsRequired();
            builder.Property(x => x.NameEn).HasMaxLength(200);
            builder.Property(x => x.Description).HasMaxLength(1000);
            builder.Property(x => x.ModuleCode).HasMaxLength(80).IsRequired();
            builder.Property(x => x.EventCode).HasMaxLength(80).IsRequired();
            builder.Property(x => x.DonationType).HasMaxLength(80);
            builder.Property(x => x.TargetingScopeCode).HasMaxLength(40);
            builder.Property(x => x.PurposeName).HasMaxLength(200);
            builder.Property(x => x.Notes).HasMaxLength(1000);

            builder.HasIndex(x => x.Code).IsUnique();
            builder.HasIndex(x => new { x.EventCode, x.TargetingScopeCode, x.PurposeName, x.DonationType });

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
