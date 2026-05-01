using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastructureManagmentCore.Domains.Profiling;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AccountConfiguration : IEntityTypeConfiguration<FinancialAccount>
{
    public void Configure(EntityTypeBuilder<FinancialAccount> builder)
    {
        builder.ToTable("FinacialAccounts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AccountCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.AccountNameAr)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.AccountNameEn)
            .HasMaxLength(200);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Property(x => x.Category)
            .IsRequired();

        builder.Property(x => x.CashKind)
            .IsRequired();

        builder.Property(x => x.AllowNegativeCashBalance)
            .HasDefaultValue(false);

        builder.Property(x => x.Level)
            .IsRequired();

        builder.HasIndex(x => x.AccountCode)
            .IsUnique();

        builder.HasOne(x => x.ParentAccount)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentAccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}