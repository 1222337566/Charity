using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class CharityStoreReceiptLineConfiguration : IEntityTypeConfiguration<CharityStoreReceiptLine>
    {
        public void Configure(EntityTypeBuilder<CharityStoreReceiptLine> builder)
        {
            builder.ToTable("CharityStoreReceiptLines");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Quantity).HasColumnType("decimal(18,2)");
            builder.Property(x => x.UnitCost).HasColumnType("decimal(18,2)");
            builder.Property(x => x.BatchNo).HasMaxLength(100);
            builder.Property(x => x.Notes).HasMaxLength(1000);

            builder.HasOne(x => x.Item)
                .WithMany()
                .HasForeignKey(x => x.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
