using InfrastrfuctureManagmentCore.Domains.Charity.StockAdvanced;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.StockAdvanced
{
    public class StockNeedRequestConfiguration : IEntityTypeConfiguration<StockNeedRequest>
    {
        public void Configure(EntityTypeBuilder<StockNeedRequest> builder)
        {
            builder.ToTable("CharityStockNeedRequests");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.RequestNumber).HasMaxLength(50).IsRequired();
            builder.Property(x => x.RequestType).HasMaxLength(30).IsRequired();
            builder.Property(x => x.Status).HasMaxLength(30).IsRequired();
            builder.Property(x => x.RequestedByName).HasMaxLength(200);
            builder.Property(x => x.AidRequestLineDescriptionSnapshot).HasMaxLength(1000);
            builder.HasIndex(x => x.BeneficiaryAidRequestLineId).IsUnique().HasFilter("[BeneficiaryAidRequestLineId] IS NOT NULL");
            builder.HasMany(x => x.Lines).WithOne(x => x.StockNeedRequest).HasForeignKey(x => x.StockNeedRequestId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
