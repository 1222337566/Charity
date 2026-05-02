using InfrastrfuctureManagmentCore.Domains.Charity.StockAdvanced;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.StockAdvanced
{
    public class StockReturnVoucherLineConfiguration : IEntityTypeConfiguration<StockReturnVoucherLine>
    {
        public void Configure(EntityTypeBuilder<StockReturnVoucherLine> builder)
        {
            builder.ToTable("CharityStockReturnVoucherLines");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Quantity).HasColumnType("decimal(18,2)");
            builder.Property(x => x.UnitCost).HasColumnType("decimal(18,2)");
        }
    }
}
