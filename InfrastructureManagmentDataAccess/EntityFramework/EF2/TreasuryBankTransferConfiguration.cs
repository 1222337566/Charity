using InfrastrfuctureManagmentCore.Domains.Accounting.Treasury;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class TreasuryBankTransferConfiguration : IEntityTypeConfiguration<TreasuryBankTransfer>
    {
        public void Configure(EntityTypeBuilder<TreasuryBankTransfer> builder)
        {
            builder.ToTable("AccountingTreasuryBankTransfers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TransferNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Notes)
                .HasMaxLength(2000);

            builder.Property(x => x.Amount)
                .HasColumnType("decimal(18,2)");

            builder.HasIndex(x => x.TransferNumber)
                .IsUnique();

            builder.HasOne(x => x.FromFinancialAccount)
                .WithMany()
                .HasForeignKey(x => x.FromFinancialAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ToFinancialAccount)
                .WithMany()
                .HasForeignKey(x => x.ToFinancialAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
