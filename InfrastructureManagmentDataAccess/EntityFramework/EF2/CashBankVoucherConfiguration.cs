using InfrastrfuctureManagmentCore.Domains.Accounting.Treasury;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class CashBankVoucherConfiguration : IEntityTypeConfiguration<CashBankVoucher>
    {
        public void Configure(EntityTypeBuilder<CashBankVoucher> builder)
        {
            builder.ToTable("AccountingCashBankVouchers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.VoucherNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Notes)
                .HasMaxLength(2000);

            builder.Property(x => x.Amount)
                .HasColumnType("decimal(18,2)");

            builder.HasIndex(x => x.VoucherNumber)
                .IsUnique();

            builder.HasOne(x => x.FinancialAccount)
                .WithMany()
                .HasForeignKey(x => x.FinancialAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.OppositeAccount)
                .WithMany()
                .HasForeignKey(x => x.OppositeAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
