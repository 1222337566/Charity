using InfrastrfuctureManagmentCore.Domains.Charity.StockAdvanced;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.StockAdvanced
{
    public class StockReturnVoucherConfiguration : IEntityTypeConfiguration<StockReturnVoucher>
    {
        public void Configure(EntityTypeBuilder<StockReturnVoucher> builder)
        {
            builder.ToTable("CharityStockReturnVouchers");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.VoucherNumber).HasMaxLength(50).IsRequired();
            builder.Property(x => x.ReturnType).HasMaxLength(30).IsRequired();
            builder.HasMany(x => x.Lines).WithOne(x => x.StockReturnVoucher).HasForeignKey(x => x.StockReturnVoucherId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
