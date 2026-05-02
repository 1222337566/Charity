using InfrastrfuctureManagmentCore.Domains.Charity.StockAdvanced;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.StockAdvanced
{
    public class StockDisposalVoucherConfiguration : IEntityTypeConfiguration<StockDisposalVoucher>
    {
        public void Configure(EntityTypeBuilder<StockDisposalVoucher> builder)
        {
            builder.ToTable("CharityStockDisposalVouchers");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.VoucherNumber).HasMaxLength(50).IsRequired();
            builder.Property(x => x.DisposalType).HasMaxLength(30).IsRequired();
            builder.HasMany(x => x.Lines).WithOne(x => x.StockDisposalVoucher).HasForeignKey(x => x.StockDisposalVoucherId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
