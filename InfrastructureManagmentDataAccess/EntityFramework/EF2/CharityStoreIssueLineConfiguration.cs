using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class CharityStoreIssueLineConfiguration : IEntityTypeConfiguration<CharityStoreIssueLine>
    {
        public void Configure(EntityTypeBuilder<CharityStoreIssueLine> builder)
        {
            builder.ToTable("CharityStoreIssueLines");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Quantity).HasColumnType("decimal(18,2)");
            builder.Property(x => x.UnitCost).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Notes).HasMaxLength(1000);

            builder.HasOne(x => x.Item)
                .WithMany()
                .HasForeignKey(x => x.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
