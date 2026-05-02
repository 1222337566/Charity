using InfrastrfuctureManagmentCore.Domains.Charity.StockAdvanced;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.StockAdvanced
{
    public class StockNeedRequestLineConfiguration : IEntityTypeConfiguration<StockNeedRequestLine>
    {
        public void Configure(EntityTypeBuilder<StockNeedRequestLine> builder)
        {
            builder.ToTable("CharityStockNeedRequestLines");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.RequestedQuantity).HasColumnType("decimal(18,2)");
            builder.Property(x => x.ApprovedQuantity).HasColumnType("decimal(18,2)");
            builder.Property(x => x.FulfilledQuantity).HasColumnType("decimal(18,2)");
        }
    }
}
